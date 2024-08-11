using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AridArnold.Sound.SFX
{
	enum GameSFXType
	{
		GameSFX,
		SpacialSFX
	}

	/// <summary>
	/// Holds data and creates a sound effect
	/// </summary>
	class SFXFactory
	{
		GameSFXType mType;
		AridArnoldSFX mSFXType;
		float mVolume;
		float mPitchMin;
		float mPitchMax;

		public SFXFactory(GameSFXType type, AridArnoldSFX SFXType, float volume, float pitchMin = 0.0f, float pitchMax = 0.0f)
		{
			mType = type;
			mSFXType = SFXType;
			mVolume = volume;
			mPitchMin = pitchMin;
			mPitchMax = pitchMax;
		}

		public GameSFX CreateSFX()
		{
			return CreateSFX(Vector2.Zero);
		}

		public GameSFX CreateSFX(Vector2 pos)
		{
			switch (mType)
			{
				case GameSFXType.GameSFX:
					return new GameSFX(mSFXType, mVolume, mPitchMin, mPitchMax);
				case GameSFXType.SpacialSFX:
					return new SpacialSFX(mSFXType, pos, mVolume, mPitchMin, mPitchMax);
				default:
					break;
			}

			throw new NotImplementedException();
		}
	}
}
