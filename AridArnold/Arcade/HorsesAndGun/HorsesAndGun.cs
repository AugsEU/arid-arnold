namespace HorsesAndGun
{
	internal class HorsesAndGun : AridArnold.ArcadeGame
	{
		public HorsesAndGun(GraphicsDeviceManager deviceManager, ContentManager content) : base(deviceManager, content)
		{
			ScreenManager.I.LoadAllScreens(content, deviceManager);
			ScreenManager.I.ActivateScreen(ScreenType.MainGame);
		}

		public override void ResetGame()
		{
			TimeManager.I.Reset();
			EntityManager.I.ClearEntities();
			ScreenManager.I.ActivateScreen(ScreenType.MainGame);
			ScoreManager.I.ResetAll();
			base.ResetGame();
		}

		public override void Update(GameTime gameTime)
		{
			ScreenManager.I.Update(gameTime);
			TimeManager.I.Update(gameTime);

			MainGameScreen mainGameScreen = (MainGameScreen)ScreenManager.I.GetScreen(ScreenType.MainGame);
			if(mainGameScreen.IsExitRequested())
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

			return screenTargetRef;
		}

		public override ulong GetScore()
		{
			return ScoreManager.I.GetCurrentScore();
		}

		public override string GetMusicID()
		{
			return "HorsesAndGun";
		}
	}
}
