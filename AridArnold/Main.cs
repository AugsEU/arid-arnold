namespace AridArnold
{
	/// <summary>
	/// Program top level
	/// </summary>
	public class Main : Game
	{
		#region rTypes



		#endregion rTypes





		#region rConstants

		private const int FRAME_SLOWDOWN = 1;
		private const double FRAME_RATE = 60d;
		private const int MIN_HEIGHT = 550;
		private const float ASPECT_RATIO = 1.77778f;

		#endregion rConstants





		#region rMembers

		private GraphicsDeviceManager mGraphicsManager;
		private SpriteBatch mMainSpriteBatch;
		private Rectangle mWindowRect;
		private Rectangle mOutputRectSize;
		private int mSlowDownCount;
		private Texture2D mDummyTexture;
		private bool mInLoadingSection = true;


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
			mGraphicsManager = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			//XNA

			mInLoadingSection = false;

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
			mGraphicsManager.IsFullScreen = false;
			mGraphicsManager.ApplyChanges();

			Window.AllowUserResizing = true;
			Window.Title = "Arid Arnold";

			InputManager.I.Init();

			base.Initialize();
		}



		/// <summary>
		/// Load content for everything
		/// </summary>
		protected override void LoadContent()
		{
			MonoData.I.Init(Content);
			mMainSpriteBatch = new SpriteBatch(GraphicsDevice);

			FontManager.I.LoadAllFonts();
			ScreenManager.I.LoadAllScreens(mGraphicsManager);
			GhostManager.I.Load();
			CameraManager.I.Init();
			FlagsManager.I.Init();

			mDummyTexture = new Texture2D(GraphicsDevice, 1, 1);
			mDummyTexture.SetData(new Color[] { Color.White });

			LoadGlobalContent();

			if (BF.DEBUG_LOADER)
			{
				// Temp
				// CampaignManager.I.LoadCampaign("MainCampaign");
				ScreenManager.I.ActivateScreen(ScreenType.MainMenu);
			}
			else
			{
				ScreenManager.I.ActivateScreen(ScreenType.Title);
			}
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

		static DateTime debugNow = DateTime.Now;

		/// <summary>
		/// Update game. It updates in fixed intervals, even if the frame time was way longer.
		/// </summary>
		/// <param name="gameTime"></param>
		protected override void Update(GameTime gameTime)
		{
			//gameTime.ElapsedGameTime = TargetElapsedTime; This isn't needed with fixed time step

			TimeSpan now = DateTime.Now - debugNow;

			mSlowDownCount = (mSlowDownCount + 1) % FRAME_SLOWDOWN;
			if (mSlowDownCount == 0)
			{
				//Record elapsed time
				CameraManager.I.UpdateAllCameras(gameTime);

				KeyboardState keyboardState = Keyboard.GetState();
				foreach (Keys key in keyboardState.GetPressedKeys())
				{
					HandleKeyPress(key);
				}

				Screen screen = ScreenManager.I.GetActiveScreen();

				if (!mInLoadingSection)
				{
					InputManager.I.Update(gameTime);
				}

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
		}



		/// <summary>
		/// Enter/leave full screen
		/// </summary>
		private void ToggleFullscreen()
		{
			if (mGraphicsManager.IsFullScreen)
			{
				mGraphicsManager.IsFullScreen = false;
				mGraphicsManager.PreferredBackBufferWidth = mWindowRect.Width;
				mGraphicsManager.PreferredBackBufferHeight = mWindowRect.Height;
			}
			else
			{
				mWindowRect = GraphicsDevice.PresentationParameters.Bounds;
				mGraphicsManager.IsFullScreen = true;

				mGraphicsManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
				mGraphicsManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
			}

			mGraphicsManager.ApplyChanges();
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

			frameInfo.graphics = mGraphicsManager;
			frameInfo.spriteBatch = mMainSpriteBatch;
			frameInfo.gameTime = gameTime;
			frameInfo.device = GraphicsDevice;

			//Draw active screen.
			Screen screen = ScreenManager.I.GetActiveScreen();

			if (screen != null)
			{
				RenderTarget2D finalFrame = screen.DrawToRenderTarget(frameInfo);
				Rectangle screenRect = GraphicsDevice.PresentationParameters.Bounds;
				Camera screenCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.GlobalCamera);

				// Draw!
				screenCam.StartSpriteBatch(frameInfo, new Vector2(screenRect.X, screenRect.Y), null, Color.Black);

				Rectangle destRect = DrawScreenPixelPerfect(frameInfo, finalFrame);
				MonoDraw.DrawTexture(frameInfo, finalFrame, destRect);
				mOutputRectSize = destRect;

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
		private Rectangle DrawScreenPixelPerfect(DrawInfo info, RenderTarget2D screen)
		{
			Rectangle screenRect = info.device.PresentationParameters.Bounds;

			int multiplier = (int)MathF.Min(screenRect.Width / screen.Width, screenRect.Height / screen.Height);

			int finalWidth = screen.Width * multiplier;
			int finalHeight = screen.Height * multiplier;

			return new Rectangle((screenRect.Width - finalWidth) / 2, (screenRect.Height - finalHeight) / 2, finalWidth, finalHeight);
		}



		/// <summary>
		/// Callback for re-sizing the screen
		/// </summary>
		/// <param name="sender">Sender of this event</param>
		/// <param name="eventArgs">Event args</param>
		private void OnResize(object sender, EventArgs eventArgs)
		{
			if (mGraphicsManager.IsFullScreen)
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
			mGraphicsManager.PreferredBackBufferWidth = (int)(height * ASPECT_RATIO);
			mGraphicsManager.PreferredBackBufferHeight = height;
			mGraphicsManager.ApplyChanges();
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
		/// Get rectangle corresponding to the area where the game is actually drawn
		/// </summary>
		public static Rectangle GetGameDrawArea()
		{
			return sSelf.mOutputRectSize;
		}



		/// <summary>
		/// Signal the start of a loading screen.
		/// </summary>
		public static void LoadingScreenBegin()
		{
			sSelf.mInLoadingSection = true;
			sSelf.IsFixedTimeStep = false;
		}



		/// <summary>
		/// Signal the end of a loading screen.
		/// </summary>
		public static void LoadingScreenEnd()
		{
			sSelf.mInLoadingSection = false;
			sSelf.IsFixedTimeStep = true;
			sSelf.ResetElapsedTime();
		}



		/// <summary>
		/// Are we loading something right now?
		/// </summary>
		public static bool IsLoadingSection()
		{
			return sSelf.mInLoadingSection;
		}

		#endregion rUtility
	}
}
