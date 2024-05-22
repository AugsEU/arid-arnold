namespace GMTK2023
{
	class RunManager : Singleton<RunManager>
	{
		const int NUM_HEALTH_PACKS = 2;

		int mHighScore = 0;
		int mCurrentHealth;
		int mHealthPacksRemaining;
		int mRoundNumber;
		bool mRunStarted = false;


		public bool HasStarted()
		{
			return mRunStarted;
		}

		public void StartRun()
		{
			mHealthPacksRemaining = NUM_HEALTH_PACKS;
			mCurrentHealth = Player.MAX_HEALTH;
			mRoundNumber = 0;
			mRunStarted = true;
		}


		public void EndRound(Player player)
		{
			mCurrentHealth = player.GetHealth();
			mRoundNumber++;
		}

		public void EndRun()
		{
			mRunStarted = false;
			if (mRoundNumber > mHighScore)
			{
				mHighScore = mRoundNumber;
			}
			SoundManager.I.PlaySFX(SoundManager.SFXType.GameOver, 1.0f);
			SoundManager.I.StopMusic();
			Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.ScreenCamera);
			gameCam.QueueMovement(new DiminishCameraShake(5.5f, 5.0f, 25.0f));
		}

		public int GetHealth()
		{
			return mCurrentHealth;
		}


		public int GetNumberOfEnemies()
		{
			return 2 * (int)MathF.Ceiling(MathF.Sqrt(mRoundNumber + 0.25f)) + mRoundNumber;
		}

		public void UseHealthPack()
		{
			if (mHealthPacksRemaining > 0)
			{
				mCurrentHealth = Player.MAX_HEALTH;
				mHealthPacksRemaining--;
			}
		}

		public int GetRounds()
		{
			return mRoundNumber;
		}

		public int GetHighScore()
		{
			return mHighScore;
		}
	}
}
