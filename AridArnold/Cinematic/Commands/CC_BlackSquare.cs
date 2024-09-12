namespace AridArnold
{
	internal class CC_BlackSquare : CinematicCommand
	{
		public CC_BlackSquare(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
		}

		public override void Draw(DrawInfo info, int currentFrame)
		{
			MonoDraw.DrawRectDepth(info, Screen.SCREEN_RECTANGLE, Color.Black, DrawLayer.SequenceBG);
		}
	}
}
