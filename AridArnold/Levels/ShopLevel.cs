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
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update, checks for win condition
		/// </summary>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		protected override void UpdateInternal(GameTime gameTime)
		{
			if (EventManager.I.IsSignaled(EventType.ShopDoorOpen))
			{
				mLevelStatus = LevelStatus.Win;
			}
		}

		#endregion rUpdate
	}
}