
namespace AridArnold
{
	/// <summary>
	/// Button that exits the game when clicked
	/// </summary>
	internal class EResetControlsButton : MenuButton
	{
		public EResetControlsButton(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
		}

		public override void DoAction()
		{
			InputManager.I.SetDefaultBindings();
		}
	}
}
