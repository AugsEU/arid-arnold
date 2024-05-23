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
	public class MainGame : Game
	{
		const int APPLE_FREQ = 6;

		const int SCREEN_WIDTH = 1280;
		const int SCREEN_HEIGHT = 720;

		const float BG_START = 600.0f;
		const float BG_STEP = 456.0f;

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		SpriteFont mFont;
		Camera MainCamera;
		List<SnakeGame> SnakeGames = new List<SnakeGame>();
		Dictionary<string, Texture2D> tb;
		Dictionary<string, SoundEffect> SFX_db;
		List<Uptext> ScoreMarkers = new List<Uptext>();

		Song mainSong;
		Song menuSong;

		int num_apple_placed = 0;

		int current_step = -1;
		TimeSpan begin_time;
		int current_bg_frame = 0;

		bool enter_held;

		int target_width = 0;

		int curr_score;
		int high_score;

		Vector2 BGpos;



		GameState CurrentState;
		float GameOverOpacity = 0.0f;

		public MainGame()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.IsFullScreen = false;

			graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
			graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
			graphics.ApplyChanges();
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			MainCamera = new Camera(0f, 0f);
			base.Initialize();
			high_score = 0;
			enter_held = Keyboard.GetState().IsKeyDown(Keys.Enter);
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
			tb = LoadContent<Texture2D>("Textures");
			SFX_db = LoadContent<SoundEffect>("SFX");

			mainSong = Content.Load<Song>("Music/GJ Main");
			menuSong = Content.Load<Song>("Music/GameJam21");

			mFont = Content.Load<SpriteFont>("Fonts/PixelFont");

			InitMenu();
		}

		private Dictionary<string, T> LoadContent<T>(string contentFolder)
		{
			//Load directory info, abort if none
			DirectoryInfo dir = new DirectoryInfo(Content.RootDirectory + "\\" + contentFolder);
			if (!dir.Exists)
				throw new DirectoryNotFoundException();
			//Init the resulting list
			Dictionary<string, T> result = new Dictionary<string, T>();

			//Load all files that matches the file filter
			FileInfo[] files = dir.GetFiles("*.*");
			foreach (FileInfo file in files)
			{
				string key = Path.GetFileNameWithoutExtension(file.Name);

				result[key] = Content.Load<T>(contentFolder + "/" + key);
			}
			//Return the result
			return result;
		}

		private Dictionary<string, T> LoadContent<T>()
		{
			//Load directory info, abort if none
			DirectoryInfo dir = new DirectoryInfo(Content.RootDirectory + "\\");
			if (!dir.Exists)
				throw new DirectoryNotFoundException();
			//Init the resulting list
			Dictionary<string, T> result = new Dictionary<string, T>();

			//Load all files that matches the file filter
			FileInfo[] files = dir.GetFiles("*.*");
			foreach (FileInfo file in files)
			{
				string key = Path.GetFileNameWithoutExtension(file.Name);

				result[key] = Content.Load<T>(key);
			}
			//Return the result
			return result;
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		void PlaySFX(SFX_TYPE type)
		{
			switch (type)
			{
				case SFX_TYPE.ST_PICKUP:
					SFX_db["PointGetSFX"].Play(0.15f, 0.0f, 0.0f);
					break;
				case SFX_TYPE.ST_DIMENSION:
					SFX_db["NewDimensionSFX"].Play(0.2f, 0.0f, 0.0f);
					break;
				case SFX_TYPE.ST_GAMEOVER:
					SFX_db["GameOverSFX"].Play(0.7f, 0.0f, 0.0f);
					break;
			}
		}

		void InitNewMutiverse()
		{
			SnakeGames.Clear();

			//Add first snake game
			AddNewSnakeGame();
			num_apple_placed = 0;
			AddNewApple();

			//Start music
			MediaPlayer.Stop();
			MediaPlayer.Volume = 0.15f;
			MediaPlayer.Play(mainSong);
			MediaPlayer.IsRepeating = true;

			CurrentState = GameState.GS_MAIN;

			if (high_score < curr_score)
			{
				high_score = curr_score;
			}
			curr_score = 0;
			current_step = -1;
		}

		void InitMenu()
		{
			BGpos = new Vector2(0.0f, BG_START);
			ScoreMarkers.Clear();
			GameOverOpacity = 0.0f;

			CurrentState = GameState.GS_START;

			//Start music
			MediaPlayer.Stop();
			MediaPlayer.Volume = 0.2f;
			MediaPlayer.Play(menuSong);
			MediaPlayer.IsRepeating = true;
		}

		void InitPan(GameTime start)
		{
			CurrentState = GameState.GS_PAN_DOWN;
			begin_time = start.TotalGameTime;
		}

		void InitGameOver()
		{
			CurrentState = GameState.GS_GAMEOVER;
			MediaPlayer.Stop();
		}
		void AddNewApple()
		{
			int universe_to_add = num_apple_placed % SnakeGames.Count;

			AppleType type = AppleType.AT_NORMAL;
			if (num_apple_placed % APPLE_FREQ == 1)
			{
				type = AppleType.AT_DIMENSION;
			}

			SnakeGames[universe_to_add].PlaceApple(type);

			num_apple_placed++;
		}

		void AddNewSnakeGame()
		{
			Vector2 StartPos;
			if (SnakeGames.Count == 0)
			{
				StartPos = new Vector2(320, 70);
				target_width = 100;
			}
			else
			{
				StartPos = SnakeGames[SnakeGames.Count - 1].CurrPos;
			}
			SnakeGames.Add(new SnakeGame(StartPos, target_width, tb["SnakeHead"], tb["SnakeBody"], tb["SnakeCorner"], tb["SnakeTail"], tb["GrassTile"], new Texture2D[] { tb["AppleNormal"], tb["AppleDimension"] }, tb["SnakeDead"]));
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			if (Keyboard.GetState().IsKeyDown(Keys.Enter) && (Keyboard.GetState().IsKeyDown(Keys.LeftAlt) || Keyboard.GetState().IsKeyDown(Keys.RightAlt)))
			{
				graphics.ToggleFullScreen();
			}

			if (CurrentState == GameState.GS_START)
			{
				current_bg_frame = (int)Math.Floor(gameTime.TotalGameTime.TotalMilliseconds / 500.0) % 2;
				if (Keyboard.GetState().IsKeyDown(Keys.Space))
				{
					if (!enter_held)
					{
						InitPan(gameTime);
					}
				}
				else if (Keyboard.GetState().IsKeyUp(Keys.Space))
				{
					enter_held = false;
				}
			}
			else if (CurrentState == GameState.GS_PAN_DOWN)
			{
				current_bg_frame = (int)Math.Floor(gameTime.TotalGameTime.TotalMilliseconds / 250.0) % 2;
				float secFraq = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

				MediaPlayer.Volume = Math.Max(0.0f, MediaPlayer.Volume - 0.1f * secFraq);

				BGpos.Y = Math.Max(0.0f, BGpos.Y - BG_STEP * secFraq);

				if (MediaPlayer.Volume == 0.0f && BGpos.Y == 0.0f)
				{
					InitNewMutiverse();
				}
			}
			else if (CurrentState == GameState.GS_MAIN)
			{
				if (current_step == -1)
				{
					begin_time = gameTime.TotalGameTime;
					current_step = 0;
				}

				current_step = (int)Math.Floor((gameTime.TotalGameTime.TotalMilliseconds - begin_time.TotalMilliseconds) / 500.0);
				current_bg_frame = (int)Math.Floor((gameTime.TotalGameTime.TotalMilliseconds - begin_time.TotalMilliseconds) / 250.0) % 2;

				SFX_TYPE sfxToPlay = SFX_TYPE.ST_NONE;
				for (int i = 0; i < SnakeGames.Count; i++)
				{
					HandleUpdateResult(SnakeGames[i].Update(current_step), i, ref sfxToPlay);
				}

				PlaySFX(sfxToPlay);
			}
			else if (CurrentState == GameState.GS_GAMEOVER)
			{
				GameOverOpacity = Math.Min(GameOverOpacity + 0.2f * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f), 0.6f);
				if (Keyboard.GetState().IsKeyDown(Keys.Space))
				{
					InitMenu();
					enter_held = true;
				}
			}

			for (int i = 0; i < ScoreMarkers.Count; i++)
			{
				ScoreMarkers[i].Update(gameTime);
				if (ScoreMarkers[i].deleteMe())
				{
					ScoreMarkers.RemoveAt(i);
					i--;
				}
			}

			base.Update(gameTime);
		}

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
					AddScore(10 * SnakeGames.Count, Color.MediumVioletRed, idx);
					AddNewSnakeGame();
					AddNewApple();
					break;
				case UpdateResult.UR_APPLE_EATEN:
					if (SFX_TYPE.ST_PICKUP > sfx)
					{
						sfx = SFX_TYPE.ST_PICKUP;
					}
					AddScore(5 * SnakeGames.Count, Color.Wheat, idx);
					AddNewApple();
					break;
			}
		}

		void AddScore(int numpts, Color col, int i)
		{
			ScoreMarkers.Add(new Uptext(SnakeGames[i].GetSnakeHeadPos(), "+" + numpts.ToString(), col));
			curr_score += numpts;
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			// TODO: Add your drawing code here
			spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Matrix.CreateScale(MainCamera.Zoom));

			const int TOP_PAD = 90;
			const int BORDER = 15;
			const int SPACING = 15;
			const int PLAYABLE_WIDTH = SCREEN_WIDTH - 2 * BORDER;
			const int PLAYABLE_HEIGHT = SCREEN_HEIGHT - BORDER - TOP_PAD;
			Vector2 TL_OFFSET = new Vector2(BORDER, TOP_PAD);

			Rectangle fullScreen = new Rectangle((int)BGpos.X, (int)BGpos.Y, SCREEN_WIDTH, SCREEN_HEIGHT);

			if (current_bg_frame == 0)
			{
				spriteBatch.Draw(tb["SnakeBackground"], fullScreen, Color.White);
			}
			else
			{
				spriteBatch.Draw(tb["SnakeBackgroundAnimated"], fullScreen, Color.White);
			}

			if (CurrentState == GameState.GS_START || CurrentState == GameState.GS_PAN_DOWN)
			{
				DrawString("Worm Warp", new Vector2(60.0f, 60.0f + (BGpos.Y - BG_START)), Color.Wheat, 1.0f, false);

				DrawString("Press space to start...", new Vector2(760.0f, 560.0f + (BGpos.Y - BG_START)), Color.Wheat, 0.5f, false);
			}
			else if (CurrentState == GameState.GS_MAIN || CurrentState == GameState.GS_GAMEOVER)
			{
				int num_col = 1;
				int num_rows = 0;
				int num_games = SnakeGames.Count;

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
				target_width = ViewSize;
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
							SnakeGames[g].Draw(spriteBatch, MainCamera, new Vector2(x * GameSize + last_row_buffer + SPACING, y * GameSize + vertical_buffer) + TL_OFFSET, ViewSize, ViewSize);
						}
						else
						{
							SnakeGames[g].Draw(spriteBatch, MainCamera, new Vector2(x * GameSize + SPACING, y * GameSize + vertical_buffer) + TL_OFFSET, ViewSize, ViewSize);
						}


						g++;
					}
				}

				for (int i = 0; i < ScoreMarkers.Count; i++)
				{
					ScoreMarkers[i].Draw(ref spriteBatch, ref mFont);
				}

				if (CurrentState != GameState.GS_GAMEOVER)
				{
					DrawString("Score: " + curr_score, new Vector2(35.0f, 15.0f), Color.Wheat, 0.8f, false);
				}
			}
			if (CurrentState == GameState.GS_GAMEOVER)
			{
				Rectangle ScreenBlank = new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
				Color blankCol = new Color(255, 0, 0, 0);

				DrawRect(ScreenBlank, blankCol, GameOverOpacity);

				DrawString("GAME OVER!", new Vector2(SCREEN_WIDTH / 2.0f, TOP_PAD * 0.94f), Color.Wheat, 1.5f, true);
				DrawString("Final Score:" + curr_score, new Vector2(SCREEN_WIDTH / 2.0f, 1.45f * TOP_PAD), Color.Wheat, 0.5f, true);

				if (curr_score > high_score)
				{
					DrawString("New high score!", new Vector2(SCREEN_WIDTH / 2.0f, 1.7f * TOP_PAD), Color.HotPink, 0.5f, true);
				}
				else
				{
					DrawString("High Score:" + high_score, new Vector2(SCREEN_WIDTH / 2.0f, 1.7f * TOP_PAD), Color.Wheat, 0.5f, true);
				}

				DrawString("Press space to continue...", new Vector2(SCREEN_WIDTH / 2.0f, SCREEN_HEIGHT - TOP_PAD * 0.7f), Color.Wheat, 0.5f, true);
			}


			spriteBatch.End();
			base.Draw(gameTime);
		}

		public void DrawString(string text, Vector2 pos, Color color, float scale, bool center)
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


		void DrawRect(Rectangle rectangle, Color C, float opacity)
		{
			Texture2D transitionTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			transitionTexture.SetData(new Color[] { Color.Black });

			spriteBatch.Draw(transitionTexture, rectangle, Color.Black * opacity);
		}
	}
}
