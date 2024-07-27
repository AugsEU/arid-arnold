
namespace AridArnold
{
	internal class EMasterVolOption : PercentageOption
	{
		public EMasterVolOption(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
		}

		protected override void SyncOption()
		{
			SyncFromFloat(OptionsManager.I.GetMasterVolume());
		}

		protected override void OnOptionSelect(int optionIdx)
		{
			OptionsManager.I.SetMasterVolume(GetPercentage());
		}

		protected override string GetDescriptionStrID(int optionIdx)
		{
			return "UI.Menu.EarPowerOptionDesc";
		}
	}
}
