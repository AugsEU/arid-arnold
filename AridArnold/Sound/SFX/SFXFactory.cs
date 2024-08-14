namespace AridArnold
{
	/// <summary>
	/// Holds data and creates a sound effect
	/// </summary>
	class SFXFactory
	{
		AridArnoldSFX mSFXType;
		float mVolume;
		float mPitchMin;
		float mPitchMax;

		public SFXFactory(AridArnoldSFX SFXType, float volume, float pitchMin = 0.0f, float pitchMax = 0.0f)
		{
			mSFXType = SFXType;
			mVolume = volume;
			mPitchMin = pitchMin;
			mPitchMax = pitchMax;
		}

		public GameSFX CreateSFX()
		{
			return new GameSFX(mSFXType, mVolume, mPitchMin, mPitchMax);
		}

		public SpacialSFX CreateSpacialSFX(Vector2 pos)
		{
			return new SpacialSFX(mSFXType, pos, mVolume, mPitchMin, mPitchMax);
		}
	}
}
