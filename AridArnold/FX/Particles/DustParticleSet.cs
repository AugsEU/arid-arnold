
namespace AridArnold
{
	internal class DustParticleSet : ParticleSet
	{
		public const ushort DUST_LIFETIME = 40;

		public DustParticleSet() : base(
			"Particles/DustSet",
			new Point(3, 3),
			new byte[] { 0, 2, 1 }, 
			20)
		{
		}

		public override void Update(GameTime gameTime)
		{
			for (int i = 0; i < mParticleHead; i++)
			{
				ref Particle particleRef = ref mParticles[i];
				
				// Make older particles get smaller
				if(particleRef.mLifetime < DUST_LIFETIME / 2 && particleRef.mLifetime % 20 == 0 && particleRef.mTextureIndex != 0)
				{
					particleRef.mTextureIndex -= 1;
				}
			}
			base.Update(gameTime);
		}
	}
}
