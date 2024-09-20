namespace AridArnold
{
	class DeathRideCabinet : ArcadeCabinet
	{
		static Rect2f DEATH_SCREEN_SPACE = new Rect2f(new Vector2(0.0f, 28.0f), 960.0f, 512.0f);
		Texture2D mBGTexture;

		public DeathRideCabinet(GraphicsDeviceManager deviceManager, ContentManager content) : base(new DeathRide.DeathRide(deviceManager, content), DEATH_SCREEN_SPACE)
		{
			mBGTexture = MonoData.I.MonoGameLoad<Texture2D>("Arcade/Cabinets/DeathRide");
		}

		protected override void SetDefaultScores()
		{
			
			AddHighScore(60000, "AJD");
			AddHighScore(56855, "PIX");
			AddHighScore(45635, "ACE");
			AddHighScore(25325, "BAR");
			AddHighScore(6170, "DOK");
			AddHighScore(6020, "JCD");
			AddHighScore(270, "CJM");
			AddHighScore(210, "ASS");
		}

		protected override void DrawCabinetBG(DrawInfo info)
		{
			MonoDraw.DrawTextureDepth(info, mBGTexture, Vector2.Zero, DrawLayer.Front);
		}

		protected override void DrawTitleScreen(DrawInfo info)
		{
			SpriteFont bigFont = FontManager.I.GetFont("Pixica", 36);
			SpriteFont font = FontManager.I.GetFont("Pixica", 24);
			Vector2 textPos = DEATH_SCREEN_SPACE.min;
			textPos.X += DEATH_SCREEN_SPACE.Width * 0.5f;

			string deathTitle = LanguageManager.I.GetText("Arcade.DeathRide.Title");
			string insertCoin = LanguageManager.I.GetText("Arcade.InsertCoin");
			string exitCab = LanguageManager.I.GetText("Arcade.QuitGame");

			textPos.Y += 65.0f;
			MonoDraw.DrawStringCentred(info, bigFont, textPos, new Color(196, 0, 0), deathTitle, DrawLayer.Background);

			textPos.Y += 80.0f;
			DrawScoreList(info, textPos.Y);

			textPos.Y += 250.0f;
			MonoDraw.DrawStringCentred(info, font, textPos, new Color(91, 19, 0), insertCoin, DrawLayer.Background);

			textPos.Y += 40.0f;
			MonoDraw.DrawStringCentred(info, font, textPos, new Color(91, 19, 0), exitCab, DrawLayer.Background);
		}
	}
}
