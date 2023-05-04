namespace AridArnold
{
	/// <summary>
	/// Level where you collect a flag to win.
	/// </summary>
	class CollectKeyLevel : Level
	{
		#region rInitialisation

		/// <summary>
		/// Collect flag level construct
		/// </summary>
		public CollectKeyLevel(AuxData auxData, int id) : base(auxData, id)
		{
			EventManager.I.AddListener(EventType.KeyCollect, KeyCollectCallback);
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
			return mLevelStatus;
		}



		/// <summary>
		/// Called when a key is collected
		/// </summary>
		void KeyCollectCallback(EArgs eventArgs)
		{
			mLevelStatus = LevelStatus.Win;
		}

		#endregion rUpdate
	}
}