using System;
using System.Collections.Generic;

namespace HorsesAndGun
{
	internal class RossButtonsScreen : Screen
	{
		private List<Button> mButtonList;
		Texture2D mLogo;
		ScrollingImage mGround;

		MonoTimer mStartGameTimer;
		const double mWaitTime = 2000.0;

		private void PlayButtonClick(object sender, EventArgs e)
		{
			mStartGameTimer.Start();
			SoundManager.I.PlaySFX(SoundManager.SFXType.GetHerDone, 0.5f);
		}

		private void OptionsButtonClick(object sender, EventArgs e)
		{
			ScreenManager.I.ActivateScreen(ScreenType.Options);
		}

		private void QuitButtonClick(object sender, EventArgs e)
		{
			Game1.self.Exit();
		}

		public RossButtonsScreen(ContentManager content, GraphicsDeviceManager graphics) : base(content, graphics)
		{
			mStartGameTimer = new MonoTimer();
		}

		public override void LoadContent(ContentManager content)
		{
			Button playButton = new Button(content.Load<Texture2D>("wooden_button"), content.Load<SpriteFont>("Fonts/Pixica"))
			{
				mPosition = new Vector2(196.5f, 160.0f),
				mText = "Play"
			};

			Button optionsButton = new Button(content.Load<Texture2D>("wooden_button"), content.Load<SpriteFont>("Fonts/Pixica"))
			{
				mPosition = new Vector2(196.5f, 220.0f),
				mText = "Help"
			};

			Button quitButton = new Button(content.Load<Texture2D>("wooden_button"), content.Load<SpriteFont>("Fonts/Pixica"))
			{
				mPosition = new Vector2(196.5f, 280.0f),
				mText = "Quit"
			};

			mGround = new ScrollingImage(content.Load<Texture2D>("menu_background"), content.Load<Texture2D>("menu_background"), Vector2.Zero, 200);


			playButton.mOnMouseClick += PlayButtonClick;
			optionsButton.mOnMouseClick += OptionsButtonClick;
			quitButton.mOnMouseClick += QuitButtonClick;


			mButtonList = new List<Button>()
			{
				playButton,
				optionsButton,
				quitButton
			};

			mLogo = content.Load<Texture2D>("main_logo");
		}

		public override void OnActivate()
		{
			mStartGameTimer.FullReset();
		}

		public override void OnDeactivate()
		{
		}

		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			//Draw out the game area
			info.device.SetRenderTarget(mScreenTarget);
			info.device.Clear(new Color(211, 145, 65));

			info.spriteBatch.Begin(SpriteSortMode.Immediate,
									BlendState.AlphaBlend,
									SamplerState.PointClamp,
									DepthStencilState.None,
									RasterizerState.CullNone);

			mGround.Draw(info);

			foreach (Button button in mButtonList)
			{
				button.Draw(info);
			}

			Vector2 logoPos = new Vector2(SCREEN_WIDTH / 2.0f, 17.0f);
			logoPos.X -= mLogo.Width / 2.0f;

			info.spriteBatch.Draw(mLogo, logoPos, Color.White);

			info.spriteBatch.End();

			return mScreenTarget;
		}

		public override void Update(GameTime gameTime)
		{
			mGround.Update(gameTime);

			if (mStartGameTimer.IsPlaying())
			{
				if (mStartGameTimer.GetElapsedMs() > mWaitTime)
				{
					mStartGameTimer.FullReset();
					ScreenManager.I.ActivateScreen(ScreenType.MainGame);
				}
				return;
			}

			foreach (Button button in mButtonList)
			{
				button.Update();
			}
		}

	}
}
