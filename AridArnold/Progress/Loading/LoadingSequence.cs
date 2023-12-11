namespace AridArnold
{
	abstract class LoadingSequence
	{
		protected int mLevelID;
		protected List<Entity> mPersistentEntities;

		public LoadingSequence(int levelID)
		{
			mPersistentEntities = new List<Entity>();
			mLevelID = levelID;
		}

		public abstract void Update(GameTime gameTime);

		public abstract void Draw(DrawInfo info);

		public abstract bool Finished();

		public void AddPersistentEntities(params Entity[] entities)
		{
			foreach (Entity entity in entities)
			{
				mPersistentEntities.Add(entity);
			}
		}

		protected void LoadAsHubLevel()
		{
			Level prevLevel = CampaignManager.I.GetCurrentLevel();

			if (prevLevel is not null) prevLevel.End();

			Level newLevel = CampaignManager.I.LoadHubRoom(mLevelID);

			if (newLevel is not null) newLevel.Begin();

			PostLevelLoad();
		}

		protected void LoadAsGameLevel()
		{
			Level prevLevel = CampaignManager.I.GetCurrentLevel();

			if (prevLevel is not null) prevLevel.End();

			Level newLevel = CampaignManager.I.LoadGameLevel(mLevelID);

			if (newLevel is not null) newLevel.Begin();

			PostLevelLoad();
		}

		protected void LoadLevel(Level level)
		{
			Level prevLevel = CampaignManager.I.GetCurrentLevel();

			if (prevLevel is not null) prevLevel.End();

			CampaignManager.I.SetCurrentLevel(level);

			level.Begin();

			PostLevelLoad();
		}

		protected virtual void PostLevelLoad()
		{
			foreach (Entity entity in mPersistentEntities)
			{
				EntityManager.I.InsertEntity(entity);
			}
		}
	}
}
