﻿namespace AridArnold
{
	/// <summary>
	/// Manages particles, designed for performance.
	/// </summary>
	internal class ParticleManager : Singleton<ParticleManager>
	{
		#region rTypes

		public enum ParticleType
		{
			kSmoke
		}

		#endregion rTypes





		#region rMembers

		ParticleSet[] mParticleSets;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create all particle info
		/// </summary>
		public void Init()
		{
			mParticleSets = new ParticleSet[MonoAlg.EnumLength(typeof(ParticleType))];
			mParticleSets[(int)ParticleType.kSmoke] = new SmokeParticleSet();
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update all particles
		/// </summary>
		public void Update(GameTime gameTime)
		{
			for(int i = 0; i < mParticleSets.Length; i++)
			{
				mParticleSets[i].Update(gameTime);
			}
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw all particles
		/// </summary>
		public void Draw(DrawInfo info)
		{
			for (int i = 0; i < mParticleSets.Length; i++)
			{
				mParticleSets[i].Draw(info);
			}
		}

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Add particle to be draw
		/// </summary>
		public void AddParticle(ref Particle particle, ParticleType particleType)
		{
			mParticleSets[(int)particleType].AddParticle(ref particle);
		}



		/// <summary>
		/// Delete all particles
		/// </summary>
		public void Clear()
		{
			for (int i = 0; i < mParticleSets.Length; i++)
			{
				mParticleSets[i].Clear();
			}
		}

		#endregion rUtility
	}
}