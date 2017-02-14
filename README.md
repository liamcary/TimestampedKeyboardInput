# TimestampedKeyboardInput
Manually polling keyboard input at a high frequency for input timestamps

This was created as a quick Unity3D example of one way to read input with precise timestamps for when the button was pressed. (excuse the messy commits)
It uses the windows multimedia timer in an unmanaged DLL with a 1ms resolution. This will have significantly more overhead than the standard Input library, but is still pretty quick. Most of the overhead is in thread switching.

Usage:
Run the "game" in the editor. Pressing the A, B or C keys will trigger log messages. The first log message logs the time of the current frame and the time of the last frame. The second log messages logs the time recorded for the KeyDown event, and a percentage indicating how far between the previous frame and current frame it was.

Note: You shouldn't act on the input in the multimedia timer callback, just record the time. Too much processing in the callback will break things horrifically.

Note2: There's a problem with unity's mono and cleaning up threads in managed DLLs that have callbacks to managed code. You can start and end the game fine, but the editor will hang when you close the editor program. 
For more info: https://forum.unity3d.com/threads/problem-with-callbacks.87513/
A workaround could be to cache the key down events in the managed DLL and just fetch them using an exported method. This could be more efficient too. (THIS WORKAROUND IS DONE IN LATEST UPDATE)

Note3: With the change that removes the use of native code callbacks to managed code, it is possible that events will be added in the background thread after the unity main thread has started processing them one at a time. This means that sometimes the percentage value will be > 1. 

Note4: Also worth noting that this was a rushed example and likely contains some thread-safety issues and possibly memory leaks. 

Note5: The native plugin uses the clock() method which has millisecond resolution, which now makes it easier to hit two keys "at the same time". A higher resolution clock could give more precision, but I don't know the various time APIs available in c++ off the top of my head.