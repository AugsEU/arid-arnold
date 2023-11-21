namespace AridArnold
{
	internal class HubInstantLoader : LoadingSequence
	{
		public HubInstantLoader(int levelID, params Entity[] persistentEntities) : base(levelID)
		{
			LoadAsHubLevel();
		}

		public override void Draw(DrawInfo info)
		{
		}

		public override bool Finished()
		{
			return true;
		}

		public override void Update(GameTime gameTime)
		{
		}
	}
}
