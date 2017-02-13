#pragma once

#ifdef INPUT_EXPORTS
#define INPUT_EXPORTS __declspec(dllexport)
#else
#define INPUT_EXPORTS __declspec(dllimport)
#endif

#include <Windows.h>

typedef void(*KeyDownCallback)(int);

extern "C"
{
	INPUT_EXPORTS void RegisterInput(int keycode);

	INPUT_EXPORTS void StartPolling();

	INPUT_EXPORTS void StopPolling();

	INPUT_EXPORTS void SetKeyDownCallback(KeyDownCallback callback);
}