
namespace AridArnold
{
	class ProfileSaveInfo : MonoReadWriteFile, IComparable<ProfileSaveInfo>
	{
		public enum ReadMode
		{
			kMetaOnly,
			kFull
		}

		public const string PROFILE_SAVE_FOLDER = "profileSave/";
		const string PROFILE_SAVE_MAGIC = "pas";
		public const int PROFILE_SAVE_VER = 2;
		const UInt64 RELOAD_FRAME_PENALTY = 121; // 2 seconds added just for reloading to avoid weird strats.

		ReadMode mReadMode;

		// Basic data
		string mProfileName = "Debug";
		UInt64 mNumInputUpdates = 0;

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

		public int CompareTo(ProfileSaveInfo other)
		{
			if (other == null)
				return 1;

			return other.mNumInputUpdates.CompareTo(mNumInputUpdates);
		}

		protected override void ReadBinary(BinaryReader br)
		{
			string magic = br.ReadString();
			int fileVer = br.ReadInt32();

			MonoDebug.Assert(magic == "pas");
			MonoDebug.Assert(fileVer == PROFILE_SAVE_VER);

			mProfileName = br.ReadString();
			mNumInputUpdates = br.ReadUInt64();

			if(mReadMode == ReadMode.kMetaOnly)
			{
				// Don't set the state
				return;
			}

			TimeZoneManager.I.ReadBinary(br);
			CollectableManager.I.ReadBinary(br);
			FlagsManager.I.ReadBinary(br);
			CampaignManager.I.ReadBinary(br, fileVer);

			ArcadeGameScreen arcadeGameScreen = ScreenManager.I.GetScreen<ArcadeGameScreen>();
			arcadeGameScreen.ReadBinary(br);

			InputManager.I.LoadInputFrames(mNumInputUpdates + RELOAD_FRAME_PENALTY);

			
		}

		protected override void WriteBinary(BinaryWriter bw)
		{
			mNumInputUpdates = InputManager.I.GetNumberOfInputFrames();

			bw.Write(PROFILE_SAVE_MAGIC);
			bw.Write(PROFILE_SAVE_VER);

			bw.Write(mProfileName);
			bw.Write(mNumInputUpdates);

			TimeZoneManager.I.WriteBinary(bw);
			CollectableManager.I.WriteBinary(bw);
			FlagsManager.I.WriteBinary(bw);
			CampaignManager.I.WriteBinary(bw);

			ArcadeGameScreen arcadeGameScreen = ScreenManager.I.GetScreen<ArcadeGameScreen>();
			arcadeGameScreen.WriteBinary(bw);
		}
	}
}
