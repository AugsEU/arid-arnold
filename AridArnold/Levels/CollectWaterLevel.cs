namespace AridArnold.Levels
{
    class CollectWaterLevel : Level
    {
        int mNumWaterNeeded;
        public CollectWaterLevel(string levelName, int numNeeded) : base(levelName)
        {
            mNumWaterNeeded = numNeeded;
        }

        public override LevelStatus Update(GameTime gameTime)
        {
            if (CollectibleManager.I.GetCollected(CollectibleType.WaterBottle) >= mNumWaterNeeded)
            {
                mLevelStatus = LevelStatus.Win;
            }

            return mLevelStatus;
        }
    }
}
