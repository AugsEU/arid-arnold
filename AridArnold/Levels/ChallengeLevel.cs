namespace AridArnold
{
	/// <summary>
	/// Level that is actually a challenge to beat not a hub world or shop
	/// </summary>
	abstract class ChallengeLevel : Level
	{
		protected ChallengeLevel(AuxData data, int id) : base(data, id)
		{
		}

		public override void End()
		{
			bool success = mLevelStatus == LevelStatus.Win;
			GhostManager.I.EndLevel(success);
			ItemManager.I.LevelEnd(success);
			if(!success)
			{
				CollectableManager.I.NotifyLevelFail();
			}

			base.End();
		}

		public override bool CanLoseLives()
		{
			return true;
		}
	}
}
