#include "Input.h"

#include <Mmsystem.h>
#include <map>
#include <mutex>
#include <queue>
#include <time.h>

UINT timerId = 0;
std::map<int, bool> keyStates; // true == pressed

std::mutex eventMutex;
std::queue<keyEvent*> events;

double GetCurrentTimeStamp()
{
	return clock() / (double)CLOCKS_PER_SEC;
}

void CALLBACK TimerCallback(UINT uTimerID, UINT uMsg, DWORD_PTR dwUser, DWORD_PTR dw1, DWORD_PTR dw2)
{
	for (auto &key : keyStates)
	{
		short keyState = GetAsyncKeyState(key.first);
		
		bool isPressed = keyState & 0x8000;

		if (!key.second && isPressed) {
			keyEvent* pEvent = new keyEvent();
			pEvent->key = key.first;
			pEvent->timeStamp = GetCurrentTimeStamp();

			std::lock_guard<std::mutex> lock(eventMutex);

			events.push(pEvent);
		}

		key.second = isPressed;
	}
}

bool GetKeyEvent(keyEvent* pKeyEvent)
{
	std::lock_guard<std::mutex> lock(eventMutex);
	
	if (events.size() > 0) {
		OutputDebugString("Events!!");

		keyEvent* pEvent = events.front();
		events.pop();

		pKeyEvent->key = pEvent->key;
		pKeyEvent->timeStamp = pEvent->timeStamp;

		delete pEvent;

		return true;
	}
	else {
		OutputDebugString("No events");
		return false;
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
	else {
		OutputDebugString("TimeSetEvent succeeded");
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

	keyStates.clear();
}
