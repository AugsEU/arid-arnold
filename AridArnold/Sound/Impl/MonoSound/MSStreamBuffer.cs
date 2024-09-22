using MonoSound;
using MonoSound.Streaming;
using System;
using System.IO;

namespace AridArnold
{
	internal class MSStreamBuffer : AudioBuffer
	{
		#region rMembers

		static StreamPackage sStreamingSound;
		static string sLastPlayedPath;

		string mPath;
		bool mLooped;
		float mVolume;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create streamed buffer
		/// </summary>
		public MSStreamBuffer(string path)
		{
			mPath = path;
			mLooped = true;
			mVolume = 1.0f;
			
		}

		#endregion rInit





		#region rPlay

		/// <summary>
		/// Play the stream.
		/// </summary>
		public override void Play()
		{
			if(IsUsPlaying())
			{
				return;
			}

			if(sStreamingSound is not null)
			{
				sStreamingSound.Dispose();
				sStreamingSound = null;
			}

			sStreamingSound = StreamLoader.GetStreamedSound(mPath, mLooped);
			sLastPlayedPath = mPath;

			if (!StreamValid()) return;
			sStreamingSound.Metrics.Volume = mVolume;
			sStreamingSound.Play();
		}

		/// <summary>
		/// Resume playing
		/// </summary>
		public override void Resume()
		{
			if (!StreamValid()) return;
			sStreamingSound.Resume();
		}

		/// <summary>
		/// Pause playing.
		/// </summary>
		public override void Pause()
		{
			if (!StreamValid()) return;
			sStreamingSound.Pause();
		}

		/// <summary>
		/// Stop playing completely.
		/// </summary>
		public override void Stop()
		{
			if (!StreamValid()) return;
			sStreamingSound.Stop();
			sLastPlayedPath = "";
		}

		#endregion rPlay





		#region rUtil

		public override SoundState GetState()
		{
			if(!IsUsPlaying())
			{
				return SoundState.Stopped;
			}

			if (!StreamValid()) return SoundState.Stopped;
			return sStreamingSound.Metrics.State;
		}

		public override bool GetLoop()
		{
			return mLooped;
		}

		public override void SetLoop(bool loop)
		{
			mLooped = loop;

			if (IsUsPlaying())
			{
				if (!StreamValid()) return;
				sStreamingSound.IsLooping = loop;
			}
		}

		public override float GetPan()
		{
#if WARN_UNIMPLEMENTED_SOUND
			throw new NotImplementedException();
#else
			return 0.0f;
#endif // WARN_UNIMPLEMENTED_SOUND

		}

		public override void SetPan(float pan)
		{
#if WARN_UNIMPLEMENTED_SOUND
			throw new NotImplementedException();
#endif // WARN_UNIMPLEMENTED_SOUND
		}

		public override float GetPitch()
		{
#if WARN_UNIMPLEMENTED_SOUND
			throw new NotImplementedException();
#else
			return 0.0f;
#endif // WARN_UNIMPLEMENTED_SOUND
		}

		public override void SetPitch(float pitch)
		{
#if WARN_UNIMPLEMENTED_SOUND
			throw new NotImplementedException();
#endif // WARN_UNIMPLEMENTED_SOUND
		}

		public override float GetVolume()
		{
			return mVolume;
		}

		public override void SetVolume(float volume)
		{
			mVolume = volume;

			if(IsUsPlaying())
			{
				if (!StreamValid()) return;
				sStreamingSound.Metrics.Volume = volume;
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
			lock (sStreamingSound)
			{
				return sStreamingSound is not null && !sStreamingSound.Disposed && sStreamingSound.Metrics is not null;
			}
		}

		private bool IsUsPlaying()
		{
			return mPath == sLastPlayedPath;
		}

		#endregion rUtil
	}
}
