using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold.Screens
{
    enum ScreenType
    {
        Title,
        LevelStart,
        Game,
        GameOver,
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
        protected ContentManager mContentManager;
        protected GraphicsDeviceManager mGraphics;
        public Screen(ContentManager content, GraphicsDeviceManager graphics)
        {
            mContentManager = content;
            mGraphics = graphics;
        }

        public abstract void Draw(DrawInfo info);

        public abstract void Update(GameTime gameTime);

        public abstract void OnActivate();

        public abstract void OnDeactivate();

        public abstract void LoadContent(ContentManager content);
    }
}
