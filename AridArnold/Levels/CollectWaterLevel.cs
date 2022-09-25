namespace AridArnold.Levels
{
    /// <summary>
    /// Level type where you have to collect water to win.
    /// </summary>
    class CollectWaterLevel : Level
    {
		#region rMembers

		int mNumWaterNeeded;

		#endregion rMembers





		#region rInitialisation

        /// <summary>
        /// Collect water level constructor
        /// </summary>
        /// <param name="levelName">Name of the level</param>
        /// <param name="numNeeded">Number of water bottles needed to win</param>
		public CollectWaterLevel(string levelName, int numNeeded) : base(levelName)
        {
            mNumWaterNeeded = numNeeded;
        }

		#endregion rInitialisation





		#region rUpdate

        /// <summary>
        /// Update, checks for number of bottles needed to win.
        /// </summary>
        /// <param name="gameTime">Frame time</param>
        /// <returns>Level completion status</returns>
		public override LevelStatus Update(GameTime gameTime)
        {
            if (CollectibleManager.I.GetCollected(CollectibleType.WaterBottle) >= mNumWaterNeeded)
            {
                mLevelStatus = LevelStatus.Win;
            }

            return mLevelStatus;
        }

		#endregion rUpdate
	}
}
