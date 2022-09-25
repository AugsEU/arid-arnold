namespace AridArnold.Levels
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
        }



        /// <summary>
        /// Load tile map from content manager
        /// </summary>
        /// <param name="content">Monogame content manager</param>
		public void Begin(ContentManager content)
        {
            mLevelStatus = LevelStatus.Continue;
            TileManager.I.LoadLevel(content, "Levels/" + mName);
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
        public string pName
        { 
            get 
            { 
                return mName; 
            } 
        }

		#endregion rUtility
	}


}
