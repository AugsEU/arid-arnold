using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold
{
    internal class EntityManager : Singleton<EntityManager>
    {
        private List<Entity> mRegisteredEntities = new List<Entity>();


        public void RegisterEntity(Entity entity, ContentManager content)
        {
            mRegisteredEntities.Add(entity);
            entity.LoadContent(content);
        }

        public void DeleteEntity(Entity entity)
        {
            mRegisteredEntities.Remove(entity);
        }

        public void ClearEntities()
        {
            mRegisteredEntities.Clear();
        }

        public void Update(GameTime gameTime)
        {
            foreach (Entity entity in mRegisteredEntities)
            {
                entity.Update(gameTime);
            }
        }

        public int GetEntityNum()
        {
            return mRegisteredEntities.Count;
        }

        public Entity GetEntity(int index)
        {
            return mRegisteredEntities[index];
        }

        public void Draw(DrawInfo info)
        {
            foreach (Entity entity in mRegisteredEntities)
            {
                entity.Draw(info);
            }
        }
    }
}
