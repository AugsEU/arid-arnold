
namespace AridArnold
{
	internal class ESpeedRunOption : YesNoOption
	{
		int REQ_WORLD_REACHED = 3;

		public ESpeedRunOption(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			SetSelectedOptionsIdx(1);//Default no
			mBlockedOut = SaveManager.I.GetGlobalSaveInfo().GetWorldReachedList().Count < REQ_WORLD_REACHED;
			CampaignManager.I.SetSpeedrunMode(false);
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
