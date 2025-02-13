﻿
namespace AridArnold
{
	internal class SmokeParticleSet : CollidingParticleSet
	{
		public const ushort SMOKE_LIFETIME = 240;

		public SmokeParticleSet() : base(
			"Particles/SmokeSet",
			new Point(3, 3),
			new byte[] { 0, 2, 1, 3, 4}, 
			33)
		{
		}

		public override void Update(GameTime gameTime)
		{
			for (int i = 0; i < mParticleHead; i++)
			{
				ref Particle particleRef = ref mParticles[i];
				
				// Make older particles get smaller
				if(particleRef.mLifetime < SMOKE_LIFETIME / 2 && particleRef.mLifetime % 20 == 0 && particleRef.mTextureIndex != 0)
				{
					particleRef.mTextureIndex -= 1;
				}
			}
			base.Update(gameTime);
		}
	}
}
