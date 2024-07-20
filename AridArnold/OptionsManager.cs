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
		VisionOption mVision;
		float mMasterVolume;
		float mMusicVolume;
		bool mImpatientPlayer;

		public void Init()
		{
			SetDefaults();
		}

		public void SetDefaults()
		{
			mVision = VisionOption.kPerfect;
			mMasterVolume = 0.5f;
			mMusicVolume = 0.5f;
			mImpatientPlayer = false;
		}

		public VisionOption GetVision() { return mVision; }
		public void SetVision(VisionOption vision) { mVision = vision; }


		public float GetMasterVolume() { return mMasterVolume; }
		public void SetMasterVolume(float masterVolume) { mMasterVolume = masterVolume; }


		public float GetMusicVolume() { return mMusicVolume; }
		public void SetMusicVolume(float musicVolume) { mMusicVolume = musicVolume; }

		public bool GetImpatientPlayer() { return mImpatientPlayer; }
		public void SetImpatientPlayer(bool impatientPlayer) { mImpatientPlayer = impatientPlayer; }
	}
}
