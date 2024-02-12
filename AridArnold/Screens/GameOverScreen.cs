namespace AridArnold
{
	/// <summary>
	/// Screen displayed on a game over
	/// </summary>
	internal class GameOverScreen : Screen
	{
		#region rInitialisation

		/// <summary>
		/// Game over constructor
		/// </summary>
		/// <param name="graphics">Graphics device</param>
		public GameOverScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update game over screen, check for inputs.
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public override void Update(GameTime gameTime)
		{
			if (InputManager.I.KeyPressed(AridArnoldKeys.Confirm) || InputManager.I.KeyPressed(AridArnoldKeys.ArnoldJump))
			{
				ScreenManager.I.ActivateScreen(ScreenType.Game);
			}
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the game over screen to the renter target
		/// </summary>
		/// <param name="info">Information</param>
		/// <returns>Render target with screen on it</returns>
		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			SpriteFont pixelFont = FontManager.I.GetFont("Pixica-24");
			SpriteFont pixelSmallFont = FontManager.I.GetFont("Pixica-12");

			Vector2 centre = new Vector2(mScreenTarget.Width / 2, mScreenTarget.Height / 2);

			//Draw out the game area
			info.device.SetRenderTarget(mScreenTarget);
			info.device.Clear(new Color(0, 0, 0));

			StartScreenSpriteBatch(info);

			MonoDraw.DrawStringCentred(info, pixelFont, centre, Color.White, "GAME OVER");
			MonoDraw.DrawStringCentred(info, pixelSmallFont, centre + new Vector2(0.0f, 30.0f), Color.Yellow, "-Press Enter-");

			EndScreenSpriteBatch(info);

			return mScreenTarget;
		}

		#endregion rDraw
	}
}
