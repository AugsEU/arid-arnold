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

        public Level(string levelName)
        {
            this.mName = levelName;
        }

        public void Begin(ContentManager content)
        {
            TileManager.I.LoadLevel(content, "Levels/" + mName);
        }

        public abstract LevelStatus Update(GameTime gameTime);
    }

    class CollectWaterLevel : Level
    {
        int mNumWaterNeeded;
        public CollectWaterLevel(string levelName, int numNeeded) : base(levelName)
        {
            mNumWaterNeeded = numNeeded;
        }

        public override LevelStatus Update(GameTime gameTime)
        {
            if(CollectibleManager.I.GetCollected(CollectibleType.WaterBottle) >= mNumWaterNeeded)
            {
                return LevelStatus.Win;
            }

            return LevelStatus.Continue;
        }
    }
}
