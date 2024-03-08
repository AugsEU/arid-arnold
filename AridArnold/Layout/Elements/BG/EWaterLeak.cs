namespace AridArnold
{
	internal class EWaterLeak : LayElement
	{
		WaterLeakFX mWaterLeak;

		public EWaterLeak(XmlNode rootNode) : base(rootNode)
		{
			float distance = MonoParse.GetFloat(rootNode["distance"]);
			Color secColor = MonoParse.GetColor(rootNode["secondaryColor"]);

			mWaterLeak = new WaterLeakFX(GetPosition(), distance, GetColor(), secColor);
		}

		public override void Update(GameTime gameTime)
		{
			mWaterLeak.Update(gameTime);
		}

		public override void Draw(DrawInfo info)
		{
			mWaterLeak.Draw(info);
			base.Draw(info);
		}
	}
}
