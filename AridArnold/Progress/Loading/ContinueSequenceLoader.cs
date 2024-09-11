namespace AridArnold
{
	internal class ContinueSequenceLoader : FadeOutFadeInLoader
	{
		public ContinueSequenceLoader() : base(0)
		{
			mFadeOut = new FadeFX(new ScreenStars(), 0.1f, true);
			mFadeIn = new FadeFX(new ScreenStars(), 0.1f, false);
		}

		protected override void LevelLoadUpdate(GameTime gameTime)
		{
			Level nextLevel = CampaignManager.I.GetNextLevelInSequence();
			MonoDebug.Assert(nextLevel != null);
			LoadLevel(nextLevel);
			CollectableManager.I.NotifyLevelBegin();

			NotifyFinishedLoading();
		}
	}
}
