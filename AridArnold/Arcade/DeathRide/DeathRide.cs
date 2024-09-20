
namespace DeathRide
{
	internal class DeathRide : AridArnold.ArcadeGame
	{
		public DeathRide(GraphicsDeviceManager deviceManager, ContentManager content) : base(deviceManager, content)
		{
			MonoData.I.Init(content);
			CameraManager.I.Init();

			ScreenManager.I.LoadAllScreens(deviceManager);
			ScreenManager.I.ActivateScreen(ScreenType.Game);
		}

		public override void ResetGame()
		{
			TimeManager.I.Reset();
			RunManager.I.ResetNoEffects();
			ScreenManager.I.ActivateScreen(ScreenType.Game);
			base.ResetGame();
		}

		public override void Update(GameTime gameTime)
		{
			//Update Active screen
			Screen screen = ScreenManager.I.GetActiveScreen();

			//Always update every game update.
			CameraManager.I.UpdateAllCameras(gameTime);

			//Record elapsed time
			TimeManager.I.Update(gameTime);

			if (screen != null)
			{
				screen.Update(gameTime);
			}

			if(RunManager.I.ExitRequested())
			{
				SetState(ArcadeGameState.kGameOver);
			}
		}

		public override RenderTarget2D DrawToRenderTarget(AridArnold.DrawInfo info)
		{
			// These are actually different types. So convert them....
			DrawInfo frameInfo;

			frameInfo.graphics = info.graphics;
			frameInfo.spriteBatch = info.spriteBatch;
			frameInfo.gameTime = info.gameTime;
			frameInfo.device = info.device;

			RenderTarget2D screenTargetRef = null;

			//Draw active screen.
			Screen screen = ScreenManager.I.GetActiveScreen();

			if (screen != null)
			{
				screenTargetRef = screen.DrawToRenderTarget(frameInfo);
			}

			MonoDraw.FlushRender();

			return screenTargetRef;
		}

		public override ulong GetScore()
		{
			return RunManager.I.GetScore();
		}

		public override string GetMusicID()
		{
			return "DeathRide";
		}

		public override bool AllowQuit()
		{
			if(ScreenManager.I.GetActiveScreenType() == ScreenType.GameOver)
			{
				return false;
			}

			return base.AllowQuit();
		}
	}
}
