using GMTK2023.UI;

namespace GMTK2023
{
	internal class NextRoundScreen : Screen
	{
		ScreenTransitionButton mNextRoundButton;

		public NextRoundScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
			mNextRoundButton = new ScreenTransitionButton(new Vector2(500.0f, 200.0f), "Next Round", ScreenType.Game);
		}

		public override void LoadContent()
		{
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

			mNextRoundButton.Draw(info);

			EndScreenSpriteBatch(info);

			return mScreenTarget;
		}

		public override void Update(GameTime gameTime)
		{
			mNextRoundButton.Update(gameTime);
		}
	}
}
