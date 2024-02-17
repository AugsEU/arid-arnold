namespace AridArnold
{
	internal class TitleScreen : Screen
	{
		#region rMembers

		Layout mLayoutBG;

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
			mLayoutBG = new Layout("Layouts/TitleScreen.mlo");
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
			mLayoutBG.Update(gameTime);

			if (InputManager.I.KeyPressed(AridArnoldKeys.Confirm))
			{
				ScreenManager.I.ActivateScreen(ScreenType.Game);
			}

			if (InputManager.I.KeyHeld(AridArnoldKeys.RestartLevel) &&
				InputManager.I.KeyHeld(AridArnoldKeys.ArnoldUp))
			{
				CollectableManager.I.ChangePermanentItem(0x0000, 20);
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
			//Draw out the game area
			info.device.SetRenderTarget(mScreenTarget);
			info.device.Clear(new Color(0, 0, 0));

			StartScreenSpriteBatch(info);

			mLayoutBG.Draw(info);

			EndScreenSpriteBatch(info);

			return mScreenTarget;
		}

		#endregion rDraw
	}
}
