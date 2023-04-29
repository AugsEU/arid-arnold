namespace AridArnold
{
	class HubLevel : Level
	{
		public HubLevel(AuxData data) : base(data)
		{
		}

		protected override LevelStatus UpdateInternal(GameTime gameTime)
		{
			return LevelStatus.Continue;
		}
	}
}
