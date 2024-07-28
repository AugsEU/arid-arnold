
using System.Reflection.PortableExecutable;

namespace AridArnold
{
	internal class ELoadGameButton : EMessageButton
	{
		public ELoadGameButton(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			mBlockedOut = SaveManager.I.GetSaveFileList().Count == 0;
		}
	}
}
