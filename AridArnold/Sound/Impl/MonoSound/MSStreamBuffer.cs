using MonoSound;
using MonoSound.Streaming;

namespace AridArnold
{
	internal class MSStreamBuffer : AudioBuffer
	{
		#region rMembers

		StreamPackage mStreamPackage;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create streamed buffer
		/// </summary>
		public MSStreamBuffer(string path)
		{
			mStreamPackage = StreamLoader.GetStreamedSound(path, true);
		}

		#endregion rInit





		#region rPlay

		/// <summary>
		/// Play the stream.
		/// </summary>
		public override void Play()
		{
			mStreamPackage.Play();
		}

		/// <summary>
		/// Resume playing
		/// </summary>
		public override void Resume()
		{
			mStreamPackage.Resume();
		}

		/// <summary>
		/// Pause playing.
		/// </summary>
		public override void Pause()
		{
			mStreamPackage.Pause();
		}

		/// <summary>
		/// Stop playing completely.
		/// </summary>
		public override void Stop()
		{
			mStreamPackage.Stop();
		}

		#endregion rPlay





		#region rUtil

		public override SoundState SoundState()
		{
			return mStreamPackage.Metrics.State;
		}

		public override bool GetLoop()
		{
			return mStreamPackage.IsLooping;
		}

		public override void SetLoop(bool loop)
		{
			mStreamPackage.IsLooping = loop;
		}

		public override float GetPan()
		{
			return mStreamPackage.Metrics.Pan;
		}

		public override void SetPan(float pan)
		{
			mStreamPackage.Metrics.Pan = pan;
		}

		public override float GetPitch()
		{
			return mStreamPackage.Metrics.Pitch;
		}

		public override void SetPitch(float pitch)
		{
			mStreamPackage.Metrics.Pitch = pitch;
		}

		public override float GetVolume()
		{
			return mStreamPackage.Metrics.Volume;
		}

		public override void SetVolume(float volume)
		{
			mStreamPackage.Metrics.Volume = volume;
		}

		public override void SetEmitter(AudioPositionInfo info)
		{
#if WARN_UNIMPLEMENTED_SOUND
			throw new NotImplementedException();
#endif // WARN_UNIMPLEMENTED_SOUND
		}

		public override void SetListeners(params AudioPositionInfo[] infos)
		{
#if WARN_UNIMPLEMENTED_SOUND
			throw new NotImplementedException();
#endif // WARN_UNIMPLEMENTED_SOUND
		}

		#endregion rUtil
	}
}
