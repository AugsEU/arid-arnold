namespace AridArnold
{
	/// <summary>
	/// MonoGame's version of a streamed buffer. Can only have 1 active instance at a time.
	/// </summary>
	class MGStreamBuffer : AudioBuffer
	{
		#region rMembers

		static Song mCurrMediaSong = null;
		static Dictionary<string, Song> mSongCache = new Dictionary<string, Song>();

		bool mLooped = true;
		float mVolume = 1.0f;
		Song mSong;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create streamed buffer
		/// </summary>
		public MGStreamBuffer(string path)
		{
			if(!mSongCache.TryGetValue(path, out mSong))
			{
				mSong = MonoData.I.MonoGameLoad<Song>(path);
				mSongCache[path] = mSong;
			}
		}

		#endregion rInit





		#region rPlay

		/// <summary>
		/// Play the stream.
		/// </summary>
		public override void Play()
		{
			if (mSong is null)
			{
				return;
			}

			MediaPlayer.Volume = mVolume;
			MediaPlayer.IsRepeating = false;
			MediaPlayer.Play(mSong);

			mCurrMediaSong = mSong;
		}

		/// <summary>
		/// Resume playing
		/// </summary>
		public override void Resume()
		{
			MediaPlayer.Resume();
		}

		/// <summary>
		/// Pause playing.
		/// </summary>
		public override void Pause()
		{
			MediaPlayer.Pause();
		}

		/// <summary>
		/// Stop playing completely.
		/// </summary>
		public override void Stop()
		{
			MediaPlayer.Stop();
			mCurrMediaSong = null;
		}

		#endregion rPlay





		#region rUtil

		public override SoundState GetState()
		{
			if (IsUsPlaying())
			{
				return SoundState.Playing;
			}

			switch (MediaPlayer.State)
			{
				case MediaState.Stopped:
					return SoundState.Stopped;
				case MediaState.Playing:
					return SoundState.Playing;
				case MediaState.Paused:
					return SoundState.Paused;
				default:
					break;
			}

			return SoundState.Stopped;
		}

		public override bool GetLoop()
		{
			return mLooped;
		}

		public override void SetLoop(bool loop)
		{
			mLooped = loop;
			if(IsUsPlaying())
			{
				MediaPlayer.IsRepeating = true;
			}
		}

		public override float GetPan()
		{
			return 0.0f;
		}

		public override void SetPan(float pan)
		{
#if WARN_UNIMPLEMENTED_SOUND
			throw new NotImplementedException();
#endif // WARN_UNIMPLEMENTED_SOUND
		}

		public override float GetPitch()
		{
			return 0.0f;
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
				MediaPlayer.Volume = volume;
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

		/// <summary>
		/// Are we currently playing?
		/// </summary>
		private bool IsUsPlaying()
		{
			if (mSong is null || mCurrMediaSong is null)
			{
				return false;
			}
			return mSong.Name == mCurrMediaSong.Name;
		}

		#endregion rUtil
	}
}
