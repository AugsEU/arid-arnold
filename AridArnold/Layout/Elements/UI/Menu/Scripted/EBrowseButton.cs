
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AridArnold
{
	internal class EBrowseButton : MenuButton
	{
		public EBrowseButton(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
		}

		public override void DoAction()
		{
			string baseDirectory = Path.Join("data", ProfileSaveInfo.PROFILE_SAVE_FOLDER);
			if (!Directory.Exists(baseDirectory))
			{
				return;
			}

			string fullPath = Path.GetFullPath(baseDirectory);

			try
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					string explorerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");
					if (!File.Exists(explorerPath))
					{
						throw new FileNotFoundException("Explorer.exe not found.");
					}

					Process.Start("explorer.exe", fullPath);
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					// Use an alternative for Linux, such as xdg-open.
					Process.Start("xdg-open", fullPath);
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				{
					// Use open command for macOS.
					Process.Start("open", fullPath);
				}

				// Need to exit game.
				Main.ExitGame();
			}
			catch (Exception ex)
			{
				//The system cannot find the file specified...
				Console.WriteLine(ex.Message);
			}
		}
	}
}
