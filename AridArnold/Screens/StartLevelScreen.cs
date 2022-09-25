namespace AridArnold.Screens
{
	internal class StartLevelScreen : Screen
	{
		#region rMembers

		MonoTimer mTimer;

		#endregion rMembers




		#region rInitialise

		/// <summary>
		/// Start level constructor
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		/// <param name="graphics">Graphics device manager</param>
		public StartLevelScreen(ContentManager content, GraphicsDeviceManager graphics) : base(content, graphics)
		{
			mTimer = new MonoTimer();
		}



		/// <summary>
		/// Rest on activation.
		/// </summary>
		public override void OnActivate()
		{
			mTimer.FullReset();
			mTimer.Start();
		}

		#endregion rInitialise





		#region rUpdate

		/// <summary>
		/// Update the screen
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public override void Update(GameTime gameTime)
		{
			KeyboardState state = Keyboard.GetState();

			if (mTimer.GetElapsedMs() > 2000 || state.IsKeyDown(Keys.Enter) || state.IsKeyDown(Keys.Space))
			{
				ScreenManager.I.ActivateScreen(ScreenType.Game);
			}
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the screen to a render target
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		/// <returns>Render target with the screen drawn on</returns>
		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			SpriteFont pixelFont = FontManager.I.GetFont("Pixica-24");

			Vector2 centre = new Vector2(mScreenTarget.Width / 2, mScreenTarget.Height / 2);

			//Draw out the game area
			info.device.SetRenderTarget(mScreenTarget);
			info.device.Clear(new Color(0, 0, 0));

			info.spriteBatch.Begin(SpriteSortMode.Immediate,
									BlendState.AlphaBlend,
									SamplerState.PointClamp,
									DepthStencilState.None,
									RasterizerState.CullNone);
			Util.DrawStringCentred(info.spriteBatch, pixelFont, centre, Color.Gold, (ProgressManager.I.GetWorldData().name));

			centre.Y += 25.0f;

			Util.DrawStringCentred(info.spriteBatch, pixelFont, centre, Color.White, "Level " + (ProgressManager.I.CurrentLevel + 1));

			info.spriteBatch.End();

			return mScreenTarget;
		}

		#endregion rDraw
	}
}
