
namespace AridArnold
{
	internal class EWorldInfo : LayElement
	{
		SpriteFont mFont;

		public EWorldInfo(XmlNode rootNode) : base(rootNode)
		{
			mFont = FontManager.I.GetFont("Pixica-24");
		}

		public override void Draw(DrawInfo info)
		{
			Level currLevel = CampaignManager.I.GetCurrentLevel();

			if(currLevel is not null)
			{
				string worldName = currLevel.GetTheme().GetDisplayName();
				MonoDraw.DrawStringCentred(info, mFont, GetPosition(), GetColor(), worldName, DrawLayer.Front);
			}

			base.Draw(info);
		}
	}
}
