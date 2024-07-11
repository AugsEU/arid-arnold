namespace AridArnold
{
	class DeathRideCabinet : ArcadeCabinet
	{
		static Rect2f DEATH_SCREEN_SPACE = new Rect2f(new Vector2(0.0f, 28.0f), 960.0f, 512.0f);
		Texture2D mBGTexture;

		public DeathRideCabinet(GraphicsDeviceManager deviceManager, ContentManager content) : base(new DeathRide.DeathRide(deviceManager, content), DEATH_SCREEN_SPACE)
		{
			mBGTexture = MonoData.I.MonoGameLoad<Texture2D>("Arcade/Cabinets/DeathRide");

			AddHighScore(20, "AJD");
			AddHighScore(40, "ASS");
			AddHighScore(80, "PIX");
			AddHighScore(100, "ACE");
			AddHighScore(110, "BAR");
			AddHighScore(140, "DOK");
			AddHighScore(200, "JCD");
			AddHighScore(220, "CJM");
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

			textPos.Y += 50.0f;
			MonoDraw.DrawStringCentred(info, bigFont, textPos, new Color(196, 0, 0), deathTitle, DrawLayer.Background);

			textPos.Y += 250.0f;
			MonoDraw.DrawStringCentred(info, font, textPos, new Color(91, 19, 0), insertCoin, DrawLayer.Background);
		}
	}
}
