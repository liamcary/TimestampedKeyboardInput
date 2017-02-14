#pragma once

#ifdef INPUT_EXPORTS
#define INPUT_EXPORTS __declspec(dllexport)
#else
#define INPUT_EXPORTS __declspec(dllimport)
#endif

#include <Windows.h>

typedef void(*KeyDownCallback)(int);

struct keyEvent
{
public:
	int key;
	double timeStamp;
};

extern "C"
{
	INPUT_EXPORTS double GetCurrentTimeStamp();

	INPUT_EXPORTS bool GetKeyEvent(keyEvent* pKeyEvent);

	INPUT_EXPORTS void RegisterInput(int keycode);

	INPUT_EXPORTS void StartPolling();

	INPUT_EXPORTS void StopPolling();
}