namespace AridArnold
{
	internal class LevelSequenceLoader : FadeOutFadeInLoader
	{
		List<Level> mLevelSequence;

		public LevelSequenceLoader(List<Level> levelSequence) : base(0)
		{
			mLevelSequence = levelSequence;
			mFadeOut = new ScreenStars(10.0f, 0.1f, true);
			mFadeIn = new ScreenStars(10.0f, 0.1f, false);
		}

		protected override void LevelLoadUpdate(GameTime gameTime)
		{
			// To do: Make this more complex
			CampaignManager.I.PushLevelSequence(mLevelSequence);
			LoadLevel(mLevelSequence[0]);
			mLoadingState = LoadingState.FadeIn;
		}
	}
}
