﻿namespace AridArnold
{
	internal class WormWarpCabinet : ArcadeCabinet
	{
		static Rect2f WORM_SCREEN_SPACE = new Rect2f(new Vector2(160.0f, 79.0f), 640.0f, 360.0f);
		Texture2D mBGTexture;
		
		public WormWarpCabinet(GraphicsDeviceManager deviceManager, ContentManager content) : base(new WormWarp.SnakeGameArcade(deviceManager, content), WORM_SCREEN_SPACE)
		{
			mBGTexture = MonoData.I.MonoGameLoad<Texture2D>("Arcade/Cabinets/WormWarp");

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
			SpriteFont bigFont = FontManager.I.GetFont("Pixica-32");
			SpriteFont font = FontManager.I.GetFont("Pixica-24");
			Vector2 textPos = WORM_SCREEN_SPACE.min;
			textPos.X += WORM_SCREEN_SPACE.Width * 0.5f;

			string wormTitle = LanguageManager.I.GetText("Arcade.WormWarp.Title");
			string insertCoin = LanguageManager.I.GetText("Arcade.InsertCoin");

			textPos.Y += 50.0f;
			MonoDraw.DrawStringCentred(info, bigFont, textPos, new Color(255,125,147), wormTitle, DrawLayer.Background);

			textPos.Y += 150.0f;
			MonoDraw.DrawStringCentred(info, font, textPos, new Color(71, 116, 237), insertCoin, DrawLayer.Background);
		}
	}
}