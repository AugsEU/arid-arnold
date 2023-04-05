namespace AridArnold
{
	/// <summary>
	/// Win condition status of the level
	/// </summary>
	enum LevelStatus
	{
		Continue,
		Win,
		Loss,
	}





	/// <summary>
	/// A level that can be played and has a win-condition
	/// </summary>
	abstract class Level
	{
		#region rMembers

		string mName;
		string mBGLayoutPath;
		protected LevelStatus mLevelStatus;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Level constructor
		/// </summary>
		/// <param name="levelName">Level's name</param>
		public Level(string levelName)
		{
			mName = levelName;
			EventManager.I.AddListener(EventType.PlayerDead, HandlePlayerDeath);
			mBGLayoutPath = "";
		}



		/// <summary>
		/// Load tile map from content manager
		/// </summary>
		public void Begin()
		{
			mLevelStatus = LevelStatus.Continue;
			TileManager.I.LoadLevel("Levels/" + mName);
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update level.
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		/// <returns>Win condition status</returns>
		public abstract LevelStatus Update(GameTime gameTime);

		#endregion rUpdate





		#region rUtility

		/// <summary>
		/// Handle player's death event.
		/// </summary>
		/// <param name="args">Event sender args</param>
		public virtual void HandlePlayerDeath(EArgs args)
		{
			mLevelStatus = LevelStatus.Loss;
		}



		/// <summary>
		/// Level name property
		/// </summary>
		public string GetName()
		{
			return mName;
		}



		/// <summary>
		/// Get path to layout file
		/// </summary>
		public string GetBGLayoutPath()
		{
			return mBGLayoutPath;
		}



		/// <summary>
		/// Set path to layout to use.
		/// </summary>
		public void SetBGLayoutPath(string path)
		{
			mBGLayoutPath = path;
		}
		#endregion rUtility
	}


}
