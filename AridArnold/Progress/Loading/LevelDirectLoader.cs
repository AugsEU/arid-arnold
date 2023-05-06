namespace AridArnold
{
	class LevelDirectLoader : LevelTransitionLoader
	{
		public LevelDirectLoader(int levelID) : base(levelID)
		{
			mFadeOut = null;
			mFadeIn = new ScreenStars(10.0f, 0.1f, false);
		}
	}
}
