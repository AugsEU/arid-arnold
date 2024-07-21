
using System.Reflection.PortableExecutable;

namespace AridArnold
{
	/// <summary>
	/// Button that sends a message when clicked.
	/// </summary>
	internal class EMessageButton : MenuButton
	{
		string mHeader;
		string mMessage;

		public EMessageButton(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			string rawStr = MonoParse.GetString(rootNode["msg"], "");
			string[] msgComponents = rawStr.Split(':');
			
			MonoDebug.Assert(rawStr.Length > 0 && msgComponents.Length == 2, "Invalid message");

			mHeader = msgComponents[0];
			mMessage = msgComponents[1];
		}

		public override void DoAction()
		{
			GetParent().QueueMessage(this, mHeader, mMessage);
		}
	}
}
