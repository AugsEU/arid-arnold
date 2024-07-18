namespace AridArnold
{
	class LevelDirectLoader : LevelTransitionLoader
	{
		public LevelDirectLoader(int levelID) : base(levelID)
		{
			mFadeOut = null;
			mFadeIn = new FadeFX(new ScreenStars(), 0.1f, false);
		}
	}
}
