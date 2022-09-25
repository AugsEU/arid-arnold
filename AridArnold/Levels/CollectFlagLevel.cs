namespace AridArnold.Levels
{
	/// <summary>
	/// Level where you collect a flag to win.
	/// </summary>
	class CollectFlagLevel : Level
	{
		#region rInitialisation

		/// <summary>
		/// Collect flag level construct
		/// </summary>
		/// <param name="levelName">Name of the level</param>
		public CollectFlagLevel(string levelName) : base(levelName)
		{
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update, checks for win condition
		/// </summary>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public override LevelStatus Update(GameTime gameTime)
		{
			if (CollectableManager.I.GetCollected(CollectableType.Flag) > 0)
			{
				mLevelStatus = LevelStatus.Win;
			}

			return mLevelStatus;
		}

		#endregion rUpdate
	}
}