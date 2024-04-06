namespace AridArnold
{
	/// <summary>
	/// Program top level
	/// </summary>
	public class Main : Game
	{
		#region rConstants

		private const int FRAME_SLOWDOWN = 1;
		private const double FRAME_RATE = 60d;
		private const int MIN_HEIGHT = 550;
		private const float ASPECT_RATIO = 1.77778f;

		#endregion rConstants





		#region rMembers

		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private Rectangle windowedRect;
		private int mSlowDownCount;
		private Texture2D mDummyTexture;
		private bool mIsInputFrame = true;


		// Hack
		private static Main sSelf;

		#endregion rMembers




		#region rInitialisation

		/// <summary>
		/// Program constructor
		/// </summary>
		public Main()
		{
			//XNA
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			//XNA

			mIsInputFrame = true;

			LanguageManager.I.LoadLanguage(LanguageManager.LanguageType.English);

			// Fix to 60 fps.
			IsFixedTimeStep = true;
			TargetElapsedTime = System.TimeSpan.FromSeconds(1d / FRAME_RATE);

			Window.ClientSizeChanged += OnResize;

			mSlowDownCount = 0;

			sSelf = this;
		}





		/// <summary>
		/// Init program
		/// </summary>
		protected override void Initialize()
		{
			SetWindowHeight(MIN_HEIGHT);
			_graphics.IsFullScreen = false;
			_graphics.ApplyChanges();

			Window.AllowUserResizing = true;
			Window.Title = "Arid Arnold";

			base.Initialize();
		}



		/// <summary>
		/// Load content for everything
		/// </summary>
		protected override void LoadContent()
		{
			MonoData.I.Init(Content);
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			FontManager.I.LoadAllFonts();
			ScreenManager.I.LoadAllScreens(_graphics);
			GhostManager.I.Load();
			InputManager.I.Init();
			CameraManager.I.Init();

			mDummyTexture = new Texture2D(GraphicsDevice, 1, 1);
			mDummyTexture.SetData(new Color[] { Color.White });

			LoadGlobalContent();

			// Temp
			CampaignManager.I.LoadCampaign("MainCampaign");
			ScreenManager.I.ActivateScreen(ScreenType.Game);
		}

		/// <summary>
		/// Load content storred globally
		/// </summary>
		void LoadGlobalContent()
		{
			SpeechBoxRenderer.LoadContent();
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update game. It updates in fixed intervals, even if the frame time was way longer.
		/// </summary>
		/// <param name="gameTime"></param>
		protected override void Update(GameTime gameTime)
		{
			gameTime.ElapsedGameTime = TargetElapsedTime;

			mSlowDownCount = (mSlowDownCount + 1) % FRAME_SLOWDOWN;
			if (mSlowDownCount == 0)
			{
				//Record elapsed time
				TimeManager.I.Update(gameTime);
				CameraManager.I.UpdateAllCameras(gameTime);

				KeyboardState keyboardState = Keyboard.GetState();
				foreach (Keys key in keyboardState.GetPressedKeys())
				{
					HandleKeyPress(key);
				}

				Screen screen = ScreenManager.I.GetActiveScreen();
				InputManager.I.Update(gameTime);

				if (screen is not null)
				{
					screen.Update(gameTime);
				}
			}

			base.Update(gameTime);
		}



		/// <summary>
		/// Handle key press
		/// </summary>
		/// <param name="key">Keys pressed</param>
		private void HandleKeyPress(Keys key)
		{
			KeyboardState keyboardState = Keyboard.GetState();

			bool alt = keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt);
			if (key == Keys.Enter && alt)
			{
				ToggleFullscreen();
			}

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
			{
				Exit();
			}
		}



		/// <summary>
		/// Enter/leave full screen
		/// </summary>
		private void ToggleFullscreen()
		{
			if (_graphics.IsFullScreen)
			{
				_graphics.IsFullScreen = false;
				_graphics.PreferredBackBufferWidth = windowedRect.Width;
				_graphics.PreferredBackBufferHeight = windowedRect.Height;
			}
			else
			{
				windowedRect = GraphicsDevice.PresentationParameters.Bounds;
				_graphics.IsFullScreen = true;

				_graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
				_graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
			}

			_graphics.ApplyChanges();
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw main game with the largest possible integer scaling
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		protected override void Draw(GameTime gameTime)
		{
			DrawInfo frameInfo;

			frameInfo.graphics = _graphics;
			frameInfo.spriteBatch = _spriteBatch;
			frameInfo.gameTime = gameTime;
			frameInfo.device = GraphicsDevice;

			//Draw active screen.
			Screen screen = ScreenManager.I.GetActiveScreen();

			if (screen != null)
			{
				RenderTarget2D screenTargetRef = screen.DrawToRenderTarget(frameInfo);
				Rectangle screenRect = GraphicsDevice.PresentationParameters.Bounds;
				Camera screenCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.GlobalCamera);

				// Draw!
				screenCam.StartSpriteBatch(frameInfo, new Vector2(screenRect.X, screenRect.Y), null, Color.Black);

				DrawScreenPixelPerfect(frameInfo, screenTargetRef);

				screenCam.EndSpriteBatch(frameInfo);
			}

			base.Draw(gameTime);

			MonoDraw.FlushRender();
		}



		/// <summary>
		/// Draw render target with the largest possible integer scaling
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		/// <param name="screen">Screen to draw</param>
		private void DrawScreenPixelPerfect(DrawInfo info, RenderTarget2D screen)
		{
			Rectangle screenRect = info.device.PresentationParameters.Bounds;

			int multiplier = (int)MathF.Min(screenRect.Width / screen.Width, screenRect.Height / screen.Height);

			int finalWidth = screen.Width * multiplier;
			int finalHeight = screen.Height * multiplier;

			Rectangle destRect = new Rectangle((screenRect.Width - finalWidth) / 2, (screenRect.Height - finalHeight) / 2, finalWidth, finalHeight);

			MonoDraw.DrawTexture(info, screen, destRect);
		}



		/// <summary>
		/// Callback for re-sizing the screen
		/// </summary>
		/// <param name="sender">Sender of this event</param>
		/// <param name="eventArgs">Event args</param>
		private void OnResize(object sender, EventArgs eventArgs)
		{
			if (_graphics.IsFullScreen)
			{
				return;
			}

			int min_width = (int)(ASPECT_RATIO * MIN_HEIGHT);

			if (Window.ClientBounds.Height >= MIN_HEIGHT && Window.ClientBounds.Width >= min_width)
			{
				return;
			}
			else
			{
				SetWindowHeight(MIN_HEIGHT);
			}
		}


		/// <summary>
		/// Set window height so it keeps the aspect ratio
		/// </summary>
		/// <param name="height">New window height</param>
		private void SetWindowHeight(int height)
		{
			_graphics.PreferredBackBufferWidth = (int)(height * ASPECT_RATIO);
			_graphics.PreferredBackBufferHeight = height;
			_graphics.ApplyChanges();
		}

		#endregion rDraw



		#region rUtility

		/// <summary>
		/// Get the graphics device
		/// </summary>
		public static GraphicsDevice GetGraphicsDevice()
		{
			return sSelf.GraphicsDevice;
		}


		/// <summary>
		/// Get a dummy white texture.
		/// </summary>
		public static Texture2D GetDummyTexture()
		{
			return sSelf.mDummyTexture;
		}


		/// <summary>
		/// Get content manager.
		/// </summary>
		public static ContentManager GetMainContentManager()
		{
			return sSelf.Content;
		}



		/// <summary>
		/// Signal the start of a loading screen.
		/// </summary>
		public static void LoadingScreenBegin()
		{
			sSelf.IsFixedTimeStep = false;
			sSelf.mIsInputFrame = false;
		}



		/// <summary>
		/// Signal the end of a loading screen.
		/// </summary>
		public static void LoadingScreenEnd()
		{
			sSelf.IsFixedTimeStep = true;
			sSelf.mIsInputFrame = true;
		}

		#endregion rUtility
	}
}
