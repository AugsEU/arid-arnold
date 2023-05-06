namespace AridArnold
{
	/// <summary>
	/// Level where you collect a flag to win.
	/// </summary>
	class ShopLevel : Level
	{
		#region rInitialisation

		/// <summary>
		/// Collect flag level construct
		/// </summary>
		public ShopLevel(AuxData auxData, int id) : base(auxData, id)
		{
			EventManager.I.AddListener(EventType.ShopDoorOpen, ShopDoorOpenCallback);
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
		/// Called when door is opened
		/// </summary>
		void ShopDoorOpenCallback(EArgs eventArgs)
		{
			mLevelStatus = LevelStatus.Win;
		}

		#endregion rUpdate
	}
}