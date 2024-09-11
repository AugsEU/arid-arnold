namespace AridArnold
{
	/// <summary>
	/// Return to the hub world
	/// </summary>
	abstract class ReturnToHubLoader : FadeOutFadeInLoader
	{
		public ReturnToHubLoader() : base(0)
		{
			mFadeOut = new FadeFX(new ScreenStars(), 0.1f, true);
			mFadeIn = new FadeFX(new ScreenStars(), 0.1f, false);
		}

		protected void ReturnToHubLoad(bool success)
		{
			// To do: Make this more complex
			HubReturnInfo returnInfo = CampaignManager.I.GetReturnInfo().Value;
			TimeZoneManager.I.SetCurrentTimeZoneAndAge(returnInfo.mEnterTimeZone, returnInfo.mEnterAge);

			LoadLevel(returnInfo.mHubRoom);

			// Bring these back to life
			foreach (Entity entity in returnInfo.mPersistentEntities)
			{
				EntityManager.I.InsertEntity(entity);
			}

			NotifyFinishedLoading();

			CampaignManager.I.EndSequence(success);
		}
	}

	/// <summary>
	/// Return to the hub after completing a sequence successfully
	/// </summary>
	class ReturnToHubSuccessLoader : ReturnToHubLoader
	{
		protected override void LevelLoadUpdate(GameTime gameTime)
		{
			ReturnToHubLoad(true);
		}
	}



	/// <summary>
	/// Return to the hub after failing to complete a sequence
	/// </summary>
	class ReturnToHubFailureLoader : ReturnToHubLoader
	{
		protected override void LevelLoadUpdate(GameTime gameTime)
		{
			ReturnToHubLoad(false);
		}
	}
}
