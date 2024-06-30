namespace AridArnold
{
	internal class LevelSequenceLoader : FadeOutFadeInLoader
	{
		public LevelSequenceLoader() : base(0)
		{
			mFadeOut = new ScreenStars(10.0f, 0.1f, true);
			mFadeIn = new ScreenStars(10.0f, 0.1f, false);
		}

		protected override void LevelLoadUpdate(GameTime gameTime)
		{
			// To do: Make this more complex
			Level nextLevel = CampaignManager.I.GetNextLevelInSequence();
			MonoDebug.Assert(nextLevel != null);
			LoadLevel(nextLevel);
			CollectableManager.I.NotifyLevelBegin();

			NotifyFinishedLoading();
		}
	}
}
