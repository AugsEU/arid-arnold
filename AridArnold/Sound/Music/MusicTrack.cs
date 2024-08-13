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
		}

		public string GetMusicID()
		{
			return mID;
		}
	}
}
