
namespace AridArnold
{
	internal class EMusicVolOption : PercentageOption
	{
		public EMusicVolOption(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
		}

		protected override void SyncOption()
		{
			SyncFromFloat(OptionsManager.I.GetMusicVolume());
		}

		protected override void OnOptionSelect(int optionIdx)
		{
			OptionsManager.I.SetMusicVolume(GetPercentage());
		}
	}
}
