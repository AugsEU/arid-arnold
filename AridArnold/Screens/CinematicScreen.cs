namespace AridArnold
{
	class CinematicScreen : Screen
	{
		#region rMembers

		GameCinematic mCurrentCinematic = null;
		ScreenType mScreenToReturnTo;

		#endregion rMembers


		/// <summary>
		/// Init cinematic screen
		/// </summary>
		public CinematicScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
		}




		#region rUpdate

		/// <summary>
		/// Called when opened.
		/// </summary>
		public override void OnActivate()
		{
			MonoDebug.Assert(mCurrentCinematic != null);
			mCurrentCinematic.PlayFromStart();
			base.OnActivate();
		}



		/// <summary>
		/// Update cinematic
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mCurrentCinematic.Update(gameTime);

			if (InputManager.I.KeyPressed(AridArnoldKeys.Confirm))
			{
				mCurrentCinematic.SkipToEnd();
			}

			if (mCurrentCinematic.IsComplete())
			{
				mCurrentCinematic.FullReset();
				ScreenManager.I.ActivateScreen(mScreenToReturnTo);
			}
		}


		/// <summary>
		/// Start the cinematic
		/// </summary>
		public void StartCinematic(GameCinematic cinematic, ScreenType returnToScreen)
		{
			mCurrentCinematic = cinematic;
			mScreenToReturnTo = returnToScreen;
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the cinematic
		/// </summary>
		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			StartScreenSpriteBatch(info);
			mCurrentCinematic.Draw(info);
			EndScreenSpriteBatch(info);

			return mScreenTarget;
		}

		#endregion rDraw
	}
}
