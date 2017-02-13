using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class InputPolling : MonoBehaviour
{
	int VK_A = 0x41;
	int VK_B = 0x42;
	int VK_C = 0x43;

	[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
	internal delegate void KeyDownCallback (int val);

	[DllImport ("InputPlugin", EntryPoint = "StartPolling", CallingConvention = CallingConvention.Cdecl)]
	static extern void StartPolling ();

	[DllImport ("InputPlugin", EntryPoint = "StopPolling", CallingConvention = CallingConvention.Cdecl)]
	static extern void StopPolling ();

	[DllImport ("InputPlugin", EntryPoint = "RegisterInput", CallingConvention = CallingConvention.Cdecl)]
	static extern void RegisterInput (int keyCode);

	[DllImport ("InputPlugin", EntryPoint = "SetKeyDownCallback", CallingConvention = CallingConvention.Cdecl)]
	static extern void SetKeyDownCallback (KeyDownCallback callback);

	readonly object callbackLock = new object ();
	KeyDownCallback keyDownCallback;

	public class KeyDownEvent
	{
		public int KeyCode;
		public DateTime TimeStamp;
	}

	readonly Queue<KeyDownEvent> eventsQueue = new Queue<KeyDownEvent> ();

	DateTime lastFrameTime;

	void Start ()
	{
		keyDownCallback = OnKeyDown;

		SetKeyDownCallback (keyDownCallback);

		RegisterInput (VK_A);
		RegisterInput (VK_B);
		RegisterInput (VK_C);

		Debug.Log ("Starting polling");

		StartPolling ();
	}

	void OnApplicationQuit ()
	{
		keyDownCallback = null;

		Debug.Log ("Quitting");

		lock (callbackLock) {
			StopPolling ();
		}
	}

	void OnDestroy ()
	{
		keyDownCallback = null;
		Debug.Log ("Stopping polling");

		lock (callbackLock) {
			StopPolling ();
		}
	}

	void Update ()
	{
		int eventCount = 0;
		lock (callbackLock) {
			eventCount = eventsQueue.Count;
		}

		if (eventCount > 0) {
			Debug.LogFormat ("Frame time: {0}, Last Frame Time: {1}", 
				DateTime.Now.ToString("mm:ss.ffffff"), 
				lastFrameTime.ToString("mm:ss.ffffff"));
		}

		while (eventCount > 0) {
			KeyDownEvent keyDownEvent = null;

			lock (callbackLock) {
				keyDownEvent = eventsQueue.Dequeue ();
				eventCount = eventsQueue.Count;
			}

			if (keyDownEvent == null) {
				continue;
			}

			Debug.LogFormat ("Key {0} Time: {1}, Percent: {2}", 
				keyDownEvent.KeyCode.ToString ("X2"), 
				keyDownEvent.TimeStamp.ToString ("mm:ss.ffffff"),
				((keyDownEvent.TimeStamp - lastFrameTime).TotalMilliseconds / (DateTime.Now - lastFrameTime).TotalMilliseconds).ToString ("0.00"));
		}

		lastFrameTime = DateTime.Now;
	}

	void OnKeyDown (int keyCode)
	{
		lock (callbackLock) {
			eventsQueue.Enqueue (new KeyDownEvent {
				KeyCode = keyCode,
				TimeStamp = DateTime.Now
			});
		}
	}
}
