namespace DeathRide
{
	class RunManager : Singleton<RunManager>
	{
		int mCurrentHealth;
		int mRoundNumber;
		bool mRunStarted = false;
		bool mExitRequested = false;
		ulong mScore;


		public bool HasStarted()
		{
			return mRunStarted;
		}

		public void StartRun()
		{
			mCurrentHealth = Player.MAX_HEALTH;
			mRoundNumber = 0;
			mScore = 0;
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
			// S TO DO SoundManager.I.PlaySFX(SoundManager.SFXType.GameOver, 1.0f);
			// S TO DO SoundManager.I.StopMusic();
			Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.ScreenCamera);
			gameCam.QueueMovement(new DiminishCameraShake(5.5f, 5.0f, 25.0f));
		}


		public void ResetNoEffects()
		{
			mExitRequested = false;
			mRunStarted = false;
			mRoundNumber = 0;
			mCurrentHealth = Player.MAX_HEALTH;
			mRoundNumber = 0;
			mScore = 0;
		}

		public int GetHealth()
		{
			return mCurrentHealth;
		}


		public int GetNumberOfEnemies()
		{
			int baseNum = (int)MathF.Ceiling(0.65f * MathF.Sqrt(mRoundNumber + 0.1f) + mRoundNumber);
			return Math.Min(baseNum, 25); // Cap at 25 to keep it reasonable
		}

		public int GetRounds()
		{
			return mRoundNumber;
		}

		public void AddScore(ulong delta, Vector2 pos)
		{
			delta = (ulong)(delta * (1.0f + mRoundNumber));

			if (delta > 0)
			{
				FXManager.I.AddTextScroller(Color.IndianRed, pos, "+" + delta.ToString());
			}

			if (mScore + delta < mScore)
			{
				// E MODE REACHED
				mScore = ulong.MaxValue;
			}
			else
			{
				mScore += delta;
			}
		}

		public ulong GetScore()
		{
			return mScore;
		}

		public bool ExitRequested()
		{
			return mExitRequested;
		}

		public void RequestExit()
		{
			mExitRequested = true;
		}
	}
}
