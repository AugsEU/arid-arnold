using MonoSound;
using MonoSound.Streaming;
using System;

namespace AridArnold
{
	internal class MSStreamBuffer : AudioBuffer
	{
		#region rMembers

		StreamPackage mStreamPackage;
		bool mHasStopped;

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
				if (StreamValid()) return;
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
				if (StreamValid()) return;
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
				if (StreamValid()) return;
				mStreamPackage.Pause();
			}
		}

		/// <summary>
		/// Stop playing completely.
		/// </summary>
		public override void Stop()
		{
			mHasStopped = true;
			lock (mStreamPackage)
			{
				if (StreamValid()) return;
				mStreamPackage.Stop();
			}
		}

		#endregion rPlay





		#region rUtil

		public override SoundState GetState()
		{
			lock (mStreamPackage)
			{
				if (StreamValid()) return SoundState.Stopped;
				return mStreamPackage.Metrics.State;
			}
		}

		public override bool GetLoop()
		{
			lock (mStreamPackage)
			{
				if (StreamValid()) return false;
				return mStreamPackage.IsLooping;
			}
		}

		public override void SetLoop(bool loop)
		{
			lock (mStreamPackage)
			{
				if (StreamValid()) return;
				mStreamPackage.IsLooping = loop;
			}
		}

		public override float GetPan()
		{
			lock (mStreamPackage)
			{
				if (StreamValid()) return 0.0f;
				return mStreamPackage.Metrics.Pan;
			}
		}

		public override void SetPan(float pan)
		{
			lock (mStreamPackage)
			{
				if (StreamValid()) return;
				mStreamPackage.Metrics.Pan = pan;
			}
		}

		public override float GetPitch()
		{
			lock (mStreamPackage)
			{
				if (StreamValid()) return 0.0f;
				return mStreamPackage.Metrics.Pitch;
			}
		}

		public override void SetPitch(float pitch)
		{
			lock (mStreamPackage)
			{
				if (StreamValid()) return;
				mStreamPackage.Metrics.Pitch = pitch;
			}
		}

		public override float GetVolume()
		{
			lock (mStreamPackage)
			{
				if (StreamValid()) return 0.0f;
				return mStreamPackage.Metrics.Volume;
			}
		}

		public override void SetVolume(float volume)
		{
			lock (mStreamPackage)
			{
				if (StreamValid()) return;
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

		private bool StreamValid()
		{
			lock (mStreamPackage)
			{
				if(mHasStopped)
				{
					return false;
				}
				return mStreamPackage is not null && mStreamPackage.Metrics is not null && !mStreamPackage.Disposed && mStreamPackage.TotalBytes > 0;
			}
		}

			#endregion rUtil
		}
	}
