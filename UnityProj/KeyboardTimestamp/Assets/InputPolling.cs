using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

public class InputPolling : MonoBehaviour
{
	int VK_A = 0x41;
	int VK_B = 0x42;
	int VK_C = 0x43;

	[StructLayout (LayoutKind.Sequential)]
	struct KeyEvent
	{
		public int Key;
		public double TimeStamp;
	}

	[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
	internal delegate void KeyDownCallback (int val);

	[DllImport ("InputPlugin", EntryPoint = "GetCurrentTimeStamp", CallingConvention = CallingConvention.Cdecl)]
	static extern double GetCurrentTimeStamp ();

	[DllImport ("InputPlugin", EntryPoint = "GetKeyEvent", CallingConvention = CallingConvention.Cdecl)]
	static extern bool GetKeyEvent ([MarshalAs(UnmanagedType.Struct)] ref KeyEvent keyEvent);

	[DllImport ("InputPlugin", EntryPoint = "StartPolling", CallingConvention = CallingConvention.Cdecl)]
	static extern void StartPolling ();

	[DllImport ("InputPlugin", EntryPoint = "StopPolling", CallingConvention = CallingConvention.Cdecl)]
	static extern void StopPolling ();

	[DllImport ("InputPlugin", EntryPoint = "RegisterInput", CallingConvention = CallingConvention.Cdecl)]
	static extern void RegisterInput (int keyCode);

	double lastFrameTime;

	void Start ()
	{
		RegisterInput (VK_A);
		RegisterInput (VK_B);
		RegisterInput (VK_C);

		Debug.Log ("Starting polling");

		StartPolling ();
	}

	void OnDestroy ()
	{
		Debug.Log ("Stopping polling");

		StopPolling ();
	}

	void Update ()
	{
		double frameTime = GetCurrentTimeStamp ();

		KeyEvent ke = new KeyEvent ();

		bool frameLogged = false;

		while (GetKeyEvent (ref ke)) {
			if (!frameLogged) {
				Debug.LogFormat ("Frame time: {0:F5}, Last Frame Time: {1:F5}", frameTime, lastFrameTime);
				frameLogged = true;
			}

			Debug.LogFormat ("Key {0:X2} Time: {1:F5}, Percent: {2:F3}",
				ke.Key, ke.TimeStamp, (ke.TimeStamp - lastFrameTime) / (frameTime - lastFrameTime));
		}

		lastFrameTime = frameTime;
	}
}
