
namespace AridArnold
{
	/// <summary>
	/// Credits for the game.
	/// </summary>
	internal class CreditsScreen : Screen
	{
		const float FADE_TIME = 800.0f;

		string[] mCreditsLines;
		PercentageTimer mFadeTimer;

		public CreditsScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
			string locCreditsStr = LanguageManager.I.GetText("UI.Menu.Credits");

			mCreditsLines = locCreditsStr.Split("\n");

			mFadeTimer = new PercentageTimer(FADE_TIME);
		}

		public override void OnActivate()
		{
			mFadeTimer.ResetStart();
			base.OnActivate();
		}

		public override void Update(GameTime gameTime)
		{
			mFadeTimer.Update(gameTime);

			if (InputManager.I.AnyGangPressed(BindingGang.SysConfirm) || InputManager.I.KeyPressed(InputAction.SysLClick))
			{
				ScreenManager.I.ActivateScreen(ScreenType.MainMenu);
			}
		}

		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			StartScreenSpriteBatch(info);

			SpriteFont bigFont = FontManager.I.GetFont("Pixica", 24, true);
			SpriteFont smallFont = FontManager.I.GetFont("PixicaMicro", 24);

			float t = MathF.Floor(4 * mFadeTimer.GetPercentageF()) / 4;
			Color lerpColor = MonoMath.Lerp(Color.Black, new Color(200,200,200), t);


			Vector2 pos = new Vector2(SCREEN_WIDTH * 0.5f, 90.0f);
			for(int i = 0; i < mCreditsLines.Length; i++)
			{
				if(i % 2 == 0)
				{
					MonoDraw.DrawStringCentredShadow(info, smallFont, pos, lerpColor, mCreditsLines[i], 2.0f, DrawLayer.Front);
					pos.Y += 30.0f;
				}
				else
				{
					MonoDraw.DrawStringCentredShadow(info, bigFont, pos, lerpColor, mCreditsLines[i], 2.0f, DrawLayer.Front);
					pos.Y += 70.0f;
				}
			}

			EndScreenSpriteBatch(info);

			return mScreenTarget;
		}
	}
}
