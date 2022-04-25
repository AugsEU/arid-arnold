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
    }

    internal class ScreenManager : Singleton<ScreenManager>
    {
        List<Screen> mScreens = new List<Screen>();
        int mActiveScreen = -1;

        public void LoadScreen(Screen screen, ContentManager content)
        {
            mScreens.Add(screen);
            screen.LoadContent(content);
        }         

        public Screen GetScreen(ScreenType type)
        {
            foreach(Screen screen in mScreens)
            {
                if(screen.GetScreenType() == type)
                {   
                    return screen;
                }
            }

            return null;
        }

        public Screen GetActiveScreen()
        {
            if(mActiveScreen == -1)
            {
                return null;
            }

            return mScreens[mActiveScreen];
        }

        public void ActivateScreen(ScreenType type)
        {
            for(int i = 0; i < mScreens.Count; i++)
            {
                if(mScreens[i].GetScreenType() == type)
                {
                    mActiveScreen = i;
                    return;
                }
            }

            mActiveScreen = -1;
        }
    }

    abstract class Screen
    {
        public abstract ScreenType GetScreenType();

        public abstract void Draw(DrawInfo info);

        public abstract void Update(GameTime gameTime);

        public abstract void LoadContent(ContentManager content);
    }
}
