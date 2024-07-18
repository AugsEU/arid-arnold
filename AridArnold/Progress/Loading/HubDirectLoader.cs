namespace AridArnold
{
	class HubDirectLoader : HubTransitionLoader
	{
		public HubDirectLoader(int levelID) : base(levelID)
		{
			mFadeOut = null;
			mFadeIn = new FadeFX(new ScreenStars(10.0f), 0.1f, false);
		}
	}
}
