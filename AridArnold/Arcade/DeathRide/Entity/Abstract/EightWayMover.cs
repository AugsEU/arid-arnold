namespace GMTK2023
{
	internal abstract class EightWayMover : Entity
	{

		#region rMembers

		protected Animator[] mDirectionTextures;

		EightDirection mCurrentDirection;
		float mCurrentAngle;
		float mTargetAngle;
		protected float mSpeed;
		float mTurnSpeed;

		#endregion rMembers





		#region rInit

		public EightWayMover(Vector2 pos, float angle, float turnSpeed) : base(pos)
		{
			mCurrentAngle = angle;
			mSpeed = 0.0f;
			mTargetAngle = angle;
			mTurnSpeed = turnSpeed;
			mCurrentDirection = Util.GetDirectionFromAngle(mCurrentAngle);
		}

		#endregion rInit



		#region rUpdate

		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);

			// Move to target angle
			float angleDelta = mTurnSpeed * dt;
			float angleDiff = MonoMath.GetAngleDiff(mTargetAngle, mCurrentAngle);

			if (MathF.Abs(angleDiff) < MathF.Abs(angleDelta))
			{
				mCurrentAngle = mTargetAngle;
			}
			else
			{
				mCurrentAngle += MathF.Sign(angleDiff) * angleDelta;
			}

			mCurrentDirection = Util.GetDirectionFromAngle(mCurrentAngle);
			mPosition += GetVelocity() * dt;

			ForceInBounds(GameScreen.PLAYABLE_AREA);

			foreach (Animator animator in mDirectionTextures)
			{
				animator.Update(gameTime);
			}

			base.Update(gameTime);
		}





		protected void TargetDirection(EightDirection direction)
		{
			float angle = Util.GetAngleFromDirection(direction);
			mTargetAngle = angle;
		}

		protected void TargetAngle(float angle)
		{
			mTargetAngle = angle;
		}

		protected void ForceAngle(float angle)
		{
			mCurrentAngle = angle;
			mTargetAngle = angle;
		}

		protected EightDirection GetCurrentDir()
		{
			return mCurrentDirection;
		}

		public Vector2 GetVelocity()
		{
			return MonoMath.GetVectorFromAngle(-mCurrentAngle) * mSpeed;
		}

		public float GetCurrentAngle()
		{
			return mCurrentAngle;
		}

		#endregion rUpdate


		#region rDraw

		public override void Draw(DrawInfo info)
		{
			int animIndex = (int)mCurrentDirection;
			MonoDraw.DrawTextureDepth(info, mDirectionTextures[animIndex].GetCurrentTexture(), mPosition, DrawLayer.Player);
		}

		#endregion rDraw


	}
}
