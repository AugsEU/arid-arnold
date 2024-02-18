namespace AridArnold
{
	internal class CC_BlackFade : CinematicCommand
	{
		enum FadeCmdType
		{
			ToBlack = 0b01, // Fade from something *to black*
			BlackTo = 0b10, // Fade from *black to* something
			ToBlackTo = 0b11, // Fade from something *to black to* something
		}

		FadeCmdType mType;
		ScreenStars mToBlack;

		public CC_BlackFade(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			mType = ReadFadeType(cmdNode["type"].InnerText);
			mToBlack = new ScreenStars(10.0f, 0.0f, false);
		}

		FadeCmdType ReadFadeType(string fadeStr)
		{
			switch (fadeStr.ToLower())
			{
				case "toblack":
					return FadeCmdType.ToBlack;
				case "blackto":
					return FadeCmdType.BlackTo;
				case "toblackto":
					return FadeCmdType.ToBlackTo;
			}

			throw new NotImplementedException();
		}

		public override void Draw(DrawInfo info, int currentFrame)
		{
			float t = GetActivePercent(currentFrame);

			switch (mType)
			{
				case FadeCmdType.ToBlack:
					// t = t;
					break;
				case FadeCmdType.BlackTo:
					t = 1.0f - t;
					break;
				case FadeCmdType.ToBlackTo:
					t = Math.Min(1.3f - Math.Abs(2.6f * t - 1.3f), 1.0f); // Plot this on desmos to see what is going on
					break;
			}

			mToBlack.DrawAtTime(info, t);
		}
	}
}
