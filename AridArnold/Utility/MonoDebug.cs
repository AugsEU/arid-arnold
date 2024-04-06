using System.Diagnostics;

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
		public static bool mDebugFlag1 = false;

		/// <summary>
		/// Log message to console. Only if debug is on.
		/// </summary>
		/// <param name="msg">Message to log</param>
		public static void Log(string msg, params object[] args)
		{
#if DEBUG
			Debug.WriteLine(msg, args);
#endif
		}


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
