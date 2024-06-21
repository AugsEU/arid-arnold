using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace AridArnold
{
	public static class Program
	{
		[STAThread]
		static void Main()
		{
			// Set these for safety
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			using (var game = new Main())
				game.Run();
		}
	}
}
