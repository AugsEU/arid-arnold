namespace AridArnold
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
		public CollectFlagLevel(AuxData auxData) : base(auxData)
		{
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update, checks for win condition
		/// </summary>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		protected override LevelStatus UpdateInternal(GameTime gameTime)
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