
using System.ComponentModel;
using System.Diagnostics;

namespace AridArnold
{
	internal class EBrowseButton : MenuButton
	{
		public EBrowseButton(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
		}

		public override void DoAction()
		{
			string baseDirectory = Path.Join("data/", ProfileSaveInfo.PROFILE_SAVE_FOLDER);
			if (!Directory.Exists(baseDirectory))
			{
				return;
			}

			string fullPath = Path.GetFullPath(baseDirectory);

			try
			{
				Process.Start("explorer.exe", fullPath);
				
				// Need to exit game.
				Main.ExitGame();
			}
			catch (Win32Exception win32Exception)
			{
				//The system cannot find the file specified...
				Console.WriteLine(win32Exception.Message);
			}
		}
	}
}
