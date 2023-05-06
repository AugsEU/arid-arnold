namespace AridArnold
{
	/// <summary>
	/// Return to the hub world
	/// </summary>
	abstract class ReturnToHubLoader : FadeOutFadeInLoader
	{
		public ReturnToHubLoader() : base(0)
		{
			mFadeOut = new ScreenStars(10.0f, 0.1f, true);
			mFadeIn = new ScreenStars(10.0f, 0.1f, false);
		}

		protected void ReturnToHubLoad()
		{
			// To do: Make this more complex
			HubReturnInfo? returnInfo = CampaignManager.I.GetReturnInfo();
			MonoDebug.Assert(returnInfo != null);

			LoadLevel(returnInfo.Value.mHubRoom);

			// Bring these back to life
			foreach (Entity entity in returnInfo.Value.mPersistentEntities)
			{
				EntityManager.I.InsertEntity(entity);
			}

			mLoadingState = LoadingState.FadeIn;
		}
	}



	/// <summary>
	/// Return to the hub after completing a sequence successfully
	/// </summary>
	class ReturnToHubSuccessLoader : ReturnToHubLoader
	{
		protected override void LevelLoadUpdate(GameTime gameTime)
		{
			// To do: make this more complicated
			ReturnToHubLoad();
			CampaignManager.I.EndSequence(true);
		}
	}



	/// <summary>
	/// Return to the hub after failing to complete a sequence
	/// </summary>
	class ReturnToHubFailureLoader : ReturnToHubLoader
	{
		protected override void LevelLoadUpdate(GameTime gameTime)
		{
			// To do: make this more complicated
			ReturnToHubLoad();
			CampaignManager.I.EndSequence(false);
		}
	}
}
