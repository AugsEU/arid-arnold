using System.Diagnostics;

namespace AridArnold
{
	static class MonoDebug
	{
		public static bool mDebugOn = false;

		/// <summary>
		/// Log message to console. Only if debug is on.
		/// </summary>
		/// <param name="msg">Message to log</param>
		public static void Log(string msg)
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
		public static void DLog(string msg)
		{
			Debug.WriteLine(msg);
		}
	}
}
