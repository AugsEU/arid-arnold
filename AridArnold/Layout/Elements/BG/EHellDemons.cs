
// Bespoke classes for the hell background. Not done "properly" but weirdness is contained so it's fine.
namespace AridArnold
{
	internal class EHellDemons : LayElement
	{
		const float LOCK_OUT_MIN = 2000.0f;
		const float LOCK_OUT_MAX = 3000.0f;

		List<HellDemon> mDemons;
		MonoTimer mDemonLockoutTimer;
		float mCurrentLockout = LOCK_OUT_MIN;
		AnimationData[] mDemonAnimations;

		public EHellDemons(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			mDemons = new List<HellDemon>();
			mDemonLockoutTimer = new MonoTimer();
			ResetLockoutTimer();

			mDemonAnimations = new AnimationData[] {
				MonoData.I.LoadAnimatorData("BG/Hell/DemonSmall.max"),
				MonoData.I.LoadAnimatorData("BG/Hell/DemonMedium.max")
			};

			SpawnDemon();
			SpawnDemon();
			SpawnDemon();
			SpawnDemon();

			foreach (HellDemon demon in mDemons)
			{
				demon.mPos.X = RandomManager.I.GetDraw().GetFloatRange(50.0f, 500.0f);
			}
		}

		void ResetLockoutTimer()
		{
			mCurrentLockout = RandomManager.I.GetDraw().GetFloatRange(LOCK_OUT_MIN, LOCK_OUT_MAX);
			mDemonLockoutTimer.FullReset();
			mDemonLockoutTimer.Start();
		}

		public override void Update(GameTime gameTime)
		{
			mDemonLockoutTimer.Update(gameTime);

			if (mCurrentLockout < mDemonLockoutTimer.GetElapsedMs())
			{
				SpawnDemon();
			}

			foreach (HellDemon demon in mDemons)
			{
				demon.Update(gameTime);
			}

			mDemons.RemoveAll(p => p.Finished());
		}

		void SpawnDemon()
		{
			Point FXSize = FXManager.I.GetDrawableSize();
			Vector2 centreFX = new Vector2(FXSize.X, FXSize.Y) * 0.5f;

			float downAngle = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera).GetCurrentSpec().mRotation;
			Vector2 downVector = MonoMath.Rotate(new Vector2(0.0f, 1.0f), -downAngle);
			Vector2 sideVector = MonoMath.Perpendicular(downVector);

			MonoRandom drawRandom = RandomManager.I.GetDraw();
			bool goingLeft = drawRandom.PercentChance(50.0f);

			Vector2 demonPos = centreFX;
			demonPos += downVector * drawRandom.GetFloatRange(-250.0f, 250.0f);
			demonPos += sideVector * (goingLeft ? 400.0f : -400.0f);

			Vector2 demonVel = new Vector2((goingLeft ? -1.0f : 1.0f) * drawRandom.GetFloatRange(4.0f, 6.0f), 0.0f);

			// Verify position
			foreach (HellDemon demon in mDemons)
			{
				float effectiveOtherY = Vector2.Dot(demon.mPos, downVector);
				float effectiveOurY = Vector2.Dot(demonPos, downVector);
				if (MathF.Abs(effectiveOtherY - effectiveOurY) < 40.0f)
				{
					// Too close. Try again next frame.
					return;
				}
			}

			int animType = drawRandom.GetIntRange(0, mDemonAnimations.Length - 2);
			if (drawRandom.PercentChance(2))
			{
				animType += 1;
			}
			AnimationData animData = mDemonAnimations[animType];
			float hoverSpeed = drawRandom.GetFloatRange(0.02f, 0.1f);
			float hoverAmp = drawRandom.GetFloatRange(10.0f, 26.0f);

			HellDemon newDemon = new HellDemon(demonPos, demonVel, animData.GenerateAnimator(), hoverSpeed, hoverAmp);
			mDemons.Add(newDemon);

			ResetLockoutTimer();
		}

		public override void Draw(DrawInfo info)
		{
			foreach (HellDemon demon in mDemons)
			{
				demon.Draw(info);
			}
		}
	}

	/// <summary>
	/// Represents a demon
	/// </summary>
	class HellDemon : FX
	{
		Animator mAnimator;
		public Vector2 mPos;
		Vector2 mStartPos;
		Vector2 mVelocity;
		float mHoverAngle;
		float mHoverSpeed;
		float mHoverAmplitude;


		public HellDemon(Vector2 pos, Vector2 vel, Animator anim, float hoverSpeed, float hoverAmp)
		{
			mPos = pos;
			mStartPos = pos;
			mVelocity = vel;
			mAnimator = anim;
			mAnimator.Play();

			mHoverAngle = RandomManager.I.GetDraw().GetFloatRange(0.0f, 6.28f);
			mHoverSpeed = hoverSpeed;
			mHoverAmplitude = hoverAmp;
		}

		public override void Draw(DrawInfo info)
		{
			SpriteEffects effect = SpriteEffects.None;
			if (mVelocity.X < 0.0f)
			{
				effect = SpriteEffects.FlipHorizontally;
			}
			float downAngle = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera).GetCurrentSpec().mRotation;
			Vector2 effectivePos = mPos;
			effectivePos += MonoMath.Rotate(new Vector2(0.0f, mHoverAmplitude * MonoMath.UnitWave(mHoverAngle)), -downAngle);

			MonoDraw.DrawTexture(info, mAnimator.GetCurrentTexture(), effectivePos, null, Color.White, -downAngle, Vector2.Zero, 1.0f, effect, DrawLayer.Background);
		}

		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);

			float downAngle = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera).GetCurrentSpec().mRotation;
			Vector2 effectiveVel = MonoMath.Rotate(mVelocity, -downAngle);

			mPos += effectiveVel * dt;
			mHoverAngle += mHoverSpeed * dt;

			mAnimator.Update(gameTime);
		}

		public override bool Finished()
		{
			return (mStartPos - mPos).Length() > 1500.0f;
		}
	}
}
