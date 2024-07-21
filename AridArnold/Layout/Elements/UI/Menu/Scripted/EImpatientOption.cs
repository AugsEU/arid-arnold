
namespace AridArnold
{
	internal class EImpatientOption : YesNoOption
	{
		public EImpatientOption(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
		}

		protected override void OnOptionYesNo(bool isYes)
		{
			OptionsManager.I.SetImpatientPlayer(isYes);
		}

		protected override void SyncOption()
		{
			SyncFromBool(OptionsManager.I.GetImpatientPlayer());
		}
	}
}
