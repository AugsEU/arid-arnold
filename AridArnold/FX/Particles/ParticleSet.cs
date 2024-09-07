using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AridArnold
{
	internal class ParticleSet
	{
		#region rConstants

		const int MAX_PARTICLES = 512;

		#endregion rConstants





		#region rMembers

		protected Particle[] mParticles;
		protected int mParticleHead;

		byte[] mTransitionTable;
		int mAnimFrameLength;
		Texture2D mTexture;
		Point mBaseSize;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create particle set
		/// </summary>
		public ParticleSet(string texturePath, Point baseSize, byte[] transitionTable, int animFrameLength)
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>(texturePath);
			mTransitionTable = transitionTable;
			mBaseSize = baseSize;
			mAnimFrameLength = animFrameLength;

			mParticles = new Particle[MAX_PARTICLES];
			mParticleHead = 0;
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update particles within this set
		/// </summary>
		public virtual void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);
			for(int i = 0; i < mParticleHead; i++)
			{
				ref Particle particleRef = ref mParticles[i];
				particleRef.mPosition += dt * particleRef.mVelocity;

				ushort particleAge = particleRef.mLifetime;
				if(particleAge == 0 || FXManager.I.OutsideFXRegion(particleRef.mPosition))
				{
					RemoveParticle(i);
					i--;
					continue;
				}
				else
				{
					particleRef.mLifetime--;
				}
				
				if(particleAge % mAnimFrameLength == 0)
				{
					particleRef.mTextureIndex = mTransitionTable[particleRef.mTextureIndex];
				}
			}
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw all particles within set.
		/// </summary>
		public void Draw(DrawInfo info)
		{
			Rectangle baseRect = new Rectangle(Point.Zero, mBaseSize);

			for (int i = 0; i < mParticleHead; i++)
			{
				ref Particle particleRef = ref mParticles[i];

				baseRect.X = mBaseSize.X * particleRef.mTextureIndex;
				MonoDraw.DrawTexture(info, mTexture, particleRef.mPosition, baseRect, particleRef.mColor, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, DrawLayer.Particle);
			}
		}

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Add particle to list
		/// </summary>
		public void AddParticle(ref Particle particle)
		{
			if(mParticleHead >= MAX_PARTICLES)
			{
				MonoDebug.Log("PARTICLE MAX EXCEEDED. Consider upping maximum");
				return;
			}

			mParticles[mParticleHead] = particle;
			mParticleHead++;
		}



		/// <summary>
		/// Remove particle from list
		/// </summary>
		public void RemoveParticle(int index)
		{
			if(index != mParticleHead-1)
			{
				// Move end to the index we are deleting to overwrite it.
				mParticles[index] = mParticles[mParticleHead - 1];
			}

			mParticleHead--;
		}



		/// <summary>
		/// Clear all particles
		/// </summary>
		public void Clear()
		{
			mParticleHead = 0;
		}

		#endregion rUtility
	}
}
