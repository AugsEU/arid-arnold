namespace AridArnold
{
	internal class HubTransitionLoader : FadeOutFadeInSimpleLoader
	{
		public HubTransitionLoader(int levelID) : base(levelID)
		{
		}

		protected override void DoLevelLoad()
		{
			LoadAsHubLevel();
		}
	}
}
