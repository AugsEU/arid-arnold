
namespace AridArnold
{
	internal class ESpeedRunOption : YesNoOption
	{
		public ESpeedRunOption(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			SetSelectedOptionsIdx(1);//Default no
		}

		protected override void SyncOption()
		{
			// Don't sync
		}

		protected override void OnOptionYesNo(bool isYes)
		{
			CampaignManager.I.SetSpeedrunMode(isYes);
		}
	}
}
