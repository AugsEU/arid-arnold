
namespace AridArnold
{
	internal class CC_SpawnTextbox : CinematicCommand
	{
		SpeechBoxRenderer mSpeechBox;

		public CC_SpawnTextbox(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			// Lasts forever
			mFrameSpan.SetMax(int.MaxValue);

			string stringID = MonoParse.GetString(cmdNode["text"]);

			SpeechBoxStyle style = SpeechBoxStyle.DefaultStyle;
			style.mFramesPerLetter /= 4;
			style.mWidth = 200.1f;

			style.mBorderColor = MonoParse.GetColor(cmdNode["borderColor"], style.mBorderColor);
			style.mFillColor = MonoParse.GetColor(cmdNode["fillColor"], style.mFillColor);
			style.mScrollSpeed = MonoParse.GetFloat(cmdNode["scroll"], style.mScrollSpeed);
			style.mFlipSpike = cmdNode["flipSpike"] is not null;

			float spikeOffset = MonoParse.GetFloat(cmdNode["spike"], 30.0f);
			Vector2 dialogPosition = MonoParse.GetVector(cmdNode);


			mSpeechBox = new SpeechBoxRenderer(stringID, dialogPosition, spikeOffset, style);
		}

		public override void Update(GameTime gameTime, int currentFrame)
		{
			mSpeechBox.Update(gameTime);
		}

		public override void Draw(DrawInfo info, int currentFrame)
		{
			mSpeechBox.Draw(info);
		}
	}
}
