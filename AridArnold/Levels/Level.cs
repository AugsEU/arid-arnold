using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold.Levels
{
    enum LevelStatus
    {
        Continue,
        Win,
        Loss,
    }

    abstract class Level
    {
        string mName;

        protected LevelStatus mLevelStatus;

        public Level(string levelName)
        {
            mName = levelName;
            EventManager.I.AddListener(EventType.PlayerDead, HandlePlayerDeath);
        }

        public void Begin(ContentManager content)
        {
            mLevelStatus = LevelStatus.Continue;
            TileManager.I.LoadLevel(content, "Levels/" + mName);
        }

        public virtual void HandlePlayerDeath(EArgs args)
        {
            mLevelStatus = LevelStatus.Loss;
        }

        public abstract LevelStatus Update(GameTime gameTime);
    }


}
