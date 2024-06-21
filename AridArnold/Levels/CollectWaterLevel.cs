namespace AridArnold
{
	/// <summary>
	/// Level type where you have to collect water to win.
	/// </summary>
	class CollectWaterLevel : ChallengeLevel
	{
		#region rMembers

		int mNumWaterNeeded;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Collect water level constructor
		/// </summary>
		/// <param name="numNeeded">Number of water bottles needed to win</param>
		public CollectWaterLevel(AuxData auxData, int id) : base(auxData, id)
		{
			mNumWaterNeeded = auxData.GetIntParams()[0];
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update, checks for number of bottles needed to win.
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		/// <returns>Level completion status</returns>
		protected override void UpdateInternal(GameTime gameTime)
		{
			if (CollectableManager.I.GetNumTransient(TransientCollectable.WaterBottle) >= mNumWaterNeeded)
			{
				mLevelStatus = LevelStatus.Win;
			}
		}

		#endregion rUpdate
	}
}
