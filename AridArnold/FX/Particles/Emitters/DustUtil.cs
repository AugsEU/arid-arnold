
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

		public static Color[] MIRROR_COLORS = new Color[]
		{
			new Color(0xD8D3B1u),
			new Color(0xCFD1CFu),
			new Color(0xFBFEFBu)
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

			Color color = colors[RandomManager.I.GetDraw().GetIntRange(0, colors.Length - 1)];

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


		public static void EmitSwooshLine(Vector2 start, Vector2 end, Vector2 dir, Color[] colors)
		{
			Vector2 normDir = dir;
			normDir.Normalize();

			MonoRandom rng = RandomManager.I.GetDraw();
			float len = (start - end).Length();
			for(float x = 0; x <= len; x += 2.0f)
			{
				float t = x / len;
				int numInColumn = rng.GetIntRange(0, 4);
				Vector2 basePosition = MonoMath.Lerp(start, end, t);
				
				for(int i = 0; i < numInColumn; i++)
				{
					Vector2 offset = rng.GetUnitFloat() * dir;

					EmitDust(basePosition + offset, normDir, colors, x * 0.1f);
				}
			}
		}

		public static void EmitInSquare(Rect2f rect, int numMin, int numMax, Color[] colors)
		{
			MonoRandom rng = RandomManager.I.GetDraw();

			int numToEmit = rng.GetIntRange(numMin, numMax);

			for(int i = 0; i < numToEmit; i++)
			{
				Vector2 point = rng.PointIn(rect);
				Vector2 vel = new Vector2(rng.GetFloatRange(-1.0f, 1.0f), rng.GetFloatRange(-1.0f, 1.0f));
				vel *= 0.20f;

				EmitDust(point, vel, colors, 1.0f);
			}
		}
	}
}
