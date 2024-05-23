using System.Collections.Generic;


namespace HorsesAndGun
{
	enum ScreenType
	{
		// ROSS BUTTON SCREEN FOR TESTING //
		RossButtonsScreen,

		Intro,
		MainMenu,
		Options,
		StatsScreen,
		MainGame,
		EndGame,
		None
	}

	internal class ScreenManager : Singleton<ScreenManager>
	{
		Dictionary<ScreenType, Screen> mScreens = new Dictionary<ScreenType, Screen>();
		ScreenType mActiveScreen = ScreenType.None;

		public void LoadAllScreens(ContentManager content, GraphicsDeviceManager deviceManager)
		{
			mScreens.Clear();

			// ROSS BUTTON SCREEN FOR TESTING //
			LoadScreen(ScreenType.RossButtonsScreen, new RossButtonsScreen(content, deviceManager), content);

			LoadScreen(ScreenType.Intro, new IntroScreen(content, deviceManager), content);
			LoadScreen(ScreenType.MainMenu, new MainMenuScreen(content, deviceManager), content);
			LoadScreen(ScreenType.Options, new OptionsScreen(content, deviceManager), content);
			LoadScreen(ScreenType.StatsScreen, new StatsScreen(content, deviceManager), content);
			LoadScreen(ScreenType.MainGame, new MainGameScreen(content, deviceManager), content);
			LoadScreen(ScreenType.EndGame, new EndGameScreen(content, deviceManager), content);
		}

		private void LoadScreen(ScreenType type, Screen screen, ContentManager content)
		{
			mScreens.Add(type, screen);
			screen.LoadContent(content);
		}

		public void Update(GameTime gameTime)
		{
			Screen activeScreen = GetActiveScreen();

			if (!(activeScreen is null))
			{
				GetActiveScreen().Update(gameTime);
			}
		}

		public Screen GetScreen(ScreenType type)
		{
			if (mScreens.ContainsKey(type))
			{
				return mScreens[type];
			}

			return null;
		}

		public Screen GetActiveScreen()
		{
			if (mScreens.ContainsKey(mActiveScreen))
			{
				return mScreens[mActiveScreen];
			}

			return null;
		}

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
	}

	abstract class Screen
	{
		public const int SCREEN_WIDTH = 640;
		public const int SCREEN_HEIGHT = 360;

		protected ContentManager mContentManager;
		protected GraphicsDeviceManager mGraphics;
		protected RenderTarget2D mScreenTarget;

		public Screen(ContentManager content, GraphicsDeviceManager graphics)
		{
			mContentManager = content;
			mGraphics = graphics;

			mScreenTarget = new RenderTarget2D(graphics.GraphicsDevice, SCREEN_WIDTH, SCREEN_HEIGHT);
		}

		public abstract void LoadContent(ContentManager content);

		public abstract void OnActivate();

		public abstract void OnDeactivate();

		public abstract RenderTarget2D DrawToRenderTarget(DrawInfo info);

		public abstract void Update(GameTime gameTime);
	}
}
