namespace AridArnold
{
	class HorsesAndGunCabinet : ArcadeCabinet
	{
		static Rect2f HORSE_SCREEN_SPACE = new Rect2f(new Vector2(160.0f, 70.0f), 640.0f, 360.0f);
		Texture2D mBGTexture;

		public HorsesAndGunCabinet(GraphicsDeviceManager deviceManager, ContentManager content) : base(new HorsesAndGun.HorsesAndGun(deviceManager, content), HORSE_SCREEN_SPACE)
		{
			mBGTexture = MonoData.I.MonoGameLoad<Texture2D>("Arcade/Cabinets/HorsesAndGun");
		}

		protected override void SetDefaultScores()
		{
			AddHighScore(1800, "AJD");
			AddHighScore(990, "COW");
			AddHighScore(475, "ARN");
			AddHighScore(465, "ACE");
			AddHighScore(315, "ZIP");
			AddHighScore(120, "DOK");
			AddHighScore(110, "BOY");
			AddHighScore(55, "YOU");
		}

		protected override void DrawCabinetBG(DrawInfo info)
		{
			MonoDraw.DrawTextureDepth(info, mBGTexture, Vector2.Zero, DrawLayer.Front);
		}

		protected override void DrawTitleScreen(DrawInfo info)
		{
			SpriteFont bigFont = FontManager.I.GetFont("Pixica-36");
			SpriteFont font = FontManager.I.GetFont("Pixica-24");
			Vector2 textPos = HORSE_SCREEN_SPACE.min;
			textPos.X += HORSE_SCREEN_SPACE.Width * 0.5f;

			string deathTitle = LanguageManager.I.GetText("Arcade.HorsesAndGun.Title");
			string insertCoin = LanguageManager.I.GetText("Arcade.InsertCoin");

			textPos.Y += 50.0f;
			MonoDraw.DrawStringCentred(info, bigFont, textPos, new Color(255, 209, 112), deathTitle, DrawLayer.Background);

			textPos.Y += 60.0f;
			DrawScoreList(info, textPos.Y);

			textPos.Y += 190.0f;
			MonoDraw.DrawStringCentred(info, font, textPos, new Color(111, 65, 58), insertCoin, DrawLayer.Background);
		}
	}
}
