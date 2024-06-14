namespace AridArnold
{
	class ESatelliteGPS : UIPanelBase
	{
		static Vector2 WORLD_TITLE_OFFSET = new Vector2(95.0f, 159.0f);
		

		SpriteFont mFont;


		public ESatelliteGPS(XmlNode rootNode) : base(rootNode, "UI/InGame/SatelliteBG")
		{
			mFont = FontManager.I.GetFont("Pixica", 24, true);
		}

		public override void Draw(DrawInfo info)
		{
			base.Draw(info);

			DrawWorldTitle(info);
		}

		public void DrawWorldTitle(DrawInfo info)
		{
			Level currLevel = CampaignManager.I.GetCurrentLevel();

			if (currLevel is not null)
			{
				string worldName = currLevel.GetTheme().GetDisplayName();
				MonoDraw.DrawStringCentred(info, mFont, GetPosition() + WORLD_TITLE_OFFSET, PANEL_GOLD, worldName, DrawLayer.Front);
			}
		}
	}
}
