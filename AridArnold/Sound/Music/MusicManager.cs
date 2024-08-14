namespace AridArnold
{
	class MusicManager : Singleton<MusicManager>
	{
		const double MUSIC_FADE_OUT_LENGTH = 500.0;

		MusicManifest mManifest;
		MusicTrack mCurrentTrack;
		MusicTrack mNextTrack;



		/// <summary>
		/// Initialise music
		/// </summary>
		public void Init(string manifestFile)
		{
			mCurrentTrack = null;
			mNextTrack = null;
			mManifest = new MusicManifest(manifestFile);
		}



		/// <summary>
		/// Request a track to play. Pass null to request none music.
		/// </summary>
		public void RequestTrackPlay(string musicID)
		{
			musicID = musicID.ToLower();
			// If it's the same as we are already playing, do nothing.
			if (mCurrentTrack?.GetMusicID() == musicID)
			{
				// Cancel next track.
				mNextTrack = mCurrentTrack;
				return;
			}

			mNextTrack = mManifest.LoadTrack(musicID);
			mCurrentTrack?.Stop(MUSIC_FADE_OUT_LENGTH);
		}



		/// <summary>
		/// Update music
		/// </summary>
		public void Update(GameTime gameTime)
		{
			if (mCurrentTrack is not null)
			{
				mCurrentTrack.Update(gameTime);
				if (mCurrentTrack.GetBuffer().SoundState() == SoundState.Stopped)
				{
					// Dispose of this track.
					mCurrentTrack = null;
				}
			}

			// Load next track if there is one.
			if (mCurrentTrack is null && mNextTrack is not null)
			{
				mCurrentTrack = mNextTrack;
				mNextTrack = null;
				mCurrentTrack.Begin(0.0);
			}
		}
	}
}
