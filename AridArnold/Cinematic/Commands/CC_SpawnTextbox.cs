
namespace AridArnold
{
	internal class CC_SpawnTextbox : CC_ActorCommand
	{

		SpeechBoxRenderer mSpeechBox;
		Texture2D mMouthOpen;
		Texture2D mMouthClosed;
		MonoTimer mMouthTimer;
		bool mDoneWithActor;

		public CC_SpawnTextbox(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			// Lasts forever
			mFrameSpan.SetMax(int.MaxValue);

			string stringID = MonoParse.GetString(cmdNode["text"]);

			SpeechBoxStyle style = MonoParse.GetSpeechBoxStyle(cmdNode);
			style.mWidth = 200.1f;

			float spikeOffset = MonoParse.GetFloat(cmdNode["spike"], 30.0f);
			Vector2 dialogPosition = MonoParse.GetVector(cmdNode);

			mMouthOpen = MonoParse.GetTexture(cmdNode["mouthOpen"], null);
			mMouthClosed = MonoParse.GetTexture(cmdNode["mouthClosed"], null);

			mMouthTimer = new MonoTimer();

			mSpeechBox = new SpeechBoxRenderer(stringID, dialogPosition, spikeOffset, style);
			mDoneWithActor = false;
		}

		public override void Update(GameTime gameTime, int currentFrame)
		{
			mMouthTimer.Update(gameTime);

			if (!mSpeechBox.IsTextFinished())
			{
				bool mouthOpen = MonoText.IsVowel(mSpeechBox.GetCurrentChar());

				if (mouthOpen)
				{
					mMouthTimer.FullReset();
					mMouthTimer.Start();
				}

				Texture2D actorTexture = mMouthClosed;

				if (mMouthTimer.IsPlaying() && mMouthTimer.GetElapsedMs() < NPC.MOUTH_OPEN_TIME)
				{
					actorTexture = mMouthOpen;
				}

				if (actorTexture is not null)
				{
					mTargetActor.SetDrawTexture(actorTexture);
				}
			}
			else
			{
				if(!mDoneWithActor)
				{
					// Finish up with actor on closed mouth.
					mTargetActor.SetDrawTexture(mMouthClosed);
					mDoneWithActor = true;
				}
			}

			mSpeechBox.Update(gameTime);
		}

		public override void Draw(DrawInfo info, int currentFrame)
		{
			mSpeechBox.Draw(info);
		}
	}
}
