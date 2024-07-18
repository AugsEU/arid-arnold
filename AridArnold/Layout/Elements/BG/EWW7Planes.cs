
// Bespoke classes for the WW7 background. Not done "properly" but weirdness is contained so it's fine.
namespace AridArnold
{
	internal class EWW7Planes : LayElement
	{
		const float LOCK_OUT_MIN = 1000.0f;
		const float LOCK_OUT_MAX = 4000.0f;

		List<WW7Plane> mPlanes;
		MonoTimer mPlaneLockoutTimer;
		float mCurrentLockout = LOCK_OUT_MIN;
		AnimationData[] mPlaneAnimators;

		public EWW7Planes(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			mPlanes = new List<WW7Plane>();
			mPlaneLockoutTimer = new MonoTimer();
			ResetLockoutTimer();

			mPlaneAnimators = new AnimationData[] {
				MonoData.I.LoadAnimatorData("BG/WW7/FighterJet.max"),
				MonoData.I.LoadAnimatorData("BG/WW7/Helicopter.max"),
				MonoData.I.LoadAnimatorData("BG/WW7/UFO.max")
			};

			SpawnPlane();
			SpawnPlane();
			mPlanes[0].mPos.X = RandomManager.I.GetDraw().GetFloatRange(50.0f, 500.0f);
		}

		void ResetLockoutTimer()
		{
			mCurrentLockout = RandomManager.I.GetDraw().GetFloatRange(LOCK_OUT_MIN, LOCK_OUT_MAX);
			mPlaneLockoutTimer.FullReset();
			mPlaneLockoutTimer.Start();
		}

		public override void Update(GameTime gameTime)
		{
			mPlaneLockoutTimer.Update(gameTime);

			if (mCurrentLockout < mPlaneLockoutTimer.GetElapsedMs())
			{
				SpawnPlane();
			}

			foreach (WW7Plane plane in mPlanes)
			{
				plane.Update(gameTime);
			}

			mPlanes.RemoveAll(p => p.Finished());
		}

		void SpawnPlane()
		{
			MonoRandom drawRandom = RandomManager.I.GetDraw();
			float y = drawRandom.GetFloatRange(30.0f, 190.0f);

			bool goingLeft = drawRandom.PercentChance(50.0f);


			Vector2 planeVel = new Vector2(drawRandom.GetFloatRange(4.0f, 6.0f), 0.0f);
			Vector2 planePos = new Vector2(-20.0f, y);

			int animType = drawRandom.GetIntRange(0, mPlaneAnimators.Length - 1);
			switch (animType)
			{
				case 0: // Plane
					planeVel *= 1.8f;
					break;
				case 1: // Heli
					planeVel *= 0.8f;
					break;
			}
			AnimationData animData = mPlaneAnimators[animType];

			if (goingLeft)
			{
				planeVel.X = -planeVel.X;
				planePos.X = 544.0f;
			}

			foreach (WW7Plane plane in mPlanes)
			{
				if (MathF.Abs(plane.mPos.Y - y) < 10.0f)
				{
					// Too close. Try again next frame.
					return;
				}
			}

			WW7Plane newPlane = new WW7Plane(planePos, planeVel, animData.GenerateAnimator());
			mPlanes.Add(newPlane);

			ResetLockoutTimer();
		}

		public override void Draw(DrawInfo info)
		{
			foreach (WW7Plane plane in mPlanes)
			{
				plane.Draw(info);
			}
		}
	}

	/// <summary>
	/// Represents a plane
	/// </summary>
	class WW7Plane : FX
	{
		Animator mAnimator;
		public Vector2 mPos;
		public Vector2 mVelocity;

		public WW7Plane(Vector2 pos, Vector2 vel, Animator anim)
		{
			mPos = pos;
			mVelocity = vel;
			mAnimator = anim;
			mAnimator.Play();
		}

		public override void Draw(DrawInfo info)
		{
			SpriteEffects effect = SpriteEffects.None;
			if (mVelocity.X < 0.0f)
			{
				effect = SpriteEffects.FlipHorizontally;
			}
			MonoDraw.DrawTexture(info, mAnimator.GetCurrentTexture(), mPos, null, Color.White, 0.0f, Vector2.Zero, 1.0f, effect, DrawLayer.Background);
		}

		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);
			mPos += mVelocity * dt;
			mAnimator.Update(gameTime);
		}

		public override bool Finished()
		{
			if (mVelocity.X < 0.0f)
			{
				return mPos.X < -mAnimator.GetTexture(0).Width;
			}

			return mPos.X > FXManager.I.GetDrawableSize().X + mAnimator.GetTexture(0).Width;
		}
	}
}
