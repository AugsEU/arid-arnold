namespace AridArnold
{
	internal class TitleScreen : Screen
	{
		#region rMembers

		Texture2D mBG;

		#endregion rMembers




		#region rInitialise

		/// <summary>
		/// Start level constructor
		/// </summary>
		/// <param name="graphics">Graphics device manager</param>
		public TitleScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
		}

		public override void LoadContent()
		{
			mBG = MonoData.I.MonoGameLoad<Texture2D>("UI/Title");
			base.LoadContent();
		}

		#endregion rInitialise





		#region rUpdate

		/// <summary>
		/// Update the screen
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

			StartScreenSpriteBatch(info);

			MonoDraw.DrawTexture(info, mBG, Vector2.Zero);

			EndScreenSpriteBatch(info);

			return mScreenTarget;
		}

		#endregion rDraw
	}
}
