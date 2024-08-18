namespace DeathRide
{
	internal class GameOverScreen : Screen
	{
		Texture2D mBG;

		public GameOverScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
		}

		public override void LoadContent()
		{
			mBG = MonoData.I.MonoGameLoad<Texture2D>("Backgrounds/GameOver");
			base.LoadContent();
		}

		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			SpriteFont pixelFont = AridArnold.FontManager.I.GetFont("Pixica", 24);

			Vector2 centre = new Vector2(mScreenTarget.Width / 2, mScreenTarget.Height / 2);

			//Draw out the game area
			info.device.SetRenderTarget(mScreenTarget);
			info.device.Clear(new Color(0, 0, 0));

			StartScreenSpriteBatch(info);

			MonoDraw.DrawTexture(info, mBG, Vector2.Zero);

			Color textCol = new Color(255, 219, 162);

			centre.Y += 50.0f;
			MonoDraw.DrawShadowStringCentred(info, pixelFont, centre, textCol, "Rounds: " + RunManager.I.GetRounds());
			centre.Y += 50.0f;
			MonoDraw.DrawShadowStringCentred(info, pixelFont, centre, textCol, "Score: " + RunManager.I.GetScore());

			EndScreenSpriteBatch(info);

			return mScreenTarget;
		}

		public override void Update(GameTime gameTime)
		{
			if(AridArnold.InputManager.I.KeyPressed(AridArnold.InputAction.Confirm))
			{
				RunManager.I.RequestExit();
			}
		}
	}
}
