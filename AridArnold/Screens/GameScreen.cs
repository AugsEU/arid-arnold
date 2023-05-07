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

		ScreenFade mFadeOut;
		ScreenFade mFadeIn;

		Layout mHubUI;
		Layout mLoadingUI;
		Layout mLevelUI;

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
		}



		/// <summary>
		/// Called when the game screen is activated, sets up the tile manager.
		/// </summary>
		public override void OnActivate()
		{
		}



		/// <summary>
		/// Load content for UI elements
		/// </summary>
		public override void LoadContent()
		{
			mHubUI = new Layout("Layouts/MainHub.mlo");
			mLevelUI = new Layout("Layouts/MainLevel.mlo");
			mLoadingUI = new Layout("Layouts/MainLoading.mlo");
		}

		#endregion





		#region rUpdate

		/// <summary>
		/// Update the main gameplay screen
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public override void Update(GameTime gameTime)
		{
			if (mLoadSequence is not null)
			{
				mLoadSequence.Update(gameTime);

				if (mLoadSequence.Finished())
				{
					mLoadSequence = null;
				}
			}
			else
			{
				GetCurrentUI().Update(gameTime);
				GameUpdate(gameTime);
			}
		}



		/// <summary>
		/// Update gameplay elements
		/// </summary>
		void GameUpdate(GameTime gameTime)
		{
			CheckForLoadSequence();

			Level currLevel = CampaignManager.I.GetCurrentLevel();
			if (currLevel is null || mLoadSequence is not null)
			{
				return;
			}

			FXManager.I.Update(gameTime);
			if (mLevelEndTimer.IsPlaying())
			{
				if (mLevelEndTimer.GetPercentage() == 1.0)
				{
					MoveToNextLevel();
				}

				return;
			}

			HandleInput();
			GhostManager.I.Update(gameTime);
			EntityManager.I.Update(gameTime);
			TileManager.I.Update(gameTime);
			ItemManager.I.Update(gameTime);

			LevelStatus status = currLevel.Update(gameTime);

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
		/// Handle any system-wide input
		/// </summary>
		void HandleInput()
		{
			if (InputManager.I.KeyPressed(AridArnoldKeys.RestartLevel))
			{
				EventManager.I.SendEvent(EventType.KillPlayer, new EArgs(this));
			}
			else if (InputManager.I.KeyPressed(AridArnoldKeys.SkipLevel))
			{
				LevelWin();
			}
		}



		/// <summary>
		/// Call to win the level. We will move to the next level shortly.
		/// </summary>
		private void LevelWin()
		{
			mLevelEndTimer.Start();
			DisplayLevelEndText();
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
			info.device.SetRenderTarget(mScreenTarget);
			info.device.Clear(new Color(0, 0, 0));

			info.spriteBatch.Begin(SpriteSortMode.FrontToBack,
									BlendState.AlphaBlend,
									SamplerState.PointClamp,
									DepthStencilState.Default,
									RasterizerState.CullNone);

			GetCurrentUI().Draw(info);
			Rectangle gameAreaRect = GetGameAreaRect();
			DrawGameArea(info, gameAreaRect);

			info.spriteBatch.End();

			return mScreenTarget;
		}



		/// <summary>
		/// Draw game area
		/// </summary>
		/// <param name="info"></param>
		/// <param name="destRect"></param>
		private void DrawGameArea(DrawInfo info, Rectangle destRect)
		{
			MonoDraw.DrawTexture(info, mGameArea, destRect);
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

			info.device.SetRenderTarget(mGameArea);

			info.device.Clear(Color.Black);

			info.spriteBatch.Begin(SpriteSortMode.FrontToBack,
									BlendState.AlphaBlend,
									SamplerState.PointClamp,
									DepthStencilState.Default,
									RasterizerState.CullNone);

			if (mLoadSequence is not null)
			{
				mLoadSequence.Draw(info);
			}

			DrawGamePlay(info);

			info.spriteBatch.End();
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

				FXManager.I.AddTextScroller(FontManager.I.GetFont("Pixica Micro-24"), textCol, arnold.GetPos(), prefix + GhostManager.I.FrameTimeToString(frameDiff));
			}
			else
			{
				string levelCompleteMsg = "Level complete";

				// To do: Do we need checkpoints?
				//if (ProgressManager.I.GetCurrentLevel() is CollectFlagLevel)
				//{
				//	levelCompleteMsg = "Checkpoint!";
				//}
				FXManager.I.AddTextScroller(FontManager.I.GetFont("Pixica Micro-24"), Color.Wheat, arnold.GetPos(), levelCompleteMsg);
			}
		}

		#endregion rDraw





		#region rUtil

		/// <summary>
		/// Get current layout
		/// </summary>
		private Layout GetCurrentUI()
		{
			if(mLoadSequence is not null)
			{
				return mLoadingUI;
			}

			CampaignManager.GameplayState state = CampaignManager.I.GetGameplayState();

			switch (state)
			{
				case CampaignManager.GameplayState.HubWorld:
					return mHubUI;
				case CampaignManager.GameplayState.LevelSequence:
					return mLevelUI;
			}

			throw new NotImplementedException();
		}

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
