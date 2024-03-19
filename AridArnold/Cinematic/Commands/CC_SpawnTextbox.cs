
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

			SpeechBoxStyle style = MonoParse.GetSpeechBoxStyle(cmdNode);
			style.mWidth = 200.1f;

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
