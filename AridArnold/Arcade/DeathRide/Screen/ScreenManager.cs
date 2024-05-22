namespace GMTK2023
{
	/// <summary>
	/// Enumerator to store the screen type. Each screen needs a new entry into this
	/// </summary>
	enum ScreenType
	{
		Title,
		Game,
		GameOver,
		NextRound,
		Tutorial,
		None
	}

	/// <summary>
	/// Class that manages all screens
	/// </summary>
	internal class ScreenManager : Singleton<ScreenManager>
	{
		#region rMembers

		Dictionary<ScreenType, Screen> mScreens = new Dictionary<ScreenType, Screen>();
		ScreenType mActiveScreen = ScreenType.None;

		#endregion rMembers





		#region rInitialise

		/// <summary>
		/// Load all the screens, but don't start them.
		/// </summary>
		/// <param name="deviceManager">Graphics device</param>
		public void LoadAllScreens(GraphicsDeviceManager deviceManager)
		{
			mScreens.Clear();

			LoadScreen(ScreenType.Game, new GameScreen(deviceManager));
			LoadScreen(ScreenType.Title, new TitleScreen(deviceManager));
			LoadScreen(ScreenType.NextRound, new NextRoundScreen(deviceManager));
			LoadScreen(ScreenType.GameOver, new GameOverScreen(deviceManager));
			LoadScreen(ScreenType.Tutorial, new TutorialScreen(deviceManager));
		}



		/// <summary>
		/// Load a screen of a specific type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="screen"></param>
		private void LoadScreen(ScreenType type, Screen screen)
		{
			mScreens.Add(type, screen);
			screen.LoadContent();
		}

		#endregion rInitialise





		#region rUtility

		/// <summary>
		/// Get a screen of a certain type
		/// </summary>
		/// <param name="type">Screen type you want to find.</param>
		/// <returns>Screen of that type, null if that type doesn't exist</returns>
		public Screen GetScreen(ScreenType type)
		{
			if (mScreens.ContainsKey(type))
			{
				return mScreens[type];
			}

			return null;
		}



		/// <summary>
		/// Get the currently active screen
		/// </summary>
		/// <returns>Active screen refernece, null if there is none.</returns>
		public Screen GetActiveScreen()
		{
			if (mScreens.ContainsKey(mActiveScreen))
			{
				return mScreens[mActiveScreen];
			}

			return null;
		}



		/// <summary>
		/// Activates a screen of a certain type
		/// </summary>
		/// <param name="type">Screen type you want to actiavet</param>
		public void ActivateScreen(ScreenType type)
		{
			if (!mScreens.ContainsKey(type))
			{
				return;
			}

			if (mScreens.ContainsKey(mActiveScreen))
			{
				mScreens[mActiveScreen].OnDeactivate();
			}

			mActiveScreen = type;

			mScreens[type].OnActivate();
		}

		#endregion rUtility
	}

	abstract class Screen
	{
		#region rConstants

		public const int SCREEN_WIDTH = 960;
		public const int SCREEN_HEIGHT = 505;

		#endregion rConstants





		#region rMembers

		protected GraphicsDeviceManager mGraphics;
		protected RenderTarget2D mScreenTarget;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Screen constructor
		/// </summary>
		/// <param name="graphics">Graphics device manager</param>
		public Screen(GraphicsDeviceManager graphics)
		{
			mGraphics = graphics;
			mScreenTarget = new RenderTarget2D(graphics.GraphicsDevice, SCREEN_WIDTH, SCREEN_HEIGHT);
		}



		/// <summary>
		/// Load content for this screen
		/// </summary>
		public virtual void LoadContent() { }



		/// <summary>
		/// Called when the screen is activated
		/// </summary>
		public virtual void OnActivate() { }


		/// <summary>
		/// Called when the screen is deactivated
		/// </summary>
		public virtual void OnDeactivate() { }

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update the screen
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public abstract void Update(GameTime gameTime);

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the screen to a render target
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		/// <returns>Render target with the screen drawn on</returns>
		public abstract RenderTarget2D DrawToRenderTarget(DrawInfo info);



		/// <summary>
		/// Start sprite batch for this screen
		/// </summary>
		/// <param name="info"></param>
		protected void StartScreenSpriteBatch(DrawInfo info)
		{
			CameraManager.I.GetCamera(CameraManager.CameraInstance.ScreenCamera).StartSpriteBatch(info, new Vector2(SCREEN_WIDTH, SCREEN_HEIGHT));
		}



		/// <summary>
		/// Sprite screen sprite batch.
		/// </summary>
		/// <param name="info"></param>
		protected void EndScreenSpriteBatch(DrawInfo info)
		{
			CameraManager.I.GetCamera(CameraManager.CameraInstance.ScreenCamera).EndSpriteBatch(info);
		}

		#endregion rDraw
	}
}
