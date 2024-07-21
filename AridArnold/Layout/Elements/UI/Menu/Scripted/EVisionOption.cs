
namespace AridArnold
{
	/// <summary>
	/// Button that exits the game when clicked
	/// </summary>
	internal class EVisionOption : StringOption
	{
		static string[] VISION_OPTIONS = { "UI.Menu.VisionOptionPerfect", "UI.Menu.VisionOptionSretch" };

		public EVisionOption(XmlNode rootNode, Layout parent) : base(rootNode, parent, VISION_OPTIONS)
		{
		}

		protected override void SyncOption()
		{
			int setIdx = (int)OptionsManager.I.GetVision();
			SetSelectedOptionsIdx(setIdx);
		}

		protected override void OnOptionSelect(int optionIdx)
		{
			VisionOption visionOption = VisionOption.kPerfect;
			switch (optionIdx)
			{
				case 0:
					visionOption = VisionOption.kPerfect;
					break;
				case 1:
					visionOption = VisionOption.kStretch;
					break;
				default:
					break;
			}
			OptionsManager.I.SetVision(visionOption);
		}
	}
}
