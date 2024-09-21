using System.Threading;

namespace AridArnold
{
	enum VisionOption
	{
		kPerfect = 0,
		kStretch = 1
	}

	class OptionsManager : Singleton<OptionsManager>
	{
		#region rMembers

		VisionOption mVision;
		float mSFXVolume;
		float mMusicVolume;
		bool mImpatientPlayer;
		bool mGhosts;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Initialise options with defaults
		/// </summary>
		public void Init()
		{
			SetDefaults();
		}



		/// <summary>
		/// Reset all settings to default
		/// </summary>
		public void SetDefaults()
		{
			mVision = VisionOption.kPerfect;
			mSFXVolume = 0.5f;
			mMusicVolume = 0.3f;
			mImpatientPlayer = false;
			mGhosts = true;
		}

		#endregion rInit


		#region rSGetter

		public VisionOption GetVision() { return mVision; }
		public void SetVision(VisionOption vision) { mVision = vision; }


		public float GetSFXVolume() { return mSFXVolume; }
		public void SetSFXVolume(float sfxVolume) { mSFXVolume = sfxVolume; }


		public float GetMusicVolume() { return mMusicVolume; }
		public void SetMusicVolume(float musicVolume) { mMusicVolume = musicVolume; }

		public bool GetImpatientPlayer() { return mImpatientPlayer; }
		public void SetImpatientPlayer(bool impatientPlayer) { mImpatientPlayer = impatientPlayer; }

		public bool GetGhostDisplay() { return mGhosts; }
		public void SetGhostDisplay(bool ghosts) { mGhosts = ghosts; }

		#endregion rSGetter





		#region rSerial

		/// <summary>
		/// Read binary segment
		/// </summary>
		public void ReadFromBinary(BinaryReader br)
		{
			mVision = (VisionOption)br.ReadInt32();
			mSFXVolume = br.ReadSingle();
			mMusicVolume = br.ReadSingle();
			mImpatientPlayer = br.ReadBoolean();

			bool isFullScreen = br.ReadBoolean();
			Main.SetFullScreen(isFullScreen);
		}



		/// <summary>
		/// Write binary segment
		/// </summary>
		public void WriteFromBinary(BinaryWriter bw)
		{
			bw.Write((Int32)mVision);
			bw.Write(mSFXVolume);
			bw.Write(mMusicVolume);
			bw.Write(mImpatientPlayer);

			bool isFullScreen = Main.IsFullScreen();
			bw.Write(isFullScreen);
		}

		#endregion rSerial
	}
}
