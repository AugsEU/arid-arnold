using GMTK2023.UI;

namespace GMTK2023
{
	internal class GameOverScreen : Screen
	{
		Texture2D mBG;
		ScreenTransitionButton mTutorialBtn;

		public GameOverScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
			mTutorialBtn = new ScreenTransitionButton(new Vector2(770.0f, 430.0f), "Back", ScreenType.Title);
		}

		public override void LoadContent()
		{
			mBG = MonoData.I.MonoGameLoad<Texture2D>("Backgrounds/GameOver");
			base.LoadContent();
		}

		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			SpriteFont pixelFont = FontManager.I.GetFont("Scream-24");

			Vector2 centre = new Vector2(mScreenTarget.Width / 2, mScreenTarget.Height / 2);

			//Draw out the game area
			info.device.SetRenderTarget(mScreenTarget);
			info.device.Clear(new Color(0, 0, 0));

			StartScreenSpriteBatch(info);

			MonoDraw.DrawTexture(info, mBG, Vector2.Zero);
			mTutorialBtn.Draw(info);

			Color textCol = new Color(255, 219, 162);
			MonoDraw.DrawShadowStringCentred(info, pixelFont, centre, textCol, "Rounds: " + RunManager.I.GetRounds());
			centre.Y += 50.0f;
			MonoDraw.DrawShadowStringCentred(info, pixelFont, centre, textCol, "High Score: " + RunManager.I.GetHighScore());

			EndScreenSpriteBatch(info);

			return mScreenTarget;
		}

		public override void Update(GameTime gameTime)
		{
			mTutorialBtn.Update(gameTime);
		}
	}
}
