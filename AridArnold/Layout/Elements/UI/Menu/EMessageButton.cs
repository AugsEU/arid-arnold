
namespace AridArnold
{
	/// <summary>
	/// Button that sends a message when clicked.
	/// </summary>
	internal class EMessageButton : MenuButton
	{
		string mMessage;

		public EMessageButton(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			mMessage = MonoParse.GetString(rootNode["msg"], "");
		}

		public override void DoAction()
		{
			GetParent().QueueMessage(this, mMessage);
		}
	}
}
