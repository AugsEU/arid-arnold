
namespace AridArnold
{
	class ProfileSaveInfo : MonoReadWriteFile
	{
		public enum ReadMode
		{
			kMetaOnly,
			kFull
		}

		public const string PROFILE_SAVE_FOLDER = "profileSave/";
		const string PROFILE_SAVE_MAGIC = "pas";
		const int PROFILE_SAVE_VER = 1;
		const int RELOAD_FRAME_PENALTY = 121; // 2 seconds added just for reloading to avoid weird strats.

		ReadMode mReadMode;

		// Basic data
		string mProfileName;
		int mNumInputUpdates;

		public ProfileSaveInfo(string fileName) : base(fileName, true)
		{
			mReadMode = ReadMode.kFull;
		}

		public void SetReadMode(ReadMode readMode)
		{
			mReadMode = readMode;
		}

		public void SetProfileName(string profileName)
		{
			mProfileName = profileName;
		}

		public string GetProfileName()
		{
			return mProfileName;
		}

		public string GetSaveTimeStr()
		{
			return MonoText.GetTimeTextFromFrames(mNumInputUpdates);
		}

		protected override string GetRelativeFolder()
		{
			return PROFILE_SAVE_FOLDER;
		}

		protected override void ReadBinary(BinaryReader br)
		{
			string magic = br.ReadString();
			int fileVer = br.ReadInt32();

			MonoDebug.Assert(magic == "pas");
			MonoDebug.Assert(fileVer == PROFILE_SAVE_VER);

			mProfileName = br.ReadString();
			mNumInputUpdates = br.ReadInt32();

			if(mReadMode == ReadMode.kMetaOnly)
			{
				// Don't set the state
				return;
			}

			TimeZoneManager.I.ReadBinary(br);
			CollectableManager.I.ReadBinary(br);
			FlagsManager.I.ReadBinary(br);
			CampaignManager.I.ReadBinary(br);

			InputManager.I.LoadInputFrames(mNumInputUpdates + RELOAD_FRAME_PENALTY);
		}

		protected override void WriteBinary(BinaryWriter bw)
		{
			// To do: get rid of this?
			mNumInputUpdates = InputManager.I.GetNumberOfInputFrames();

			bw.Write(PROFILE_SAVE_MAGIC);
			bw.Write(PROFILE_SAVE_VER);

			bw.Write(mProfileName);
			bw.Write(mNumInputUpdates);

			TimeZoneManager.I.WriteBinary(bw);
			CollectableManager.I.WriteBinary(bw);
			FlagsManager.I.WriteBinary(bw);
			CampaignManager.I.WriteBinary(bw);
		}
	}
}
