using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using AridArnold.Levels;

namespace AridArnold.Screens
{
    internal class GameScreen : Screen
    {
        private List<Level> mLevels;
        private int mCurrentLevel;

        public override ScreenType GetScreenType()
        {
            return ScreenType.Game;
        }

        public GameScreen(ContentManager content, GraphicsDeviceManager graphics) : base(content, graphics)
        {
            mLevels = new List<Level>();
            mLevels.Add(new CollectWaterLevel("level1-1", 5));
            mLevels.Add(new CollectWaterLevel("level1-2", 5));

            LoadLevel(0);
        }

        private void LoadLevel(int levelIndex)
        {
            mCurrentLevel = levelIndex;
            mLevels[levelIndex].Begin(mContentManager);

            TileManager.I.CentreX(mGraphics);
        }

        public override void LoadContent()
        {
        }

        public override void Draw(DrawInfo info)
        {
            EntityManager.I.Draw(info);
            TileManager.I.Draw(info);
        }

        public override void Update(GameTime gameTime)
        {
            EntityManager.I.Update(gameTime);

            LevelStatus status = mLevels[mCurrentLevel].Update(gameTime);

            if(status == LevelStatus.Win)
            {
                LoadLevel(mCurrentLevel + 1);
            }
            else if(status == LevelStatus.Loss)
            {
                LoadLevel(mCurrentLevel);
            }
        }
    }
}
