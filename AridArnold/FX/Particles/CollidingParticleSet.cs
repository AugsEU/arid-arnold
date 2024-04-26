
using System.Net;

namespace AridArnold
{
	abstract class CollidingParticleSet : ParticleSet
	{
		protected CollidingParticleSet(string texturePath, Point baseSize, byte[] transitionTable, int animFrameLength, ushort particleLifetime) : base(texturePath, baseSize, transitionTable, animFrameLength, particleLifetime)
		{
		}

		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);
			for (int i = 0; i < mParticleHead; i++)
			{
				ref Particle particleRef = ref mParticles[i];
				Vector2 probePos = particleRef.mPosition;
				probePos.X += dt * particleRef.mVelocity.X;
				if(TileManager.I.GetTile(probePos).IsSolid())
				{
					particleRef.mVelocity.X = 0.0f;
				}

				probePos = particleRef.mPosition;
				probePos.Y += dt * particleRef.mVelocity.Y;
				if (TileManager.I.GetTile(probePos).IsSolid())
				{
					particleRef.mVelocity.Y = 0.0f;
				}
			}
			base.Update(gameTime);
		}
	}
}
