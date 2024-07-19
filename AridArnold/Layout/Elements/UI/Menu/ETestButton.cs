
namespace AridArnold
{
	/// <summary>
	/// Button with no behaviour
	/// </summary>
	internal class ETestButton : MenuButton
	{
		public ETestButton(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
		}

		public override void DoAction()
		{
		}
	}
}
