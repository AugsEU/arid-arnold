
namespace AridArnold
{
	internal class EFastTextOption : YesNoOption
	{
		static string[] FAST_NORMAL_OPTIONS = { "UI.Menu.FastText", "UI.Menu.NormalText" };

		public EFastTextOption(XmlNode rootNode, Layout parent) : base(rootNode, parent, FAST_NORMAL_OPTIONS)
		{
		}

		protected override void OnOptionYesNo(bool isYes)
		{
			OptionsManager.I.SetFastText(isYes);
		}

		protected override void SyncOption()
		{
			SyncFromBool(OptionsManager.I.GetFastText());
		}

		protected override string GetDescriptionStrID(int optionIdx)
		{
			return "UI.Menu.TextSpeedOptionDesc";
		}
	}
}
