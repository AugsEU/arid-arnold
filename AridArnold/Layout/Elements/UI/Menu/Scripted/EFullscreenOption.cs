
namespace AridArnold
{
	internal class EFullscreenOption : YesNoOption
	{
		public EFullscreenOption(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
		}

		protected override string GetDescriptionStrID(int optionIdx)
		{
			return "UI.Menu.FullscreenOptionDesc";
		}

		protected override void OnOptionYesNo(bool isYes)
		{
			Main.SetFullScreen(isYes);
		}

		protected override void SyncOption()
		{
			SyncFromBool(Main.IsFullScreen());
		}
	}
}
