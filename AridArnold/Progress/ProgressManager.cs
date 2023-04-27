namespace AridArnold
{
	/// <summary>
	/// Manage progress of Arnold
	/// </summary>
	internal class ProgressManager : Singleton<ProgressManager>
	{
		#region rConstants 

		const int START_LIVES = 5;
		public const int MAX_LIVES = 7;

		#endregion rConstants





		#region rMembers

		int mLives = START_LIVES;

		List<Level> mLoadedLevels;
		int mCurrentLevel;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Init progress manager
		/// </summary>
		public void Init()
		{
			mLoadedLevels = new List<Level>();
			mCurrentLevel = 0;
		}

		#endregion rInitialisation





		#region rUtility

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
			// To do: Load next level?
			GetCurrentLevel().End();
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
		/// Reset lives to default.
		/// </summary>
		public void ResetLives()
		{
			mLives = 3; // To do
		}



		/// <summary>
		/// Can we lose lives on this level?
		/// </summary>
		public bool CanLoseLives()
		{
			return mCurrentLevel != 0;
		}



		/// <summary>
		/// Get level object
		/// </summary>
		/// <returns>Get current level</returns>
		public Level GetCurrentLevel()
		{
			if(mLoadedLevels.Count == 0)
			{
				throw new Exception("No current level");
			}

			return mLoadedLevels[mCurrentLevel];
		}



		/// <summary>
		/// Get level number out of all levels
		/// </summary>
		/// <returns>Get current level</returns>
		public int GetLevelNumber()
		{
			return mCurrentLevel + 1;
		}



		/// <summary>
		/// Lives remaining
		/// </summary>
		public int GetNumLives()
		{
			return mLives;
		}

		#endregion rUtility
	}
}
