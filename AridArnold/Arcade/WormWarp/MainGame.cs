namespace WormWarp
{
	enum GameState
	{
		GS_START,
		GS_PAN_DOWN,
		GS_MAIN,
		GS_GAMEOVER
	}

	enum SFX_TYPE
	{
		ST_NONE = -1,
		ST_PICKUP = 0,
		ST_DIMENSION,
		ST_GAMEOVER
	}

	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class MainGame
	{
		#region rConstants

		const int APPLE_FREQ = 6;

		public const int SCREEN_WIDTH = 1280;
		public const int SCREEN_HEIGHT = 720;

		const float BG_START = 600.0f;
		const float BG_STEP = 456.0f;

		#endregion rConstants





		#region rMembers

		SpriteFont mFont;
		Camera mMainCamera;
		List<SnakeGame> mSnakeGames = new List<SnakeGame>();
		Dictionary<string, Texture2D> mTextureDB;
		Dictionary<string, SoundEffect> mSoundDB;
		List<Uptext> mScoreMarkers = new List<Uptext>();

		Song mMainSong;
		Song mMenuSong;

		int mNumApplesPlaced = 0;

		int mCurrentStep = -1;
		TimeSpan mBeginTime;
		int mCurrentBGFrame = 0;

		int mTargetWidth = 0;

		int mCurrScore;
		int mHighScore;

		Vector2 mBGPos;

		GameState mGameState;
		float mGameOverOpacity = 0.0f;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create game
		/// </summary>
		public MainGame()
		{
		}



		/// <summary>
		/// Reset game state
		/// </summary>
		public void ResetGame()
		{
			mMainCamera = new Camera(0f, 0f);
			mHighScore = 0;
			mCurrScore = 0;
			InitMenu();
		}



		/// <summary>
		/// Load all textures
		/// </summary>
		public void LoadContent(ContentManager content)
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			mTextureDB = LoadTypeDB<Texture2D>(content, "Textures");
			mSoundDB = LoadTypeDB<SoundEffect>(content, "SFX");

			mMainSong = content.Load<Song>("Music/GJ Main");
			mMenuSong = content.Load<Song>("Music/GameJam21");

			mFont = content.Load<SpriteFont>("Fonts/PixelFont");

			InitMenu();
		}



		/// <summary>
		/// Load files of a type into a dictionary
		/// </summary>
		private Dictionary<string, T> LoadTypeDB<T>(ContentManager content, string contentFolder)
		{
			//Load directory info, abort if none
			DirectoryInfo dir = new DirectoryInfo(content.RootDirectory + "\\" + contentFolder);
			if (!dir.Exists)
				throw new DirectoryNotFoundException();

			//Init the resulting list
			Dictionary<string, T> result = new Dictionary<string, T>();

			//Load all files that matches the file filter
			FileInfo[] files = dir.GetFiles("*.*");
			foreach (FileInfo file in files)
			{
				string key = Path.GetFileNameWithoutExtension(file.Name);

				result[key] = content.Load<T>(contentFolder + "/" + key);
			}

			//Return the result
			return result;
		}



		/// <summary>
		/// Begin game from start
		/// </summary>
		void InitNewMutiverse()
		{
			mSnakeGames.Clear();

			//Add first snake game
			AddNewSnakeGame();
			mNumApplesPlaced = 0;
			AddNewApple();

			//Start music
			MediaPlayer.Stop();
			MediaPlayer.Volume = 0.15f;
			MediaPlayer.Play(mMainSong);
			MediaPlayer.IsRepeating = true;

			mGameState = GameState.GS_MAIN;

			if (mHighScore < mCurrScore)
			{
				mHighScore = mCurrScore;
			}
			mCurrScore = 0;
			mCurrentStep = -1;
		}



		/// <summary>
		/// Set state to be in the main menu
		/// </summary>
		void InitMenu()
		{
			mBGPos = new Vector2(0.0f, BG_START);
			mScoreMarkers.Clear();
			mGameOverOpacity = 0.0f;

			mGameState = GameState.GS_START;

			//Start music
			MediaPlayer.Stop();
			MediaPlayer.Volume = 0.2f;
			MediaPlayer.Play(mMenuSong);
			MediaPlayer.IsRepeating = true;
		}



		/// <summary>
		/// Set state to be in the panning
		/// </summary>
		void InitPan(GameTime start)
		{
			mGameState = GameState.GS_PAN_DOWN;
			mBeginTime = start.TotalGameTime;
		}



		/// <summary>
		/// Set state to be in game over
		/// </summary>
		void InitGameOver()
		{
			mGameState = GameState.GS_GAMEOVER;
			MediaPlayer.Stop();
		}

		#endregion rInit





		#region rSound

		/// <summary>
		/// Play a sound effect
		/// </summary>
		void PlaySFX(SFX_TYPE type)
		{
			switch (type)
			{
				case SFX_TYPE.ST_PICKUP:
					mSoundDB["PointGetSFX"].Play(0.15f, 0.0f, 0.0f);
					break;
				case SFX_TYPE.ST_DIMENSION:
					mSoundDB["NewDimensionSFX"].Play(0.2f, 0.0f, 0.0f);
					break;
				case SFX_TYPE.ST_GAMEOVER:
					mSoundDB["GameOverSFX"].Play(0.7f, 0.0f, 0.0f);
					break;
			}
		}

		#endregion rSound


		#region rUpdate

		/// <summary>
		/// Add an apple
		/// </summary>
		void AddNewApple()
		{
			int universe_to_add = mNumApplesPlaced % mSnakeGames.Count;

			AppleType type = AppleType.AT_NORMAL;
			if (mNumApplesPlaced % APPLE_FREQ == 1)
			{
				type = AppleType.AT_DIMENSION;
			}

			mSnakeGames[universe_to_add].PlaceApple(type);

			mNumApplesPlaced++;
		}



		/// <summary>
		/// Create another simultaneous game of snake
		/// </summary>
		void AddNewSnakeGame()
		{
			Vector2 StartPos;
			if (mSnakeGames.Count == 0)
			{
				StartPos = new Vector2(320, 70);
				mTargetWidth = 100;
			}
			else
			{
				StartPos = mSnakeGames[mSnakeGames.Count - 1].CurrPos;
			}
			mSnakeGames.Add(new SnakeGame(StartPos, mTargetWidth, mTextureDB["SnakeHead"], mTextureDB["SnakeBody"], mTextureDB["SnakeCorner"], mTextureDB["SnakeTail"], mTextureDB["GrassTile"], new Texture2D[] { mTextureDB["AppleNormal"], mTextureDB["AppleDimension"] }, mTextureDB["SnakeDead"]));
		}



		/// <summary>
		/// Update game
		/// </summary>
		public void Update(GameTime gameTime)
		{
			if (mGameState == GameState.GS_START)
			{
				mCurrentBGFrame = (int)Math.Floor(gameTime.TotalGameTime.TotalMilliseconds / 500.0) % 2;
				if (AridArnold.InputManager.I.KeyPressed(AridArnold.AridArnoldKeys.Confirm))
				{
					InitPan(gameTime);
				}
			}
			else if (mGameState == GameState.GS_PAN_DOWN)
			{
				mCurrentBGFrame = (int)Math.Floor(gameTime.TotalGameTime.TotalMilliseconds / 250.0) % 2;
				float secFraq = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

				MediaPlayer.Volume = Math.Max(0.0f, MediaPlayer.Volume - 0.1f * secFraq);

				mBGPos.Y = Math.Max(0.0f, mBGPos.Y - BG_STEP * secFraq);

				if (MediaPlayer.Volume == 0.0f && mBGPos.Y == 0.0f)
				{
					InitNewMutiverse();
				}
			}
			else if (mGameState == GameState.GS_MAIN)
			{
				if (mCurrentStep == -1)
				{
					mBeginTime = gameTime.TotalGameTime;
					mCurrentStep = 0;
				}

				mCurrentStep = (int)Math.Floor((gameTime.TotalGameTime.TotalMilliseconds - mBeginTime.TotalMilliseconds) / 500.0);
				mCurrentBGFrame = (int)Math.Floor((gameTime.TotalGameTime.TotalMilliseconds - mBeginTime.TotalMilliseconds) / 250.0) % 2;

				SFX_TYPE sfxToPlay = SFX_TYPE.ST_NONE;
				for (int i = 0; i < mSnakeGames.Count; i++)
				{
					HandleUpdateResult(mSnakeGames[i].Update(mCurrentStep), i, ref sfxToPlay);
				}

				PlaySFX(sfxToPlay);
			}
			else if (mGameState == GameState.GS_GAMEOVER)
			{
				mGameOverOpacity = Math.Min(mGameOverOpacity + 0.2f * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f), 0.6f);
				if (AridArnold.InputManager.I.KeyPressed(AridArnold.AridArnoldKeys.Confirm))
				{
					InitMenu();
				}
			}

			for (int i = 0; i < mScoreMarkers.Count; i++)
			{
				mScoreMarkers[i].Update(gameTime);
				if (mScoreMarkers[i].deleteMe())
				{
					mScoreMarkers.RemoveAt(i);
					i--;
				}
			}
		}



		/// <summary>
		/// Handle signals sent from snake games
		/// </summary>
		void HandleUpdateResult(UpdateResult res, int idx, ref SFX_TYPE sfx)
		{
			switch (res)
			{
				case UpdateResult.UR_NORMAL:
					return;
				case UpdateResult.UR_DEAD:
					if (SFX_TYPE.ST_GAMEOVER > sfx)
					{
						sfx = SFX_TYPE.ST_GAMEOVER;
					}
					InitGameOver();
					break;
				case UpdateResult.UR_NEW_DIMENSION:
					if (SFX_TYPE.ST_DIMENSION > sfx)
					{
						sfx = SFX_TYPE.ST_DIMENSION;
					}
					AddScore(10 * mSnakeGames.Count, Color.MediumVioletRed, idx);
					AddNewSnakeGame();
					AddNewApple();
					break;
				case UpdateResult.UR_APPLE_EATEN:
					if (SFX_TYPE.ST_PICKUP > sfx)
					{
						sfx = SFX_TYPE.ST_PICKUP;
					}
					AddScore(5 * mSnakeGames.Count, Color.Wheat, idx);
					AddNewApple();
					break;
			}
		}



		/// <summary>
		/// Add score to player
		/// </summary>
		void AddScore(int numpts, Color col, int i)
		{
			mScoreMarkers.Add(new Uptext(mSnakeGames[i].GetSnakeHeadPos(), "+" + numpts.ToString(), col));
			mCurrScore += numpts;
		}

		#endregion rUpdate


		#region rUpdate

		public void Draw(AridArnold.DrawInfo info)
		{
			const int TOP_PAD = 90;
			const int BORDER = 15;
			const int SPACING = 15;
			const int PLAYABLE_WIDTH = SCREEN_WIDTH - 2 * BORDER;
			const int PLAYABLE_HEIGHT = SCREEN_HEIGHT - BORDER - TOP_PAD;
			Vector2 TL_OFFSET = new Vector2(BORDER, TOP_PAD);

			SpriteBatch spriteBatch = info.spriteBatch;

			Rectangle fullScreen = new Rectangle((int)mBGPos.X, (int)mBGPos.Y, SCREEN_WIDTH, SCREEN_HEIGHT);

			if (mCurrentBGFrame == 0)
			{
				spriteBatch.Draw(mTextureDB["SnakeBackground"], fullScreen, Color.White);
			}
			else
			{
				spriteBatch.Draw(mTextureDB["SnakeBackgroundAnimated"], fullScreen, Color.White);
			}

			if (mGameState == GameState.GS_START || mGameState == GameState.GS_PAN_DOWN)
			{
				DrawString(spriteBatch, "Worm Warp", new Vector2(60.0f, 60.0f + (mBGPos.Y - BG_START)), Color.Wheat, 1.0f, false);

				DrawString(spriteBatch, "Press space to start...", new Vector2(760.0f, 560.0f + (mBGPos.Y - BG_START)), Color.Wheat, 0.5f, false);
			}
			else if (mGameState == GameState.GS_MAIN || mGameState == GameState.GS_GAMEOVER)
			{
				int num_col = 1;
				int num_rows = 0;
				int num_games = mSnakeGames.Count;

				float GameSize = 0.0f;

				do
				{
					GameSize = (float)PLAYABLE_WIDTH / num_col;
					num_rows = (int)Math.Ceiling(num_games / (double)num_col);

					if (num_rows * GameSize < PLAYABLE_HEIGHT)
					{
						break;
					}

					num_col++;
				} while (true);

				int ViewSize = (int)GameSize - SPACING * 2;
				mTargetWidth = ViewSize;
				int num_in_last_row = num_games - (num_rows - 1) * (num_col);
				float last_row_buffer = (float)(PLAYABLE_WIDTH - GameSize * num_in_last_row) / 2.0f;

				float vertical_buffer = (float)(PLAYABLE_HEIGHT - GameSize * num_rows) / 2.0f;
				float horizontal_buffer = (float)(PLAYABLE_WIDTH - GameSize * num_col) / 2.0f;

				int g = 0;

				for (int y = 0; y < num_rows && g < num_games; y++)
				{
					for (int x = 0; x < num_col && g < num_games; x++)
					{
						if (y == num_rows - 1)
						{
							mSnakeGames[g].Draw(spriteBatch, mMainCamera, new Vector2(x * GameSize + last_row_buffer + SPACING, y * GameSize + vertical_buffer) + TL_OFFSET, ViewSize, ViewSize);
						}
						else
						{
							mSnakeGames[g].Draw(spriteBatch, mMainCamera, new Vector2(x * GameSize + SPACING, y * GameSize + vertical_buffer) + TL_OFFSET, ViewSize, ViewSize);
						}

						g++;
					}
				}

				for (int i = 0; i < mScoreMarkers.Count; i++)
				{
					mScoreMarkers[i].Draw(ref spriteBatch, ref mFont);
				}

				if (mGameState != GameState.GS_GAMEOVER)
				{
					DrawString(spriteBatch, "Score: " + mCurrScore, new Vector2(35.0f, 15.0f), Color.Wheat, 0.8f, false);
				}
			}
			if (mGameState == GameState.GS_GAMEOVER)
			{
				Rectangle screenBlank = new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
				Color blankCol = new Color(255, 0, 0, 0);

				DrawRect(spriteBatch, screenBlank, blankCol, mGameOverOpacity);

				DrawString(spriteBatch, "GAME OVER!", new Vector2(SCREEN_WIDTH / 2.0f, TOP_PAD * 0.94f), Color.Wheat, 1.5f, true);
				DrawString(spriteBatch, "Final Score:" + mCurrScore, new Vector2(SCREEN_WIDTH / 2.0f, 1.45f * TOP_PAD), Color.Wheat, 0.5f, true);

				if (mCurrScore > mHighScore)
				{
					DrawString(spriteBatch, "New high score!", new Vector2(SCREEN_WIDTH / 2.0f, 1.7f * TOP_PAD), Color.HotPink, 0.5f, true);
				}
				else
				{
					DrawString(spriteBatch, "High Score:" + mHighScore, new Vector2(SCREEN_WIDTH / 2.0f, 1.7f * TOP_PAD), Color.Wheat, 0.5f, true);
				}

				DrawString(spriteBatch, "Press space to continue...", new Vector2(SCREEN_WIDTH / 2.0f, SCREEN_HEIGHT - TOP_PAD * 0.7f), Color.Wheat, 0.5f, true);
			}
		}



		/// <summary>
		/// Draw a string
		/// </summary>
		public void DrawString(SpriteBatch spriteBatch, string text, Vector2 pos, Color color, float scale, bool center)
		{
			if (center)
			{
				Vector2 stringSize = mFont.MeasureString(text) * 0.5f;
				//stringSize.Y = 0.0f;
				spriteBatch.DrawString(mFont, text, pos - (stringSize * scale), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
			}
			else
			{
				spriteBatch.DrawString(mFont, text, pos, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
			}
		}



		/// <summary>
		/// Draw a simple rect
		/// </summary>
		void DrawRect(SpriteBatch spriteBatch, Rectangle rectangle, Color C, float opacity)
		{
			Texture2D transitionTexture = AridArnold.Main.GetDummyTexture();
			spriteBatch.Draw(transitionTexture, rectangle, Color.Black * opacity);
		}

		#endregion rDraw
	}
}
