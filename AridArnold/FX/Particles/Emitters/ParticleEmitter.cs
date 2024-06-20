namespace AridArnold
{
	static class EmitterColors
	{
		public static Color[] SMOKE_COLORS = new Color[]
		{
			new Color(0x919191u),
			new Color(0x565656u),
			new Color(0x2A2A2Au),
			new Color(0x202020u),
		};
	}

	abstract class ParticleEmitter
	{
		public const float DEFAULT_EMIT_STR = 6.0f;
		Color[] mPalette;
		float mIntensity;

		public ParticleEmitter(Color[] palette, float intensity = DEFAULT_EMIT_STR)
		{
			mPalette = palette;
			mIntensity = intensity;
		}

		protected Color GetRndColor(MonoRandom rng)
		{
			return mPalette[rng.GetIntRange(0, mPalette.Length - 1)];
		}

		protected bool ShouldTrigger(MonoRandom rng)
		{
			return rng.PercentChance(mIntensity);
		}

		public abstract void Update(GameTime gameTime);
	}
}
