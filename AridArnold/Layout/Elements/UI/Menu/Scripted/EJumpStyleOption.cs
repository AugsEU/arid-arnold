
namespace AridArnold
{
	internal class EJumpStyleOption : YesNoOption
	{
		static string[] JUMP_STYLE_OPTIONS = { "UI.Menu.JumpStyleHold", "UI.Menu.JumpStylePress" };

		public EJumpStyleOption(XmlNode rootNode, Layout parent) : base(rootNode, parent, JUMP_STYLE_OPTIONS)
		{
		}

		protected override void OnOptionYesNo(bool isYes)
		{
			OptionsManager.I.SetHoldJump(isYes);
		}

		protected override void SyncOption()
		{
			SyncFromBool(OptionsManager.I.GetHoldJump());
		}

		protected override string GetDescriptionStrID(int optionIdx)
		{
			return optionIdx == 0 ? "UI.Menu.JumpStyleHoldDesc" : "UI.Menu.JumpStylePressDesc";
		}
	}
}
