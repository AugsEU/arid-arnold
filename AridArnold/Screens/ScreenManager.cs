namespace AridArnold.Screens
{
    enum ScreenType
    {
        Title,
        LevelStart,
        Game,
        GameOver,
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

            LoadScreen(ScreenType.Game, new GameScreen(content, deviceManager), content);
            LoadScreen(ScreenType.GameOver, new GameOverScreen(content, deviceManager), content);
            LoadScreen(ScreenType.LevelStart, new StartLevelScreen(content, deviceManager), content);
            LoadScreen(ScreenType.EndGame, new EndScreen(content, deviceManager), content);
        }

        private void LoadScreen(ScreenType type, Screen screen, ContentManager content)
        {
            mScreens.Add(type, screen);
            screen.LoadContent(content);
        }         

        public Screen GetScreen(ScreenType type)
        {
            if(mScreens.ContainsKey(type))
            {
                return mScreens[type];
            }

            return null;
        }

        public Screen GetActiveScreen()
        {
            if(mScreens.ContainsKey(mActiveScreen))
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

            if(mScreens.ContainsKey(mActiveScreen))
            {
                mScreens[mActiveScreen].OnDeactivate();
            }

            mActiveScreen = type;

            mScreens[type].OnActivate();
        }
    }

    abstract class Screen
    {
        const int SCREEN_WIDTH = 960;
        const int SCREEN_HEIGHT = 540;

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
