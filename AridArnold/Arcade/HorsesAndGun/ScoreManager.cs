namespace HorsesAndGun
{
	internal class ScoreManager : Singleton<ScoreManager>
	{
		ulong mCurrentScore;

		public ScoreManager()
		{
			mCurrentScore = 0;
		}

		public void ResetScore()
		{
			mCurrentScore = 0;
		}

		public void AddCurrentScore(ulong score)
		{
			mCurrentScore += score;
		}

		public ulong GetCurrentScore()
		{
			return mCurrentScore;
		}

		public void ResetAll()
		{
			mCurrentScore = 0;
		}
	}
}
