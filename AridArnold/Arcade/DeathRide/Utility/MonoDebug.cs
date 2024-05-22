using System.Diagnostics;

namespace GMTK2023
{
	static class MonoDebug
	{
		public static bool mDebugOn = false;

		/// <summary>
		/// Log message to console. Only if debug is on.
		/// </summary>
		/// <param name="msg">Message to log</param>
		public static void Log(string msg, params object[] args)
		{
			if (mDebugOn)
			{
				Debug.WriteLine(msg);
			}
		}



		/// <summary>
		/// Log a message to console.
		/// </summary>
		/// <param name="msg">Message to log</param>
		public static void DLog(string msg, params object[] args)
		{
			Debug.WriteLine(msg, args);
		}


		public static void Break(string msg = "", params object[] args)
		{
			if (msg != "")
			{
				DLog(msg, args);
			}
			System.Diagnostics.Debugger.Break();
		}


		public static void Assert(bool condition)
		{
#if DEBUG
			if (!condition)
			{
				Break("Assertion failed.");
			}
#endif
		}
	}
}
