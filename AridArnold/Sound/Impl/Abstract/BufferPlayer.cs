namespace AridArnold
{
	/// <summary>
	/// Wrapper class that plays buffers and can handle fading and such.
	/// </summary>
	internal class BufferPlayer
	{
		MonoTimer mFadeTimer;
		double mFadeTime;

		protected AudioBuffer mBuffer;

		public BufferPlayer(AudioBuffer buffer)
		{
			mBuffer = buffer;
			mFadeTimer = new MonoTimer();
		}

		public virtual void Begin(double fadeInTime)
		{
			mFadeTime = fadeInTime;
			mFadeTimer.ResetStart();
			mBuffer.Play();
		}

		public void Update(GameTime gameTime)
		{
			if(mBuffer.SoundState() != SoundState.Playing)
			{
				return;
			}

			mFadeTimer.Update(gameTime);

			float volume = 1.0f;
			if(mFadeTimer.IsPlaying())
			{
				if(mFadeTime > 0.0f)
				{
					volume = (float)((mFadeTimer.GetElapsedMs()) / mFadeTime);
				}
				else
				{
					volume = (float)((mFadeTimer.GetElapsedMs() + mFadeTime) / -mFadeTime);
				}

				if(mFadeTimer.GetElapsedMs() >= Math.Abs(mFadeTime))
				{
					mFadeTimer.Stop();
					
					// Negative fade time means we are fading out.
					if (mFadeTime < 0.0)
					{
						mBuffer.Stop();
					}
				}
			}

			volume = Math.Clamp(volume, 0.0f, 1.0f);

			mBuffer.SetVolume(volume);
		}

		public void Stop(double fadeOutTime)
		{
			mFadeTime = -fadeOutTime;
			mFadeTimer.ResetStart();
		}

		public AudioBuffer GetBuffer()
		{
			return mBuffer;
		}
	}
}
