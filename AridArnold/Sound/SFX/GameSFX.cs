namespace AridArnold
{
	/// <summary>
	/// A sound effect for our game.
	/// </summary>
	class GameSFX : BufferPlayer
	{
		public GameSFX(AridArnoldSFX effect, float maxVol, float minPitch = 0.0f, float maxPitch = 0.0f) : base(MonoSound.Impl.LoadAudioBuffer(effect), maxVol)
		{
			if(maxPitch != 0.0f || minPitch != 0.0f)
			{
				MonoRandom rng = RandomManager.I.GetDraw();

				float pitch = rng.GetFloatRange(minPitch, maxPitch);
				mBuffer.SetPitch(pitch);
			}
		}

		public virtual void UpdateListeners(List<AudioPositionInfo> listeners)
		{
			// By default do nothing.
		}

		public bool IsFinished()
		{
			return mBuffer.GetState() == SoundState.Stopped;
		}
	}
}
