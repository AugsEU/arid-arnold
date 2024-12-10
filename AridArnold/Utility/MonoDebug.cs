using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AridArnold
{
	static class MonoDebug
	{
		struct DebugRect
		{
			public Rectangle mRectangle;
			public Color mColor;
		}

		private static List<DebugRect> mDebugRectToDraw = new List<DebugRect>();
		public static bool mConsoleAlloc = false;
		public static bool mDebugFlag1 = false;
		static uint mLogLineNum = 0;

		/// <summary>
		/// Log message to console. Only if debug is on.
		/// </summary>
		/// <param name="msg">Message to log</param>
		public static void Log(string msg, params object[] args)
		{
#if DEBUG
#if WINDOWS_OFF
			if(!mConsoleAlloc)
			{
				AllocConsole();
				mConsoleAlloc = true;
			}
			mLogLineNum++;
			string format = string.Format("[{0}]: {1}", mLogLineNum.ToString("X4"), msg);
			Console.WriteLine(format, args);
#endif // WINDOWS
#endif
		}

#if WINDOWS_OFF
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool AllocConsole();
#endif // WINDOWS


		public static void Break(string msg = "", params object[] args)
		{
			if (msg != "")
			{
				Log(msg, args);
			}
			System.Diagnostics.Debugger.Break();
		}


		public static void Assert(bool condition, string msg = "", params object[] args)
		{
#if DEBUG
			if (!condition)
			{
				Log(msg, args);
				Break("Assertion failed.");
			}
#else
			if (!condition)
			{
				string errorMsg = string.Format(msg, args);
				throw new Exception(errorMsg);
			}
#endif
		}

		public static void AddDebugRect(Rectangle rect, Color color)
		{
#if DEBUG
			DebugRect debugRect;
			debugRect.mRectangle = rect;
			debugRect.mColor = color;

			mDebugRectToDraw.Add(debugRect);
#endif
		}


		public static void AddDebugRect(Rect2f rect, Color color)
		{
#if DEBUG
			DebugRect debugRect;
			debugRect.mRectangle = rect.ToRectangle();
			debugRect.mColor = color;

			mDebugRectToDraw.Add(debugRect);
#endif
		}

		public static void AddDebugPoint(Vector2 pos, Color color)
		{
#if DEBUG
			AddDebugRect(new Rect2f(pos, 2.0f, 2.0f), color);
#endif
		}

		public static void DrawDebugRects(DrawInfo info)
		{
#if DEBUG
			foreach(DebugRect debugRect in mDebugRectToDraw)
			{
				MonoDraw.DrawRectDepth(info, debugRect.mRectangle, debugRect.mColor, DrawLayer.Front);
			}
			mDebugRectToDraw.Clear();
#endif
		}
	}
}
