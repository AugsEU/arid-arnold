
using System.Xml.Linq;

namespace AridArnold
{
	internal class ELevelGoal : LayElement
	{
		SpriteFont mFont;

		public ELevelGoal(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			mFont = FontManager.I.GetFont("Pixica-12");
		}

		public override void Draw(DrawInfo info)
		{
			Level currLevel = CampaignManager.I.GetCurrentLevel();

			if (currLevel is null)
			{
				return;
			}

			string goalStr = "";

			switch (currLevel.GetAuxData().GetLevelType())
			{
				case AuxData.LevelType.CollectWater:
					goalStr = "Collect all water bottles";
					break;
				case AuxData.LevelType.CollectKey:
					goalStr = "Collect the key";
					break;
				case AuxData.LevelType.Shop:
					goalStr = "Go Shopping";
					break;
				default:
					goalStr = "ERROR";
					break;
			}

			Vector2 pos = GetPosition();
			MonoDraw.DrawStringCentred(info, mFont, pos, GetColor(), "GOAL:", GetDepth());
			pos.Y += 15.0f;
			MonoDraw.DrawStringCentred(info, mFont, pos, GetColor(), goalStr, GetDepth());
		}
	}
}
