
namespace AridArnold
{
	class SmokeEmitter : ParticleEmitter
	{
		static Color[] COLOR_TABLE = new Color[]
		{
			new Color(0x919191u),
			new Color(0x565656u),
			new Color(0x2A2A2Au),
			new Color(0x202020u),
		};

		public SmokeEmitter(Vector2 source) : base(source)
		{
		}

		public override void Update(GameTime gameTime)
		{
			const float STACK_TIGHTNESS = 15.0f;
			const float EMIT_CHANCE = 6.0f;
			const float X_DIFF_VAR = 3.0f;

			MonoRandom rng = RandomManager.I.GetDraw();
			if(rng.PercentChance(EMIT_CHANCE))
			{
				byte textureIndex = (byte)(rng.GetIntRange(0, 4));
				Color color = COLOR_TABLE[rng.GetIntRange(0, COLOR_TABLE.Length-1)];

				float xDiff = rng.GetFloatRange(-X_DIFF_VAR, X_DIFF_VAR);
				Vector2 position = mSource;
				Vector2 vel = new Vector2(xDiff / STACK_TIGHTNESS, -(rng.GetUnitFloat() * 2.0f + 0.5f));
				position.X += xDiff;
			
				Particle newParticle = new Particle(color, position, vel, textureIndex, SmokeParticleSet.SMOKE_LIFETIME);
				ParticleManager.I.AddParticle(ref newParticle, ParticleManager.ParticleType.kSmoke);
			}
		}
	}
}
