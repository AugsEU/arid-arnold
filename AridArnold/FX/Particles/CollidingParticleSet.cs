
using System.Net;

namespace AridArnold
{
	abstract class CollidingParticleSet : ParticleSet
	{
		protected CollidingParticleSet(string texturePath, Point baseSize, byte[] transitionTable, int animFrameLength) : base(texturePath, baseSize, transitionTable, animFrameLength)
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

				Tile localTile = TileManager.I.GetTile(probePos);
				if (localTile is not null && localTile.IsSolid())
				{
					particleRef.mVelocity.X = 0.0f;
				}

				probePos = particleRef.mPosition;
				probePos.Y += dt * particleRef.mVelocity.Y;
				localTile = TileManager.I.GetTile(probePos);
				if (localTile is not null && localTile.IsSolid())
				{
					particleRef.mVelocity.Y = 0.0f;
				}
			}
			base.Update(gameTime);
		}
	}
}
