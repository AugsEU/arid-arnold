
namespace AridArnold
{
	/// <summary>
	/// Button that exits the game when clicked
	/// </summary>
	internal class EExitButton : MenuButton
	{
		public EExitButton(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
		}

		public override void DoAction()
		{
			Main.ExitGame();
		}
	}
}
