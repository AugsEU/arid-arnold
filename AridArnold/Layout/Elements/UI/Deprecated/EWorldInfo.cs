
namespace AridArnold
{
	internal class EWorldInfo : LayElement
	{
		SpriteFont mFont;
		SpriteFont mSmallFont;

		public EWorldInfo(XmlNode rootNode) : base(rootNode)
		{
			mFont = FontManager.I.GetFont("Pixica-24");
			mSmallFont = FontManager.I.GetFont("Pixica-12");
		}

		public override void Draw(DrawInfo info)
		{
			Level currLevel = CampaignManager.I.GetCurrentLevel();

			if (currLevel is not null)
			{
				string worldName = currLevel.GetTheme().GetDisplayName();
				MonoDraw.DrawStringCentred(info, mFont, GetPosition(), GetColor(), worldName, DrawLayer.Front);

				if (currLevel.GetAuxData().GetLevelType() != AuxData.LevelType.Hub)
				{
					Vector2 subPos = GetPosition();
					subPos.Y += 13.0f;
					MonoDraw.DrawStringCentred(info, mSmallFont, subPos, GetColor(), currLevel.GetID().ToString("0000"), DrawLayer.Front);
				}
			}

			base.Draw(info);
		}
	}
}
