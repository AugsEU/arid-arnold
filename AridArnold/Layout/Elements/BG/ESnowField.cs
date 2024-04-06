namespace AridArnold
{
	class ESnowField : LayElement
	{
		const int kNumSnowRect = 20;

		SnowRect[] mSnowRects;
		int mTimeZone = 0;

		public ESnowField(XmlNode rootNode) : base(rootNode)
		{
			Rectangle snowSizeRect = MonoParse.GetRectangle(rootNode);

			mSnowRects = new SnowRect[kNumSnowRect];

			for (int i = 0; i < mSnowRects.Length; i++)
			{
				mSnowRects[i] = new SnowRect(snowSizeRect, 5, 5, 1.9f);
			}

			mTimeZone = MonoParse.GetInt(rootNode["time"], 1);
		}

		public override void Update(GameTime gameTime)
		{
			if (TimeZoneManager.I.GetCurrentTimeZone() != mTimeZone)
			{
				return;
			}
			foreach (SnowRect rect in mSnowRects)
			{
				rect.Update(gameTime);
			}
			base.Update(gameTime);
		}

		public override void Draw(DrawInfo info)
		{
			if (TimeZoneManager.I.GetCurrentTimeZone() != mTimeZone)
			{
				return;
			}
			foreach (SnowRect rect in mSnowRects)
			{
				rect.Draw(info);
			}
			base.Draw(info);
		}
	}
}
