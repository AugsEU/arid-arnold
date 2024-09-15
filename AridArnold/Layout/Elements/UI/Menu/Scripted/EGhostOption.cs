
namespace AridArnold
{
	internal class EGhostOption : YesNoOption
	{
		public EGhostOption(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
		}

		protected override void OnOptionYesNo(bool isYes)
		{
			OptionsManager.I.SetGhostDisplay(isYes);
		}

		protected override void SyncOption()
		{
			SyncFromBool(OptionsManager.I.GetGhostDisplay());
		}

		protected override string GetDescriptionStrID(int optionIdx)
		{
			return "UI.Menu.GhostOptionDesc";
		}
	}
}
