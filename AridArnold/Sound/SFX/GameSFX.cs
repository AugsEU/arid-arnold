namespace AridArnold
{
	/// <summary>
	/// A sound effect for our game.
	/// </summary>
	class GameSFX : BufferPlayer
	{
		public GameSFX(AridArnoldSFX effect, float minPitch = 0.0f, float maxPitch = 0.0f) : base(MonoSound.Impl.LoadAudioBuffer(effect))
		{
			if(maxPitch != minPitch)
			{
				MonoRandom rng = RandomManager.I.GetDraw();

				float pitch = rng.GetFloatRange(minPitch, maxPitch);
				mBuffer.SetPitch(pitch);
			}
		}

		public virtual void UpdateListeners(List<Vector2> listeners)
		{
			// By default do nothing.
		}

		public bool IsFinished()
		{
			return mBuffer.SoundState() == SoundState.Stopped;
		}
	}
}
