namespace AridArnold
{
	internal class ETimeTexture : ETexture
	{
		int mTimeZone = 0;

		public ETimeTexture(XmlNode node) : base(node)
		{
			mTimeZone = MonoParse.GetInt(node["time"]);
			SetVisible(mTimeZone == TimeZoneManager.I.GetCurrentTimeZone());
		}

		public override void Update(GameTime gameTime)
		{
			if(EventManager.I.IsSignaled(EventType.TimeChanged))
			{
				SetVisible(mTimeZone == TimeZoneManager.I.GetCurrentTimeZone());
			}
			base.Update(gameTime);
		}
	}
}
