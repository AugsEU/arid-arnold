namespace HorsesAndGun
{
	internal class IntroScreen : Screen
	{
		const double FADE_IN_TIME = 2000.0;

		MonoTimer mFadeInTimer;



		public IntroScreen(ContentManager content, GraphicsDeviceManager graphics) : base(content, graphics)
		{
			mFadeInTimer = new MonoTimer();
		}

		public override void LoadContent(ContentManager content)
		{

		}

		public override void OnActivate()
		{
			mFadeInTimer.Reset();
			mFadeInTimer.Start();
		}

		public override void OnDeactivate()
		{
		}

		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			SpriteFont pixelFont = FontManager.I.GetFont("Pixica-24");

			Vector2 centre = new Vector2(mScreenTarget.Width / 2, mScreenTarget.Height / 2);

			//Draw out the game area
			info.device.SetRenderTarget(mScreenTarget);
			info.device.Clear(Color.SaddleBrown);

			info.spriteBatch.Begin(SpriteSortMode.Immediate,
									BlendState.Additive,
									SamplerState.PointClamp,
									DepthStencilState.None,
									RasterizerState.CullNone);

			Color color = Color.White;
			double alpha = 1.0f;

			if (mFadeInTimer.GetElapsedMs() < FADE_IN_TIME)
			{
				alpha = mFadeInTimer.GetElapsedMs() / FADE_IN_TIME;
			}
			else if (mFadeInTimer.GetElapsedMs() < 2.0f * FADE_IN_TIME)
			{
				alpha = 1.0;
			}
			else if (mFadeInTimer.GetElapsedMs() < 3.0f * FADE_IN_TIME)
			{
				alpha = 1.0 - (mFadeInTimer.GetElapsedMs() - 2.0 * FADE_IN_TIME) / FADE_IN_TIME;
			}
			else
			{
				alpha = 0.0;
			}

			color.A = (byte)(255 * alpha);

			Util.DrawStringCentred(info.spriteBatch, pixelFont, centre, color, "Welcome to horses and gun.");

			info.spriteBatch.End();

			return mScreenTarget;
		}

		public override void Update(GameTime gameTime)
		{
			Keys[] nextKeys = { Keys.Enter, Keys.Space };

			if (InputManager.I.AnyKeysPressed(nextKeys) || InputManager.I.LClick())
			{
				ScreenManager.I.ActivateScreen(ScreenType.MainMenu);
			}

			if (mFadeInTimer.GetElapsedMs() > 3.5 * FADE_IN_TIME)
			{
				ScreenManager.I.ActivateScreen(ScreenType.MainMenu);
			}
		}
	}
}
