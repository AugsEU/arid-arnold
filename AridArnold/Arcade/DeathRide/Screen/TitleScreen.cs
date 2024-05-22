using GMTK2023.UI;

namespace GMTK2023
{
	internal class TitleScreen : Screen
	{
		Texture2D mBackgroundTex;
		Texture2D mLogo;

		ScreenTransitionButton mStartGameBtn;
		ScreenTransitionButton mTutorialBtn;
		ExitGameButton mExitGameBtn;

		public TitleScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
			mStartGameBtn = new ScreenTransitionButton(new Vector2(700.0f, 50.0f), "Play", ScreenType.Game);
			mExitGameBtn = new ExitGameButton(new Vector2(700.0f, 250.0f), "Exit");
			mTutorialBtn = new ScreenTransitionButton(new Vector2(700.0f, 150.0f), "Help", ScreenType.Tutorial);
		}

		public override void LoadContent()
		{
			mBackgroundTex = MonoData.I.MonoGameLoad<Texture2D>("Backgrounds/TitleScreen");
			mLogo = MonoData.I.MonoGameLoad<Texture2D>("UI/TitleLogo");
			base.LoadContent();
		}

		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			SpriteFont pixelFont = FontManager.I.GetFont("Pixica-24");

			Vector2 centre = new Vector2(mScreenTarget.Width / 2, mScreenTarget.Height / 2);

			//Draw out the game area
			info.device.SetRenderTarget(mScreenTarget);
			info.device.Clear(new Color(0, 0, 0));

			StartScreenSpriteBatch(info);

			MonoDraw.DrawTextureDepth(info, mBackgroundTex, Vector2.Zero, DrawLayer.BackgroundElement);

			MonoDraw.DrawTextureDepth(info, mLogo, new Vector2(32.0f, 16.0f), DrawLayer.BackgroundElement);

			mStartGameBtn.Draw(info);
			mExitGameBtn.Draw(info);
			mTutorialBtn.Draw(info);

			EndScreenSpriteBatch(info);

			return mScreenTarget;
		}

		public override void Update(GameTime gameTime)
		{
			mStartGameBtn.Update(gameTime);
			mExitGameBtn.Update(gameTime);
			mTutorialBtn.Update(gameTime);
		}
	}
}
