namespace AridArnold
{
	struct LevelPoint
	{
		public int mWorldIndex;
		public int mLevel;

		public LevelPoint(int worldIndex, int level)
		{
			mWorldIndex = worldIndex;
			mLevel = level;
		}

		public static bool operator ==(LevelPoint a, LevelPoint b)
		=> (a.mWorldIndex == b.mWorldIndex && a.mLevel == b.mLevel);

		public static bool operator !=(LevelPoint a, LevelPoint b)
		=> (!(a == b));

		public override bool Equals(object obj)
		{
			if (!(obj is LevelPoint))
				return false;

			return this == (LevelPoint)obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}


 
	/// <summary>
	/// Manage progress of Arnold
	/// </summary>
	internal class ProgressManager : Singleton<ProgressManager>
	{
		#region rConstants 

		const int START_WORLD = 4; 
		const int START_LEVEL = 2;
		const int START_LIVES = 4;
		public const int MAX_LIVES = 6;

		#endregion rConstants





		#region rMembers

		Campaign mCampaign;
		LevelPoint mCurrentLevel;
		LevelPoint mLastCheckPoint;
		int mLives = START_LIVES;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Init progress manager
		/// </summary>
		public void Init(string campaignXML)
		{
			mCurrentLevel = new LevelPoint(START_WORLD, START_LEVEL);
			mLastCheckPoint = new LevelPoint(START_WORLD, START_LEVEL);
			mCampaign = new Campaign(campaignXML);
			ResetGame();
		}



		/// <summary>
		/// Reset progress manager after a game over.
		/// </summary>
		public void ResetGame()
		{
			ResetLives();
			mCurrentLevel = mLastCheckPoint;
		}


		/// <summary>
		/// Reset lives to full.
		/// </summary>
		public void ResetLives()
		{
			mLives = START_LIVES;
		}

		#endregion rInitialisation





		#region rUtility

		/// <summary>
		/// Tell the progress manager we hit a checkpoint level.
		/// </summary>
		public void ReportCheckpoint()
		{
			mLastCheckPoint = GetNextLevelPoint(mCurrentLevel);
		}



		/// <summary>
		/// Tell the progress manager we lost a level
		/// </summary>
		public void ReportLevelLoss()
		{
			//Don't lose lives on the checkpoint levels.
			if (CanLoseLives())
			{
				mLives--;
			}
		}



		/// <summary>
		/// Tell the progress manager we have won a level.
		/// </summary>
		public void ReportLevelWin()
		{
			mCurrentLevel = GetNextLevelPoint(mCurrentLevel);
		}



		/// <summary>
		/// Gain 1 life
		/// </summary>
		public void GiveLife()
		{
			if (mLives < MAX_LIVES)
			{
				mLives++;
			}
		}



		/// <summary>
		/// Can we lose lives on this level?
		/// </summary>
		public bool CanLoseLives()
		{
			return mCurrentLevel != mLastCheckPoint;
		}



		/// <summary>
		/// Get world data.
		/// </summary>
		public World GetCurrentWorld()
		{
			return mCampaign.GetWorld(mCurrentLevel.mWorldIndex);
		}



		/// <summary>
		/// Get world at index.
		/// </summary>
		public World GetWorld(int i)
		{
			return mCampaign.GetWorld(i);
		}



		/// <summary>
		/// Get level object
		/// </summary>
		/// <returns>Get current level</returns>
		public Level GetCurrentLevel()
		{
			return GetCurrentWorld().GetLevel(mCurrentLevel.mLevel);
		}



		/// <summary>
		/// Get level number out of all levels
		/// </summary>
		/// <returns>Get current level</returns>
		public int GetTotalLevelNumber()
		{
			int total = 0;
			for (int w = 0; w < mCurrentLevel.mWorldIndex; w++)
			{
				total += mCampaign.GetWorld(w).GetNumberOfLevels();
			}

			total += mCurrentLevel.mLevel;
			return total + 1;
		}



		/// <summary>
		/// Get the current level point
		/// </summary>
		public LevelPoint GetLevelPoint()
		{
			return mCurrentLevel;
		}


		/// <summary>
		/// Get the current level point as a combined hex code.
		/// </summary>
		public uint GetLevelPointHex()
		{
			return ((uint)mCurrentLevel.mWorldIndex << 8) + (uint)mCurrentLevel.mLevel;
		}



		/// <summary>
		/// Have we finished the game?
		/// </summary>
		/// <returns>True if we have finished all the levels</returns>
		public bool HasFinishedGame()
		{
			return mCurrentLevel.mWorldIndex >= mCampaign.GetNumberOfWorlds();
		}



		/// <summary>
		/// Advance a level forwards by one
		/// </summary>
		/// <param name="levelPoint">Starting level</param>
		/// <returns>Level point 1 after the starting level</returns>
		public LevelPoint GetNextLevelPoint(LevelPoint levelPoint)
		{
			levelPoint.mLevel += 1;
			if (levelPoint.mLevel >= GetCurrentWorld().GetNumberOfLevels())
			{
				levelPoint.mLevel = 0;
				levelPoint.mWorldIndex += 1;
			}

			return levelPoint;
		}



		/// <summary>
		/// Lives remaining
		/// </summary>
		public int pLives
		{
			get { return mLives; }
			set { mLives = value; }
		}

		#endregion rUtility
	}
}
