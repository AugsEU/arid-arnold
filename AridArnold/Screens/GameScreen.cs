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
        private Arnold mArnold;

        private List<Entity> mRegisteredEntities;

        private List<Level> mLevels;
        private int mCurrentLevel;

        public override ScreenType GetScreenType()
        {
            return ScreenType.Game;
        }

        public GameScreen(ContentManager content) : base(content)
        {
            mArnold = new Arnold();

            mRegisteredEntities = new List<Entity>();
            RegisterDefaultEntities();

            mLevels = new List<Level>();
            mLevels.Add(new CollectWaterLevel("testlevel", 5));

            LoadLevel(0);
        }

        private void LoadLevel(int levelIndex)
        {
            mCurrentLevel = levelIndex;
            mLevels[levelIndex].Begin(mContentManager);
        }

        public override void LoadContent()
        {
            TileManager.I.LoadContent(mContentManager);

            foreach (Entity entity in mRegisteredEntities)
            {
                entity.LoadContent(mContentManager);
            }
        }

        public override void Draw(DrawInfo info)
        {
            foreach (Entity entity in mRegisteredEntities)
            {
                entity.Draw(info);
            }

            TileManager.I.DrawCentredX(info);

        }

        public override void Update(GameTime gameTime)
        {
            foreach (Entity entity in mRegisteredEntities)
            {
                entity.Update(gameTime);
            }
        }

        private void RegisterEntity(Entity entity)
        {
            mRegisteredEntities.Add(entity);
        }

        private void RegisterDefaultEntities()
        {
            RegisterEntity(mArnold);
        }
    }
}
