namespace AridArnold.Screens
{
	/// <summary>
	/// Gameplay screen
	/// </summary>
	internal class GameScreen : Screen
	{
		#region rConstants

		public const int TILE_SIZE = 16;

		private const double END_LEVEL_TIME = 1000.0;
		private const double END_LEVEL_FLASH_TIME = 100.0;
		private const int UI_PANEL_SIZE = 190;

		#endregion rConstants





		#region rMembers

		private RenderTarget2D mGameArea;
		private RenderTarget2D mLeftUI;
		private RenderTarget2D mRightUI;

		private Texture2D mLifeTexture;
		private Texture2D mEmptyLifeTexture;
		private Texture2D mUIBG;
		private BGRenderer mBGRenderer;

		private PercentageTimer mLevelEndTimer;

		SpriteFont mPixelFont;

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
			mLeftUI = null;
			mRightUI = null;
		}



		/// <summary>
		/// Load level
		/// </summary>
		/// <param name="levelToBegin">Level object to begin</param>
		private void LoadLevel(Level levelToBegin)
		{
			FXManager.I.Clear();
			levelToBegin.Begin();

			if(ProgressManager.I.CanLoseLives() == false)
			{
				//Can't lose lives on this level, so we must reset the life count to the default.
				ProgressManager.I.ResetLives();
			}

			string bgName = ProgressManager.I.GetCurrentWorld().GetTheme().GetBGName();

			mBGRenderer = BGRenderer.GetRenderer(bgName);
		}



		/// <summary>
		/// Called when the game screen is activated, sets up the tile manager.
		/// </summary>
		public override void OnActivate()
		{
			TileManager.I.Init(new Vector2(0.0f, TILE_SIZE), TILE_SIZE);
			TileManager.I.CentreX(TileManager.I.GetDrawWidth() + 2 * TILE_SIZE);

			StartLevel();
		}



		/// <summary>
		/// Load content for UI elements
		/// </summary>
		public override void LoadContent()
		{
			mPixelFont = FontManager.I.GetFont("Pixica Micro-24");
			mLifeTexture = MonoData.I.MonoGameLoad<Texture2D>("UI/Arnold-Life");
			mEmptyLifeTexture = MonoData.I.MonoGameLoad<Texture2D>("UI/Arnold-Life-Empty");
			mUIBG = MonoData.I.MonoGameLoad<Texture2D>("UI/ui_bg");
		}

		#endregion





		#region rUtility

		/// <summary>
		/// Start the current level. Restarts if we have already started it.
		/// </summary>
		private void StartLevel()
		{
			Level levelToStart = ProgressManager.I.GetCurrentLevel();
			LoadLevel(levelToStart);
			mLevelEndTimer.FullReset();
			GhostManager.I.StartLevel(levelToStart);
		}



		/// <summary>
		/// Area of the screen we actually play the game in
		/// </summary>
		/// <returns></returns>
		private Rectangle GetGameAreaRect()
		{
			return new Rectangle((mScreenTarget.Width - mGameArea.Width) / 2, (mScreenTarget.Height - mGameArea.Height) / 2, mGameArea.Width, mGameArea.Height);
		}

		#endregion rUtility





		#region rDraw

		/// <summary>
		/// Draw screen to render target
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		/// <returns>Render target with screen drawn on it</returns>
		public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
		{
			//Get game rendered as a texture & UI
			RenderGameAreaToTarget(info);
			DrawUIToTarget(info);

			//Draw out the game area
			info.device.SetRenderTarget(mScreenTarget);
			info.device.Clear(new Color(0, 0, 0));

			info.spriteBatch.Begin(SpriteSortMode.FrontToBack,
									BlendState.AlphaBlend,
									SamplerState.PointClamp,
									DepthStencilState.Default,
									RasterizerState.CullNone);

			MonoDraw.DrawTexture(info, mUIBG, Vector2.Zero);

			Rectangle gameAreaRect = GetGameAreaRect();
			DrawGameArea(info, gameAreaRect);
			DrawUI(info, gameAreaRect);



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
				mGameArea = new RenderTarget2D(info.device, TileManager.I.GetDrawWidth() + 2 * TILE_SIZE, TileManager.I.GetDrawHeight() + TILE_SIZE);
			}

			info.device.SetRenderTarget(mGameArea);

			if (mLevelEndTimer.IsPlaying())
			{
				double timeSinceDeath = mLevelEndTimer.GetElapsedMs();

				if ((int)(timeSinceDeath / END_LEVEL_FLASH_TIME) % 2 == 0)
				{
					// TO DO: ADD FLASHING!!
					//MonoColor.BrightenColour(ref clearCol, 0.05f);
				}
			}

			info.device.Clear(Color.Black);

			info.spriteBatch.Begin(SpriteSortMode.FrontToBack,
									BlendState.AlphaBlend,
									SamplerState.PointClamp,
									DepthStencilState.Default,
									RasterizerState.CullNone);

			GhostManager.I.Draw(info);
			EntityManager.I.Draw(info);
			TileManager.I.Draw(info);
			FXManager.I.Draw(info);

			mBGRenderer.Draw(info);

			info.spriteBatch.End();
		}



		/// <summary>
		/// Draw UI next to the game area
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		/// <param name="gameAreaRect">Rectangle that represents the game area</param>
		private void DrawUI(DrawInfo info, Rectangle gameAreaRect)
		{
			Rectangle leftRectangle = new Rectangle((gameAreaRect.X - mLeftUI.Width) / 2, gameAreaRect.Y, mLeftUI.Width, mLeftUI.Height);
			Rectangle rightRectangle = new Rectangle((mScreenTarget.Width + gameAreaRect.X + gameAreaRect.Width - mRightUI.Width) / 2, gameAreaRect.Y, mRightUI.Width, mRightUI.Height);

			MonoDraw.DrawTexture(info, mLeftUI, leftRectangle);
			MonoDraw.DrawTexture(info, mRightUI, rightRectangle);
		}



		/// <summary>
		/// Draw the left and right UI to their render targets.
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		private void DrawUIToTarget(DrawInfo info)
		{
			if (mLeftUI == null && mRightUI == null)
			{
				mLeftUI = new RenderTarget2D(info.device, UI_PANEL_SIZE, mGameArea.Height);
				mRightUI = new RenderTarget2D(info.device, UI_PANEL_SIZE, mGameArea.Height);
			}

			DrawLeftUI(info);
			DrawRightUI(info);
		}



		/// <summary>
		/// Draw the left side of the UI
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		private void DrawLeftUI(DrawInfo info)
		{
			info.device.SetRenderTarget(mLeftUI);
			info.device.Clear(Color.Transparent);

			info.spriteBatch.Begin(SpriteSortMode.FrontToBack,
						BlendState.AlphaBlend,
						SamplerState.PointClamp,
						DepthStencilState.Default,
						RasterizerState.CullNone);

			int lives = ProgressManager.I.pLives;

			int texScale = 4;
			int texWidth = mLifeTexture.Width * texScale;
			int texHeight = mLifeTexture.Height * texScale;

			if (ProgressManager.I.CanLoseLives())
			{
				Rectangle lifeRect = new Rectangle((mLeftUI.Width - texWidth) / 2, 32, texWidth, texHeight);
				Rectangle emptyLifeRect = new Rectangle((mLeftUI.Width - texWidth) / 2, 32, texWidth + 1, texHeight + 1);

				for (int i = 0; i < ProgressManager.MAX_LIVES; i++)
				{
					if (i < lives)
					{
						MonoDraw.DrawTexture(info, mLifeTexture, lifeRect);
					}
					else
					{
						MonoDraw.DrawTexture(info, mEmptyLifeTexture, emptyLifeRect);
					}

					lifeRect.Y += texHeight + 10;
					emptyLifeRect.Y += texHeight + 10;
				}
			}

			MonoDraw.DrawStringCentred(info, mPixelFont, new Vector2(mLeftUI.Width / 2, 485.0f), Color.Yellow, ProgressManager.I.GetCurrentWorld().GetName(), MonoDraw.LAYER_TEXT);
			MonoDraw.DrawStringCentred(info, mPixelFont, new Vector2(mLeftUI.Width / 2, 505.0f), Color.White, "Level " + ProgressManager.I.GetLevelNumber(), MonoDraw.LAYER_TEXT);

			info.spriteBatch.End();
		}



		/// <summary>
		/// Draw the right side of the UI
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		private void DrawRightUI(DrawInfo info)
		{
			info.device.SetRenderTarget(mRightUI);
			info.device.Clear(Color.Transparent);

			info.spriteBatch.Begin(SpriteSortMode.FrontToBack,
						BlendState.AlphaBlend,
						SamplerState.PointClamp,
						DepthStencilState.Default,
						RasterizerState.CullNone);

			MonoDraw.DrawStringCentred(info, mPixelFont, new Vector2(mRightUI.Width / 2, 223.0f), Color.White, "Time", MonoDraw.LAYER_TEXT);
			MonoDraw.DrawStringCentred(info, mPixelFont, new Vector2(mRightUI.Width / 2, 238.0f), Color.White, GhostManager.I.GetTime(), MonoDraw.LAYER_TEXT);

			string timeToBeat = GhostManager.I.GetTimeToBeat();

			if (timeToBeat != "")
			{
				MonoDraw.DrawStringCentred(info, mPixelFont, new Vector2(mRightUI.Width / 2, 283.0f), Color.DarkOliveGreen, "Time to beat", MonoDraw.LAYER_TEXT);
				MonoDraw.DrawStringCentred(info, mPixelFont, new Vector2(mRightUI.Width / 2, 298.0f), Color.DarkOliveGreen, timeToBeat, MonoDraw.LAYER_TEXT);
			}

			info.spriteBatch.End();
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

				if (ProgressManager.I.GetCurrentLevel() is CollectFlagLevel)
				{
					levelCompleteMsg = "Checkpoint!";
				}
				FXManager.I.AddTextScroller(FontManager.I.GetFont("Pixica Micro-24"), Color.Wheat, arnold.GetPos(), levelCompleteMsg);
			}
		}

		#endregion rDraw





		#region rUpdate

		/// <summary>
		/// Update the main gameplay screen
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public override void Update(GameTime gameTime)
		{
			FXManager.I.Update(gameTime);
			if (mLevelEndTimer.IsPlaying())
			{
				if (mLevelEndTimer.GetPercentage() == 1.0)
				{
					MoveToNextLevel();
				}

				return;
			}

			KeyboardState keyboardState = Keyboard.GetState();

			if (InputManager.I.KeyPressed(AridArnoldKeys.RestartLevel))
			{
				EArgs eArgs;
				eArgs.sender = this;

				EventManager.I.SendEvent(EventType.KillPlayer, eArgs);
			}

			GhostManager.I.Update(gameTime);
			EntityManager.I.Update(gameTime);
			TileManager.I.Update(gameTime);
			mBGRenderer.Update(gameTime);

			LevelStatus status = ProgressManager.I.GetCurrentLevel().Update(gameTime);

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
		/// Call to win the level. We will move to the next level shortly.
		/// </summary>
		private void LevelWin()
		{
			mLevelEndTimer.Start();
			DisplayLevelEndText();
			GhostManager.I.EndLevel(true);
		}



		/// <summary>
		/// Move to the next level.
		/// </summary>
		private void MoveToNextLevel()
		{
			ProgressManager.I.ReportLevelWin();
			FXManager.I.Clear();
			if (ProgressManager.I.HasFinishedGame())
			{
				ScreenManager.I.ActivateScreen(ScreenType.EndGame);
			}
			else
			{
				ScreenManager.I.ActivateScreen(ScreenType.LevelStart);
			}
		}



		/// <summary>
		/// Call when the level is lost. Restart the level again.
		/// </summary>
		private void LevelLose()
		{
			ProgressManager.I.ReportLevelLoss();

			if (ProgressManager.I.pLives == 0)
			{
				ScreenManager.I.ActivateScreen(ScreenType.GameOver);
				ProgressManager.I.ResetGame();
			}
			else
			{
				//Start the level again
				StartLevel();
			}
		}

		#endregion rUpdate
	}
}
