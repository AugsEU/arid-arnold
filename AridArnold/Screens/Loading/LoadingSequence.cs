﻿namespace AridArnold
{
	abstract class LoadingSequence
	{
		protected int mLevelID;

		public LoadingSequence(int levelID)
		{
			mLevelID = levelID;
		}

		public abstract void Update(GameTime gameTime);

		public abstract void Draw(DrawInfo info);

		public abstract bool Finished();


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

		protected virtual void PostLevelLoad()
		{
		}
	}
}