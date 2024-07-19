
namespace AridArnold
{
	/// <summary>
	/// Initial screen the game shows.
	/// Does some initial loading and displays the initial titles.
	/// </summary>
	internal class TitleScreen : Screen
	{
		const double FADE_IN_TIME = 8000.0;

		string mIcefishSoftware;
		string mPresents;
		PercentageTimer mFadeInTimer;

		public TitleScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
			mIcefishSoftware = LanguageManager.I.GetText("UI.IcefishSoftware");
			mPresents = LanguageManager.I.GetText("UI.Presents");

			mFadeInTimer = new PercentageTimer(FADE_IN_TIME);
		}

		public override void OnActivate()
		{
			mFadeInTimer.FullReset();
			mFadeInTimer.Start();
			base.OnActivate();
		}

		public override void Update(GameTime gameTime)
		{
			mFadeInTimer.Update(gameTime);

			if (Main.IsLoadingSection())
			{
				Main.LoadingScreenEnd();
			}

			if(InputManager.I.KeyPressed(InputAction.Confirm) || mFadeInTimer.GetPercentageF() >= 1.0f)
			{
				ScreenManager.I.ActivateScreen(ScreenType.MainMenu);
			}
		}

		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			SpriteFont pixelFont = FontManager.I.GetFont("Pixica", 36);
			SpriteFont pixelSmallFont = FontManager.I.GetFont("PixicaMicro", 24);

			Vector2 centre = new Vector2(mScreenTarget.Width / 2, mScreenTarget.Height / 2);

			//Draw out the game area
			StartScreenSpriteBatch(info);

			float t = GetColorT();
			Color textColor = new Color(t, t, t);
			MonoDraw.DrawStringCentred(info, pixelFont, centre, textColor, mIcefishSoftware);
			centre.Y += 25.0f;
			MonoDraw.DrawStringCentred(info, pixelSmallFont, centre, textColor, mPresents);

			EndScreenSpriteBatch(info);

			return mScreenTarget;
		}

		public float GetColorT()
		{
			const float alpha = 0.22f;
			const float beta = 12.0f;

			float t = mFadeInTimer.GetPercentageF();
			t = (t - 1.0f) * (alpha - t);
			t *= beta;

			t = Math.Clamp(t, 0.0f, 1.0f);
			t = MonoMath.SmoothZeroToOne(t);

			return t;
		}
	}
}
