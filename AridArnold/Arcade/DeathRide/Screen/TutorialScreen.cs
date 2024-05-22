using GMTK2023.UI;

namespace GMTK2023
{
	internal class TutorialScreen : Screen
	{
		Texture2D mBG;
		ScreenTransitionButton mTutorialBtn;

		public TutorialScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
			mTutorialBtn = new ScreenTransitionButton(new Vector2(770.0f, 430.0f), "Back", ScreenType.Title);
		}

		public override void LoadContent()
		{
			mBG = MonoData.I.MonoGameLoad<Texture2D>("Backgrounds/TutorialScreen");
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
			MonoDraw.DrawTexture(info, mBG, Vector2.Zero);
			mTutorialBtn.Draw(info);
			EndScreenSpriteBatch(info);

			return mScreenTarget;
		}

		public override void Update(GameTime gameTime)
		{
			mTutorialBtn.Update(gameTime);
		}
	}
}
