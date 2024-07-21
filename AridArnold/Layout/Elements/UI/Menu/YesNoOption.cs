
namespace AridArnold
{
	abstract class YesNoOption : StringOption
	{
		static string[] YES_NO_OPTIONS = { "UI.Menu.Yes", "UI.Menu.No" };

		public YesNoOption(XmlNode rootNode, Layout parent) : base(rootNode, parent, YES_NO_OPTIONS)
		{
		}

		protected void SyncFromBool(bool isYes)
		{
			SetSelectedOptionsIdx(isYes ? 0 : 1);
		}

		protected override void OnOptionSelect(int optionIdx)
		{
			OnOptionYesNo(optionIdx == 0);
		}

		protected abstract void OnOptionYesNo(bool isYes);
	}
}
