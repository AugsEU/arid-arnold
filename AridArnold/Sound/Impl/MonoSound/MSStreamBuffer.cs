using MonoSound;
using MonoSound.Streaming;
using System;

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
			lock (mStreamPackage)
			{
				if (mStreamPackage.Disposed) return;
				mStreamPackage.Play();
			}
		}

		/// <summary>
		/// Resume playing
		/// </summary>
		public override void Resume()
		{
			lock (mStreamPackage)
			{
				if (mStreamPackage.Disposed) return;
				mStreamPackage.Resume();
			}
		}

		/// <summary>
		/// Pause playing.
		/// </summary>
		public override void Pause()
		{
			lock (mStreamPackage)
			{
				if (mStreamPackage.Disposed) return;
				mStreamPackage.Pause();
			}
		}

		/// <summary>
		/// Stop playing completely.
		/// </summary>
		public override void Stop()
		{
			lock (mStreamPackage)
			{
				if (mStreamPackage.Disposed) return;
				mStreamPackage.Stop();
			}
		}

		#endregion rPlay





		#region rUtil

		public override SoundState GetState()
		{
			lock (mStreamPackage)
			{
				if (mStreamPackage.Disposed || mStreamPackage.Metrics is null) return SoundState.Stopped;
				return mStreamPackage.Metrics.State;
			}
		}

		public override bool GetLoop()
		{
			lock (mStreamPackage)
			{
				if (mStreamPackage.Disposed) return false;
				return mStreamPackage.IsLooping;
			}
		}

		public override void SetLoop(bool loop)
		{
			lock (mStreamPackage)
			{
				if (mStreamPackage.Disposed) return;
				mStreamPackage.IsLooping = loop;
			}
		}

		public override float GetPan()
		{
			lock (mStreamPackage)
			{
				if (mStreamPackage.Disposed || mStreamPackage.Metrics is null) return 0.0f;
				return mStreamPackage.Metrics.Pan;
			}
		}

		public override void SetPan(float pan)
		{
			lock (mStreamPackage)
			{
				if (mStreamPackage.Disposed || mStreamPackage.Metrics is null) return;
				mStreamPackage.Metrics.Pan = pan;
			}
		}

		public override float GetPitch()
		{
			lock (mStreamPackage)
			{
				if (mStreamPackage.Disposed || mStreamPackage.Metrics is null) return 0.0f;
				return mStreamPackage.Metrics.Pitch;
			}
		}

		public override void SetPitch(float pitch)
		{
			lock (mStreamPackage)
			{
				if (mStreamPackage.Disposed || mStreamPackage.Metrics is null) return;
				mStreamPackage.Metrics.Pitch = pitch;
			}
		}

		public override float GetVolume()
		{
			lock (mStreamPackage)
			{
				if (mStreamPackage.Disposed || mStreamPackage.Metrics is null) return 0.0f;
				return mStreamPackage.Metrics.Volume;
			}
		}

		public override void SetVolume(float volume)
		{
			lock (mStreamPackage)
			{
				if (mStreamPackage.Disposed || mStreamPackage.Metrics is null) return;
				mStreamPackage.Metrics.Volume = volume;
			}
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
