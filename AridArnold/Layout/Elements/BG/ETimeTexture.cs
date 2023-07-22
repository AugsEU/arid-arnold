namespace AridArnold
{
	internal class ETimeTexture : ETexture
	{
		int mTimeZone = 0;

		public ETimeTexture(XmlNode node) : base(node)
		{
			mTimeZone = MonoParse.GetInt(node["time"]);
			EventManager.I.AddListener(EventType.TimeChanged, OnTimeChange);
			SetVisible(mTimeZone == TimeZoneManager.I.GetCurrentTimeZone());
		}

		void OnTimeChange(EArgs eArgs)
		{
			SetVisible(mTimeZone == TimeZoneManager.I.GetCurrentTimeZone());
		}
	}
}
