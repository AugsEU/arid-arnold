namespace AridArnold
{
	/// <summary>
	/// Wrapper class that plays buffers and can handle fading and such.
	/// </summary>
	internal class BufferPlayer
	{
		float mMaxVolume;
		MonoTimer mFadeTimer;
		double mFadeTime;

		protected AudioBuffer mBuffer;

		public BufferPlayer(AudioBuffer buffer, float maxVolume)
		{
			mBuffer = buffer;
			mFadeTimer = new MonoTimer();
			mMaxVolume = maxVolume;
		}

		public virtual void Begin(double fadeInTime)
		{
			mFadeTime = fadeInTime;
			mFadeTimer.ResetStart();
			ApplyVolume();
			mBuffer.Play();
		}

		public void Update(GameTime gameTime)
		{
			if (mBuffer.SoundState() != SoundState.Playing)
			{
				return;
			}

			mFadeTimer.Update(gameTime);

			ApplyVolume();
		}

		protected virtual float DecideVolume()
		{
			float volume = 1.0f;
			if (mFadeTimer.IsPlaying())
			{
				if (mFadeTime > 0.0f)
				{
					volume = (float)((mFadeTimer.GetElapsedMs()) / mFadeTime);
				}
				else
				{
					volume = (float)((mFadeTimer.GetElapsedMs() + mFadeTime) / mFadeTime);
				}

				if (mFadeTimer.GetElapsedMs() >= Math.Abs(mFadeTime))
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

			return volume;
		}

		private void ApplyVolume()
		{
			float vol = DecideVolume();
			mBuffer.SetVolume(vol * mMaxVolume);
		}

		public void Stop(double fadeOutTime)
		{
			if(mFadeTime < 0.0)
			{
				// Already stopping, don't keep stopping.
				return;
			}
			mFadeTime = -fadeOutTime;
			mFadeTimer.ResetStart();
		}


		public AudioBuffer GetBuffer()
		{
			return mBuffer;
		}
	}
}
