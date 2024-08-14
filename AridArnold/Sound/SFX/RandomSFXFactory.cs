namespace AridArnold
{
	/// <summary>
	/// Holds data and creates a sound effect
	/// </summary>
	class RandomSFXFactory
	{
		List<SFXFactory> mSFXFactories;

		public RandomSFXFactory()
		{
			mSFXFactories = new List<SFXFactory>();
		}

		public void AddSFX(AridArnoldSFX SFXType, float volume, float pitchMin = 0.0f, float pitchMax = 0.0f)
		{
			mSFXFactories.Add(new SFXFactory(SFXType, volume, pitchMin, pitchMax));
		}

		public GameSFX CreateSFX()
		{
			return GetRandomFactory().CreateSFX();
		}

		public SpacialSFX CreateSpacialSFX(Vector2 pos)
		{
			return GetRandomFactory().CreateSpacialSFX(pos);
		}

		private SFXFactory GetRandomFactory()
		{
			MonoRandom random = RandomManager.I.GetDraw();

			return random.InList(mSFXFactories);
		}
	}
}
