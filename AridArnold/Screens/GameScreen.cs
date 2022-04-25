using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold.Screens
{
    internal class GameScreen : Screen
    {
        private Arnold mArnold;

        private List<Entity> mRegisteredEntities;

        public override ScreenType GetScreenType()
        {
            return ScreenType.Game;
        }

        public GameScreen()
        {
            mArnold = new Arnold();

            mRegisteredEntities = new List<Entity>();
            RegisterDefaultEntities();
        }

        public override void LoadContent(ContentManager content)
        {
            TileManager.I.LoadContent(content);

            foreach (Entity entity in mRegisteredEntities)
            {
                entity.LoadContent(content);
            }

            TileManager.I.LoadLevel(content, "Levels/testlevel");
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
