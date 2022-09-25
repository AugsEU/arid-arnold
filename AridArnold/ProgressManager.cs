namespace AridArnold
{
	/// <summary>
	/// Data about the world(set of levels)
	/// </summary>
	struct WorldData
	{
		public int startLevel;
		public string name;

		//Style
		public Color worldColor;
		public string wallTexture;
		public string platformTexture;

		public WorldData(int _start, string _name, Color _worldColor, string _wallTexture, string _platformTexture)
		{
			startLevel = _start;
			name = _name;

			//Style
			worldColor = _worldColor;
			wallTexture = _wallTexture;
			platformTexture = _platformTexture;
		}
	}





	/// <summary>
	/// Manage progress of Arnold
	/// </summary>
	internal class ProgressManager : Singleton<ProgressManager>
	{
		#region rConstants

		const int START_LEVEL = 0;
		const int START_LIVES = 4;
		const int MAX_LIVES = 6;

		WorldData[] mWorldData =
		{
			new WorldData(0, "Iron Works", new Color(0, 10,20), "steel", "platform"),
			new WorldData(4, "Land of Mirrors", new Color(0, 24, 14), "cobble", "gold-platform"),
			new WorldData(8, "Buk's Cave", new Color(3, 3, 9), "cave", "cave-platform")
		};

		#endregion rConstants





		#region rMembers

		int mCurrentLevel = START_LEVEL;
		int mLastCheckPoint = START_LEVEL;
		int mLives = START_LIVES;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Init progress manager
		/// </summary>
		public void Init()
		{
			mLastCheckPoint = START_LEVEL;
			ResetGame();
		}



		/// <summary>
		/// Reset progress manager after a game over.
		/// </summary>
		public void ResetGame()
		{
			mLives = START_LIVES;
			mCurrentLevel = mLastCheckPoint;
		}

		#endregion rInitialisation



		#region rUtility

		/// <summary>
		/// Tell the progress manager we hit a checkpoint level.
		/// </summary>
		public void ReportCheckpoint()
		{
			mLastCheckPoint = mCurrentLevel + 1;

			if (mLives < START_LIVES)
			{
				mLives = START_LIVES;
			}
		}



		/// <summary>
		/// Tell the progress manager we lost a level
		/// </summary>
		public void ReportLevelLoss()
		{
			//Don't lose lives on the checkpoint levels.
			if (mCurrentLevel != mLastCheckPoint)
			{
				mLives--;
			}
		}



		/// <summary>
		/// Tell the progress manager we have won a level.
		/// </summary>
		public void ReportLevelWin()
		{
			mCurrentLevel++;
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
		/// Get world data. Member variables here break the style because they are effectively static variables(C++ style)
		/// </summary>
		int mPrevLevelQuery = -1;
		int mPrevWorldIndex = -1;
		public WorldData GetWorldData()
		{
			//Memoize 
			if (mCurrentLevel == mPrevLevelQuery && mPrevWorldIndex != -1)
			{
				return mWorldData[mPrevWorldIndex];
			}

			mPrevLevelQuery = mCurrentLevel;


			for (int i = 0; i < mWorldData.Length; i++)
			{
				mPrevWorldIndex = i;

				if (mCurrentLevel < mWorldData[i].startLevel)
				{
					mPrevWorldIndex = i - 1;
					break;
				}
			}

			return mWorldData[mPrevWorldIndex];
		}



		/// <summary>
		/// Lives remaining
		/// </summary>
		public int pLives
		{
			get { return mLives; }
			set { mLives = value; }
		}



		/// <summary>
		/// Current level index(out of all levels)
		/// </summary>
		public int pCurrentLevel
		{
			get { return mCurrentLevel; }
		}



		/// <summary>
		/// Last checkpoint level index(out of all levels)
		/// </summary>
		public int pLastCheckPoint
		{
			get { return mLastCheckPoint; }
		}

		#endregion rUtility
	}
}
