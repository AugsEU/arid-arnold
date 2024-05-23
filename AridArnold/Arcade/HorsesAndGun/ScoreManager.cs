namespace HorsesAndGun
{
	internal class ScoreManager : Singleton<ScoreManager>
	{
		int mCurrentScore;
		int mHighScore;

		public ScoreManager()
		{
			mHighScore = 0;
			mCurrentScore = 0;
		}

		public void ResetScore()
		{
			mCurrentScore = 0;
		}

		public void AddCurrentScore(int score)
		{
			mCurrentScore += score;
		}

		public int GetCurrentScore()
		{
			return mCurrentScore;
		}

		public int GetHighScore()
		{
			return mHighScore;
		}

		public bool CheckHighScore()
		{
			if (mCurrentScore > mHighScore)
			{
				mHighScore = mCurrentScore;
				return true;
			}

			return false;
		}

		public void ResetAll()
		{
			mHighScore = 0;
			mCurrentScore = 0;
		}
	}
}
