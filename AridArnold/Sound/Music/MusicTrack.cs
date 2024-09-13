namespace AridArnold
{
	/// <summary>
	/// Represents a music track.
	/// </summary>
	class MusicTrack : BufferPlayer
	{
		string mID;

		public MusicTrack(string id, MusicManifestEntry data) : base(MonoSound.Impl.LoadAudioBuffer(data.mFileName), data.mVolume)
		{
			mID = id;
			
			GetBuffer().SetLoop(!data.mNoLoop);
		}

		public string GetMusicID()
		{
			return mID;
		}

		protected override float DecideVolume()
		{
			float musicVol = OptionsManager.I.GetMusicVolume();
			return base.DecideVolume() * musicVol;
		}
	}
}
