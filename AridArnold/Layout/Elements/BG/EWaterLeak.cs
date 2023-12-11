namespace AridArnold
{
	internal class EWaterLeak : LayElement
	{
		public EWaterLeak(XmlNode rootNode) : base(rootNode)
		{
			float distance = MonoParse.GetFloat(rootNode["distance"]);
			Color secColor = MonoParse.GetColor(rootNode["secondaryColor"]);

			FXManager.I.AddFX(new WaterLeakFX(GetPosition(), distance, GetColor(), secColor));
		}
	}
}
