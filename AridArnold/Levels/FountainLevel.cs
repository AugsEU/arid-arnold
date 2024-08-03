namespace AridArnold
{
	/// <summary>
	/// Level where nothing happens.
	/// </summary>
	class FountainLevel : Level
	{
		#region rInitialisation

		/// <summary>
		/// Collect flag level construct
		/// </summary>
		public FountainLevel(AuxData auxData, int id) : base(auxData, id)
		{
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update, checks for win condition
		/// </summary>
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