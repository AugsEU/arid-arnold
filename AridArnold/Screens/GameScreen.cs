namespace AridArnold
{
	/// <summary>
	/// Gameplay screen
	/// </summary>
	internal class GameScreen : Screen
	{
		#region rConstants

		public const int TILE_SIZE = 16;

		const double END_LEVEL_TIME = 1000.0;

		public const int GAME_AREA_WIDTH = 544;
		public const int GAME_AREA_HEIGHT = 528;
		const int GAME_AREA_X = 208;
		const int GAME_AREA_Y = 6;

		#endregion rConstants





		#region rMembers

		RenderTarget2D mGameArea;
		private PercentageTimer mLevelEndTimer;
		LoadingSequence mLoadSequence;

		Layout mMainUI;

		FadeFX mFadeInFx;

		PauseMenu mPauseMenu;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Game screen constructor
		/// </summary>
		/// <param name="graphics">Graphics device</param>
		public GameScreen(GraphicsDeviceManager graphics) : base(graphics)
		{
			mLevelEndTimer = new PercentageTimer(END_LEVEL_TIME);

			mGameArea = null;
			mLoadSequence = null;

			FXManager.I.Init(GAME_AREA_WIDTH, GAME_AREA_HEIGHT);
			TileManager.I.Init(new Vector2(-TILE_SIZE, -TILE_SIZE), TILE_SIZE);
			TimeZoneManager.I.Init();

			mFadeInFx = new FadeFX(new ScreenStars(), 0.1f, true);

			mPauseMenu = new PauseMenu();
		}



		/// <summary>
		/// Called when the game screen is activated, sets up the tile manager.
		/// </summary>
		public override void OnActivate()
		{
			mFadeInFx = new FadeFX(new ScreenStars(10.0f, SCREEN_RECTANGLE));

			FXManager.I.Init(GAME_AREA_WIDTH, GAME_AREA_HEIGHT);
			mPauseMenu.Close();
		}



		/// <summary>
		/// Load content for UI elements
		/// </summary>
		public override void LoadContent()
		{
			mMainUI = new Layout("Layouts/MainGame.mlo");
		}

		#endregion





		#region rUpdate

		/// <summary>
		/// Update the main gameplay screen
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public override void Update(GameTime gameTime)
		{
			mFadeInFx.Update(gameTime);
			mLevelEndTimer.Update(gameTime);

			if (mLoadSequence is null)
			{
				CheckForLoadSequence();
			}

			mMainUI.Update(gameTime);

			if (mLoadSequence is not null)
			{
				mLoadSequence.Update(gameTime);

				if (mLoadSequence.Finished())
				{
					mLoadSequence = null;
				}
			}
			else if(mPauseMenu.IsOpen())
			{
				mPauseMenu.Update(gameTime);
			}
			else
			{
				GameUpdate(gameTime);
			}
		}



		/// <summary>
		/// Do all required steps for game update
		/// </summary>
		void GameUpdate(GameTime gameTime)
		{
			if (CameraManager.I.BlockUpdateRequested())
			{
				return;
			}

			if (InputManager.I.KeyHeld(InputAction.Pause) && AllowPauseMenu())
			{
				// Open pause menu and immediately abort the update.
				mPauseMenu.Open();
				return;
			}

			FXManager.I.Update(gameTime);
			if (mLevelEndTimer.IsPlaying())
			{
				if (mLevelEndTimer.GetPercentage() >= 0.95)
				{
					MoveToNextLevel();
				}
				return;
			}

			Level currLevel = CampaignManager.I.GetCurrentLevel();

			GameUpdateStep(gameTime, currLevel);

			// Check status.
			LevelStatus status = currLevel.GetStatus();
			if (status == LevelStatus.Win)
			{
				LevelWin();
			}
			else if (status == LevelStatus.Loss)
			{
				LevelLose();
			}
		}



		/// <summary>
		/// Update gameplay elements by 1 step
		/// </summary>
		void GameUpdateStep(GameTime gameTime, Level level)
		{
			HandleInput();
			GhostManager.I.Update(gameTime);
			EntityManager.I.Update(gameTime);
			TileManager.I.Update(gameTime);
			ItemManager.I.Update(gameTime);
			FlagsManager.I.Update(gameTime);

			level.Update(gameTime);

			EventManager.I.Update(gameTime);
		}



		/// <summary>
		/// Handle any system-wide input
		/// </summary>
		void HandleInput()
		{
			if (InputManager.I.KeyPressed(InputAction.RestartLevel) &&
				CampaignManager.I.GetGameplayState() != CampaignManager.GameplayState.HubWorld)
			{
				for (int i = 0; i < EntityManager.I.GetEntityNum(); i++)
				{
					Entity entity = EntityManager.I.GetEntity(i);
					if (entity is Arnold arnold)
					{
						// Kill all arnolds
						arnold.Kill();
					}
				}
			}
#if DEBUG
			else if (InputManager.I.KeyPressed(InputAction.SkipLevel))
			{
				LevelWin();
			}
#endif // DEBUG
		}



		/// <summary>
		/// Are we allowed to open the pause menu?
		/// </summary>
		bool AllowPauseMenu()
		{
			if(EventManager.I.IsAnyEventHappening())
			{
				// Wait for events to avoid weird saving jank.
				return false;
			}

			bool anyArnolds = false;
			for(int i = 0; i < EntityManager.I.GetEntityNum(); i++)
			{
				Entity entity = EntityManager.I.GetEntity(i);
				if(entity is Arnold arnold)
				{
					anyArnolds = true; // Found one.
					// Sorry not while you are dying...
					if (arnold.IsDying())
					{
						return false;
					}
				}
			}

			if(!anyArnolds)
			{
				// No arnolds, wait for one to spawn so we can save it.
				return false;
			}

			return true;
		}



		/// <summary>
		/// Call to win the level. We will move to the next level shortly.
		/// </summary>
		private void LevelWin()
		{
			mLevelEndTimer.Start();
			DisplayLevelEndText();
			SFXManager.I.EndAllSFX(150.0f);
			SFXManager.I.PlaySFX(AridArnoldSFX.CollectKey, 0.4f);
		}



		/// <summary>
		/// Call to win the level. We will move to the next level shortly.
		/// </summary>
		private void LevelLose()
		{
			CampaignManager.I.LoseLife();

			if (CampaignManager.I.IsGameover())
			{
				CampaignManager.I.QueueLoadSequence(new ReturnToHubFailureLoader());
			}
			else
			{
				CampaignManager.I.RestartCurrentLevel();
			}
		}



		/// <summary>
		/// Query the CampaignManager for a level change.
		/// </summary>
		private void CheckForLoadSequence()
		{
			mLoadSequence = CampaignManager.I.PopLoadSequence();
		}



		/// <summary>
		/// Move to next level
		/// </summary>
		private void MoveToNextLevel()
		{
			if (CampaignManager.I.GetNextLevelInSequence() is not null)
			{
				CampaignManager.I.QueueLoadSequence(new LevelSequenceLoader());
			}
			else
			{
				CampaignManager.I.QueueLoadSequence(new ReturnToHubSuccessLoader());
			}
			mLevelEndTimer.FullReset();
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw screen to render target
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		/// <returns>Render target with screen drawn on it</returns>
		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			//Get game rendered as a texture
			RenderGameAreaToTarget(info);

			//Draw out the game area
			StartScreenSpriteBatch(info);

			if (!mFadeInFx.Finished())
			{
				mFadeInFx.Draw(info);
			}
			mMainUI.Draw(info);
			DrawGameArea(info);

			mPauseMenu.Draw(info);

			EndScreenSpriteBatch(info);

			return mScreenTarget;
		}



		/// <summary>
		/// Draw game area
		/// </summary>
		private void DrawGameArea(DrawInfo info)
		{
			Rectangle gameAreaRect = GetGameAreaRect();
			MonoDraw.DrawTexture(info, mGameArea, gameAreaRect);
		}



		/// <summary>
		/// Render the main game to the render target
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		private void RenderGameAreaToTarget(DrawInfo info)
		{
			if (mGameArea == null)
			{
				mGameArea = new RenderTarget2D(info.device, GAME_AREA_WIDTH, GAME_AREA_HEIGHT);
			}

			Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera);
			gameCam.StartSpriteBatch(info, new Vector2(GAME_AREA_WIDTH, GAME_AREA_HEIGHT), mGameArea, Color.Black);

			if (mLoadSequence is not null)
			{
				mLoadSequence.Draw(info);
			}

			DrawGamePlay(info);

#if DEBUG
			// Debug stuff
			MonoDebug.DrawDebugRects(info);
#endif

			gameCam.EndSpriteBatch(info);
		}



		/// <summary>
		/// Draw gameplay
		/// </summary>
		private void DrawGamePlay(DrawInfo info)
		{
			Level currLevel = CampaignManager.I.GetCurrentLevel();
			if (currLevel is null || currLevel.IsActive() == false)
			{
				return;
			}

			GhostManager.I.Draw(info);
			EntityManager.I.Draw(info);
			TileManager.I.Draw(info);
			FXManager.I.Draw(info);
			currLevel.Draw(info);
		}



		/// <summary>
		/// Display text at the end of the level.
		/// </summary>
		private void DisplayLevelEndText()
		{
			for (int i = 0; i < EntityManager.I.GetEntityNum(); i++)
			{
				Entity e = EntityManager.I.GetEntity(i);
				if (e is Arnold)
				{
					DisplayLevelEndTextOnArnold((Arnold)e);
				}
			}
		}



		/// <summary>
		/// Display text on a specific instance of arnold
		/// </summary>
		/// <param name="arnold">Which arnold to draw the text over</param>
		private void DisplayLevelEndTextOnArnold(Arnold arnold)
		{
			int? timeDiff = GhostManager.I.GetTimeDifference();

			if (timeDiff.HasValue)
			{
				int frameDiff = timeDiff.Value;

				Color textCol = Color.White;
				string prefix = "+";

				if (frameDiff > 0)
				{
					prefix = "+";
					textCol = Color.Crimson;
				}
				else if (frameDiff < 0)
				{
					prefix = "-";
					textCol = Color.DeepSkyBlue;
					frameDiff = -frameDiff;
				}

				FXManager.I.AddTextScroller(FontManager.I.GetFont("PixicaMicro-24"), textCol, arnold.GetPos(), prefix + MonoText.GetTimeTextFromFrames(frameDiff));
			}
			else
			{
				string levelCompleteMsg = LanguageManager.I.GetText("InGame.LevelComplete");
				FXManager.I.AddTextScroller(FontManager.I.GetFont("PixicaMicro-24"), Color.Wheat, arnold.GetPos(), levelCompleteMsg);
			}
		}

		#endregion rDraw





		#region rUtil

		/// <summary>
		/// Area of the screen we actually play the game in
		/// </summary>
		/// <returns></returns>
		private Rectangle GetGameAreaRect()
		{
			return new Rectangle(GAME_AREA_X, GAME_AREA_Y, GAME_AREA_WIDTH, GAME_AREA_HEIGHT);
		}

		#endregion rUtil
	}
}
