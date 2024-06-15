
namespace AridArnold
{
	static class DustUtil
	{
		static Color[] DUST_COLORS = new Color[]
		{
			new Color(0x919191u),
			new Color(0xA5A5A5u),
			new Color(0xB7B7B7u)
		};

		static Color[] SPORE_COLORS = new Color[]
		{
			new Color(0xC9C9ABu),
			new Color(0xA79437u),
			new Color(0x71A337u)
		};

		/// <summary>
		/// Emit dust particle that go in the direction
		/// </summary>
		public static void EmitDust(Vector2 pos, Vector2 dir)
		{
			EmitDust(pos, dir, DUST_COLORS, 1.0f);
		}



		/// <summary>
		/// Emit dust particle that go in direction
		/// </summary>
		public static void EmitDust(Vector2 pos, Vector2 dir, Color[] colors, float xdiff)
		{
			MonoRandom rng = RandomManager.I.GetDraw();
			byte textureIndex = (byte)(rng.GetIntRange(0, 2));

			Color color = colors[RandomManager.I.GetDraw().GetIntRange(0, DUST_COLORS.Length - 1)];

			float xDiff = rng.GetFloatRange(-xdiff, xdiff);
			Vector2 position = pos;
			position.X -= 1.5f;
			position.Y -= 1.5f;

			Vector2 vel = dir * (rng.GetUnitFloat() * 0.5f + 0.4f);
			vel += MonoMath.Perpendicular(vel) * xDiff;

			ushort lifetime = (ushort)(DustParticleSet.DUST_LIFETIME + rng.GetIntRange(5, 10) * (textureIndex + 1));

			Particle newParticle = new Particle(color, position, vel, textureIndex, lifetime);
			ParticleManager.I.AddParticle(ref newParticle, ParticleManager.ParticleType.kDust);
		}



		/// <summary>
		/// Emit blue spore from mushroom
		/// </summary>
		public static void EmitSpore(Vector2 pos, Vector2 dir)
		{
			EmitDust(pos, dir, SPORE_COLORS, 2.2f);
		}

		
	}
}
