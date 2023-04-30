namespace AridArnold
{
	class HubDirectLoader : HubTransitionLoader
	{
		public HubDirectLoader(int levelID) : base(levelID)
		{
			mFadeOut = null;
			mFadeIn = new ScreenWipe(CardinalDirection.Up, 0.05f, false);
		}
	}
}
