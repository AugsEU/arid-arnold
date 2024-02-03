namespace AridArnold
{
	class CC_DisplayLayout : CinematicCommand
	{
		Layout mLayout;

		public CC_DisplayLayout(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			string layoutPath = cmdNode["layout"].InnerText;

			mLayout = new Layout(layoutPath);
		}

		public override void Draw(DrawInfo info)
		{
			mLayout.Draw(info);
		}

		public override void Update(GameTime gameTime, int currentFrame)
		{
			mLayout.Update(gameTime);
		}
	}
}
