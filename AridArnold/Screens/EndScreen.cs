namespace AridArnold.Screens
{
	/// <summary>
	/// [TEMP] Screen displayed at the end of the game.
	/// </summary>
	internal class EndScreen : Screen
	{
		#region rInitialisation

		/// <summary>
		/// End screen constructor
		/// </summary>
		/// <param name="graphics">Graphics device</param>
		public EndScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
		}



		/// <summary>
		/// Triggers on activation. 
		/// </summary>
		public override void OnActivate()
		{
			ProgressManager.I.Init("Content/Meta/Campaigns/mainCampaign.xml"); //FIX THIS!!
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update end screen, check for inputs.
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public override void Update(GameTime gameTime)
		{
			if (InputManager.I.KeyPressed(AridArnoldKeys.Confirm))
			{
				ScreenManager.I.ActivateScreen(ScreenType.Game);
			}
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the end screen to the renter target
		/// </summary>
		/// <param name="info">Information</param>
		/// <returns>Render target with screen on it</returns>
		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			SpriteFont pixelFont = FontManager.I.GetFont("Pixica-24");

			Vector2 centre = new Vector2(mScreenTarget.Width / 2, mScreenTarget.Height / 2);

			//Draw out the game area
			info.device.SetRenderTarget(mScreenTarget);
			info.device.Clear(new Color(0, 0, 0));

			info.spriteBatch.Begin(SpriteSortMode.FrontToBack,
									BlendState.AlphaBlend,
									SamplerState.PointClamp,
									DepthStencilState.Default,
									RasterizerState.CullNone);

			MonoDraw.DrawStringCentred(info, pixelFont, centre, Color.White, "YOU WIN", DrawLayer.Text);

			info.spriteBatch.End();

			return mScreenTarget;
		}

		#endregion rDraw
	}
}
