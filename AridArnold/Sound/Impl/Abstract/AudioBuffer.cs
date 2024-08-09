namespace AridArnold
{
	/// <summary>
	/// Information about an audio listener/emitter location.
	/// </summary>
	struct AudioPositionInfo
	{
		public Vector3 mPosition = Vector3.Zero;
		public Vector3 mForward = new Vector3(0.0f, 0.0f, -1.0f);
		public Vector3 mUp = new Vector3(0.0f, -1.0f, 0.0f);
		public Vector3 mVelocity = Vector3.Zero;

		public AudioPositionInfo(Vector3 pos, Vector3 velocity)
		{
			mPosition = pos;
			mVelocity = velocity;
		}

		public AudioPositionInfo(Vector2 pos, Vector2 velocity)
		{
			mPosition = new Vector3(pos, 0.0f);
			mVelocity = new Vector3(velocity, 0.0f);
		}
	}

	/// <summary>
	/// Audio buffer mostly for playing SFX.
	/// </summary>
	abstract class AudioBuffer
	{
		#region rUtil

		public abstract void Play();
		public abstract void Resume();
		public abstract void Pause();
		public abstract void Stop();

		public abstract SoundState SoundState();

		/// <summary>
		/// Set information about us emitting.
		/// </summary>
		public abstract void SetEmitter(AudioPositionInfo info);

		/// <summary>
		/// Set information about who is listening.
		/// </summary>
		/// <param name="info"></param>
		public abstract void SetListeners(params AudioPositionInfo[] infos);



		/// <summary>
		/// Get the volume
		/// </summary>
		public abstract float GetVolume();

		/// <summary>
		/// Set the volume
		/// </summary>
		public abstract void SetVolume(float volume);



		/// <summary>
		/// Set pitch +1 is one octabe, -1 is down an octave
		/// </summary>
		public abstract void SetPitch(float pitch);

		/// <summary>
		/// Get the pitch
		/// </summary>
		public abstract float GetPitch();



		/// <summary>
		/// Set pan from -1 to 1
		/// </summary>
		public abstract void SetPan(float pan);

		/// <summary>
		/// Get pan from -1 to 1
		/// </summary>
		public abstract float GetPan();



		/// <summary>
		/// Get if buffer is looping
		/// </summary>
		public abstract void SetLoop(bool loop);

		/// <summary>
		/// Get if buffer is looping
		/// </summary>
		public abstract bool GetLoop();

		#endregion rUtil
	}
}
