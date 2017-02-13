#include "Input.h"

#include <Mmsystem.h>
#include <map>
#include <time.h>

UINT timerId = 0;
std::map<int, bool> keyStates; // true == pressed
KeyDownCallback keyDownCallback;

void CALLBACK TimerCallback(UINT uTimerID, UINT uMsg, DWORD_PTR dwUser, DWORD_PTR dw1, DWORD_PTR dw2)
{
	if (!keyDownCallback) {
		return;
	}

	for (auto &key : keyStates)
	{
		short keyState = GetAsyncKeyState(key.first);
		
		bool isPressed = keyState & 0x8000;

		if (!key.second && isPressed) {
			keyDownCallback(key.first);
		}

		key.second = isPressed;
	}
}

void RegisterInput(int keycode)
{
	keyStates[keycode] = false;
}

void StartPolling()
{
	timeBeginPeriod(1);

	MMRESULT result = timeSetEvent(1, 1, TimerCallback, NULL, TIME_PERIODIC);

	if (result == NULL)
	{
		OutputDebugString("TimeSetEvent failed");
		return;
	}

	timerId = result;
}

void StopPolling()
{
	if (timerId == 0) 
	{
		return;
	}

	timeEndPeriod(1);

	timeKillEvent(timerId);
	timerId = 0;

	keyDownCallback = NULL;
	keyStates.clear();
}

void SetKeyDownCallback(KeyDownCallback callback)
{
	keyDownCallback = callback;
}