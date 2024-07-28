using Microsoft.Xna.Framework;

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
			FXManager.I.Init(SCREEN_WIDTH, SCREEN_HEIGHT);
			FXManager.I.Clear();
			base.OnActivate();
		}



		/// <summary>
		/// Update cinematic
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			FXManager.I.Update(gameTime);
			mCurrentCinematic.Update(gameTime);

			if (InputManager.I.KeyPressed(InputAction.Confirm))
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
			FXManager.I.Draw(info);

#if NO_DEBUG
			CameraSpec screenCamSpec = CameraManager.I.GetCamera(CameraManager.CameraInstance.ScreenCamera).GetCurrentSpec();
			string frameStr = string.Format("FR: {0}", mCurrentCinematic.GetFrameFromElapsedTime());
			Vector2 textPos = screenCamSpec.mPosition + new Vector2(97.0f, 57.0f) * screenCamSpec.mZoom;
			MonoDraw.DrawDebugText(info, frameStr, textPos);
#endif // DEBUG

			EndScreenSpriteBatch(info);



			return mScreenTarget;
		}

		#endregion rDraw
	}
}
