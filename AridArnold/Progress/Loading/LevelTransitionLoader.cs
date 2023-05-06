namespace AridArnold
{
	internal class LevelTransitionLoader : FadeOutFadeInSimpleLoader
	{
		public LevelTransitionLoader(int levelID) : base(levelID)
		{
		}

		protected override void DoLevelLoad()
		{
			LoadAsGameLevel();
		}
	}
}
