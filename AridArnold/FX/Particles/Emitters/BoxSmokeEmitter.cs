﻿
namespace AridArnold
{
	class BoxSmokeEmitter : ParticleEmitter
	{
		Rect2f mRect;
		MonoTimer mLockoutTimer;

		public BoxSmokeEmitter(Rect2f box, float emitStr = DEFAULT_EMIT_STR) : base(EmitterColors.SMOKE_COLORS, emitStr)
		{
			mRect = box;
			mLockoutTimer = new MonoTimer();
			mLockoutTimer.Start();
		}

		public BoxSmokeEmitter(Rect2f box, Color[] palette, float emitStr = DEFAULT_EMIT_STR) : base(palette, emitStr)
		{
			mRect = box;
			mLockoutTimer = new MonoTimer();
			mLockoutTimer.Start();
		}

		public override void Update(GameTime gameTime)
		{
			MonoRandom rng = RandomManager.I.GetDraw();

			mLockoutTimer.Update(gameTime);
			if (mLockoutTimer.GetElapsedMs() < rng.GetFloatRange(60.0f, 160.0f))
			{
				return;
			}

			mLockoutTimer.ResetStart();

			const float X_DIFF_VAR = 0.2f;

			float triggerTimes = MathF.Ceiling(mRect.Height * mRect.Width * (mIntensity / 100.0f));

			for(float t = 0.0f; t < triggerTimes; t++)
			{
				Vector2 position = rng.PointIn(mRect);
				if (rng.PercentChance(50.0f))
				{
					byte textureIndex = (byte)(rng.GetIntRange(0, 4));
					Color color = GetRndColor(rng);

					float xDiff = rng.GetFloatRange(-X_DIFF_VAR, X_DIFF_VAR);

					Vector2 vel = new Vector2(xDiff, -(rng.GetUnitFloat() * 2.0f + 0.5f));
					Particle newParticle = new Particle(color, position, vel, textureIndex, SmokeParticleSet.SMOKE_LIFETIME);
					ParticleManager.I.AddParticle(ref newParticle, ParticleManager.ParticleType.kSmoke);
				}
			}
		}

		public static BoxSmokeEmitter FromXML(XmlNode node, Color[] palette, float intensity)
		{
			Rect2f box = MonoParse.GetRect2f(node);

			return new BoxSmokeEmitter(box, palette, intensity);
		}

		public override void BindToNPC(SimpleTalkNPC npc)
		{
			Rect2f collider = npc.ColliderBounds();

			Vector2 offset = mRect.max - mRect.min;

			Vector2 origin = mRect.min;
			if (npc.GetPrevWalkDirection() == WalkDirection.Left)
			{
				origin.X = collider.Width - mRect.Width - origin.X;
			}
			origin += collider.min;

			mRect.min = origin;
			mRect.max = origin + offset;
		}


		public override void MoveTo(Vector2 pos)
		{
			Vector2 offset = mRect.max - mRect.min;
			mRect.min = pos;
			mRect.max = pos + offset;
		}
	}
}
