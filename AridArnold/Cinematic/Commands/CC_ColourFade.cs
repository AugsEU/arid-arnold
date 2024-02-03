namespace AridArnold
{
	internal class CC_ColourFade : CinematicCommand
	{
		Color mStartColour;
		Color? mMiddleColour;
		Color mEndColour;

		Color mCurrentColour;

		public CC_ColourFade(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			mStartColour = MonoParse.GetColor(cmdNode["start"], Color.Transparent);
			mMiddleColour = cmdNode["mid"] is not null ? MonoParse.GetColor(cmdNode["mid"], Color.Transparent) : null;
			mEndColour = MonoParse.GetColor(cmdNode["end"], Color.Transparent);

			mCurrentColour = mStartColour;
		}

		public override void Update(GameTime gameTime, int currentFrame)
		{
			float t = GetActivePercent(currentFrame);

			// Could be cleaner but who cares? Mess is contained within small class.
			if (mMiddleColour.HasValue)
			{
				if (t < 0.4f)
				{
					mCurrentColour = MonoMath.Lerp(mStartColour, mMiddleColour.Value, t);
				}
				else if (t > 0.6f)
				{
					mCurrentColour = MonoMath.Lerp(mMiddleColour.Value, mEndColour, t);
				}
				else
				{
					mCurrentColour = mMiddleColour.Value;
				}
			}
			else
			{
				mCurrentColour = MonoMath.Lerp(mStartColour, mEndColour, t);
			}
		}

		public override void Draw(DrawInfo info)
		{
			Rectangle bigRectangle = new Rectangle(-10000, -10000, 30000, 30000);
			//MonoDraw.DrawRectDepth(info, bigRectangle, mCurrentColour, DrawLayer.Front);
		}
	}
}
