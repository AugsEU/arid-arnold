
namespace AridArnold
{
	class PointSmokeEmitter : ParticleEmitter
	{
		Vector2 mPoint;

		public PointSmokeEmitter(Vector2 source, float emitStr = DEFAULT_EMIT_STR) : base(EmitterColors.SMOKE_COLORS, emitStr)
		{
			mPoint = source;
		}

		public PointSmokeEmitter(Vector2 source, Color[] palette, float emitStr = DEFAULT_EMIT_STR) : base(palette, emitStr)
		{
			mPoint = source;
		}

		public override void Update(GameTime gameTime)
		{
			const float X_DIFF_VAR = 0.2f;

			MonoRandom rng = RandomManager.I.GetDraw();
			if (ShouldTrigger(rng))
			{
				byte textureIndex = (byte)(rng.GetIntRange(0, 4));
				Color color = GetRndColor(rng);

				float xDiff = rng.GetFloatRange(-X_DIFF_VAR, X_DIFF_VAR);
				Vector2 position = mPoint;
				Vector2 vel = new Vector2(xDiff, -(rng.GetUnitFloat() * 2.0f + 0.5f));
				position.X += xDiff;

				Particle newParticle = new Particle(color, position, vel, textureIndex, SmokeParticleSet.SMOKE_LIFETIME);
				ParticleManager.I.AddParticle(ref newParticle, ParticleManager.ParticleType.kSmoke);
			}
		}

		public static PointSmokeEmitter FromXML(XmlNode node, Color[] palette, float intensity)
		{
			Vector2 position = MonoParse.GetVector(node);
			
			return new PointSmokeEmitter(position, palette, intensity);
		}


		public override void BindToNPC(SimpleTalkNPC npc)
		{
			Rect2f collider = npc.ColliderBounds();

			if (npc.GetPrevWalkDirection() == WalkDirection.Left)
			{
				mPoint.X = collider.Width - mPoint.X;
			}
			mPoint += collider.min;
		}
	}
}
