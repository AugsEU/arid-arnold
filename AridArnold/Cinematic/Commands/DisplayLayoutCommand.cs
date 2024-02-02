namespace AridArnold
{
	class DisplayLayoutCommand : CinematicCommand
	{
		Layout mLayout;

		public DisplayLayoutCommand(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			string layoutPath = cmdNode["layout"].Value;

			mLayout = new Layout(layoutPath);
		}

		public override void Draw(DrawInfo drawInfo)
		{
			mLayout.Draw(drawInfo);
		}

		public override void Update(GameTime gameTime, int currentFrame)
		{
			mLayout.Update(gameTime);
		}
	}
}
