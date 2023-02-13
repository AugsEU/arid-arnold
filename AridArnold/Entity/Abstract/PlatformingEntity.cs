namespace AridArnold
{
	/// <summary>
	/// Entity that can run and jump.
	/// </summary>
	abstract class PlatformingEntity : MovingEntity
	{
		#region rConstants

		const float DEFAULT_WALK_SPEED = 9.0f;
		const float DEFAULT_GRAVITY = 4.35f;
		const float DEFAULT_JUMP_SPEED = 25.0f;
		const float MAX_VELOCITY = 65.0f;

		#endregion rConstants





		#region rMembers

		protected bool mOnGround;

		//Magniture of motion vectors
		protected float mWalkSpeed;
		protected float mJumpSpeed;
		protected float mGravity;

		int mUpdatesSinceGrounded;

		private CardinalDirection mGravityDirection;
		protected WalkDirection mWalkDirection;
		protected WalkDirection mPrevDirection;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Platforming entity constructor
		/// </summary>
		/// <param name="pos">Starting position.</param>
		/// <param name="walkSpeed">Ground walk speed.</param>
		/// <param name="jumpSpeed">Initial upwards velocity when jumping.</param>
		/// <param name="gravity">Gravity acceleration.</param>
		public PlatformingEntity(Vector2 pos, float walkSpeed = DEFAULT_WALK_SPEED, float jumpSpeed = DEFAULT_JUMP_SPEED, float gravity = DEFAULT_GRAVITY) : base(pos)
		{
			mVelocity = Vector2.Zero;
			mWalkDirection = WalkDirection.None;
			mPrevDirection = mWalkDirection;

			mWalkSpeed = walkSpeed;
			mJumpSpeed = jumpSpeed;
			mGravity = gravity;

			mGravityDirection = CardinalDirection.Down;

			mUpdatesSinceGrounded = int.MaxValue;
		}

		#endregion





		#region rUpdate

		/// <summary>
		/// Update platforming entity
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public override void Update(GameTime gameTime)
		{
			Vector2 downVec = GravityVecNorm();
			Vector2 sideVec = new Vector2(MathF.Abs(downVec.Y), MathF.Abs(downVec.X));

			float component = Vector2.Dot(mVelocity, sideVec);

			mVelocity = mVelocity - component * sideVec;

			switch (mWalkDirection)
			{
				case WalkDirection.Left:
					mVelocity -= mWalkSpeed * sideVec;
					break;
				case WalkDirection.Right:
					mVelocity += mWalkSpeed * sideVec;
					break;
				case WalkDirection.None:
					if (mOnGround == false)
					{
						mVelocity += component * sideVec;
					}
					break;
			}

			if (mOnGround)
			{
				mUpdatesSinceGrounded = 0;
			}
			else if(mUpdatesSinceGrounded != int.MaxValue)
			{
				mUpdatesSinceGrounded++;
			}

			ApplyGravity(gameTime);
			mOnGround = false;

			mVelocity = new Vector2(MonoMath.ClampAbs(mVelocity.X, MAX_VELOCITY), MonoMath.ClampAbs(mVelocity.Y, MAX_VELOCITY));

			base.Update(gameTime);
		}



		/// <summary>
		/// React to collision with a block.
		/// </summary>
		/// <param name="normal">Normal vector of collision.</param>
		public override void ReactToCollision(Vector2 normal)
		{
			CollisionType collisionType;

			float dp = Vector2.Dot(normal, GravityVecNorm());

			if (dp >= 0.95f)
			{
				collisionType = CollisionType.Ceiling;
			}
			else if (dp <= -0.95f)
			{
				collisionType = CollisionType.Ground;
			}
			else
			{
				collisionType = CollisionType.Wall;
			}

			switch (collisionType)
			{
				case CollisionType.Ground:
					mOnGround = true;
					break;
				case CollisionType.Ceiling:
					if (mOnGround == false)
					{
						mVelocity = GravityVecNorm() * mGravity;
					}
					break;
			}
		}



		/// <summary>
		/// Move entity under the force of gravity
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		private void ApplyGravity(GameTime gameTime)
		{
			float vecAlongGrav = Vector2.Dot(GravityVecNorm(), mVelocity);

			float mod = 1.0f;
			if (vecAlongGrav < 0.0f)
			{
				mod = 2.0f;
			}

			float delta = mGravity * Util.GetDeltaT(gameTime) * mod;
			Vector2 deltaVec = GravityVecNorm() * delta;

			if (-delta < vecAlongGrav && vecAlongGrav < 0.0f)
			{
				deltaVec = deltaVec / 3.5f;
			}

			mVelocity += deltaVec;
		}



		/// <summary>
		/// Get the update ordering. Higher value means update will happen first.
		/// </summary>
		protected override void CalculateUpdateOrder()
		{
			// If one entity is standing on another, and they both jump on the same frame,
			// we want the one who is standing on top to 
			Vector2 gravityVec = GravityVecNorm();
			Vector2 fallingVec = Vector2.Dot(gravityVec, mVelocity) * gravityVec;
			fallingVec.Normalize();

			mUpdateOrder = Vector2.Dot(mPosition, fallingVec);
			mUpdateOrder = MonoMath.SquashToRange(mUpdateOrder, EntityManager.UPDATE_MENTITY_MIN, EntityManager.UPDATE_MENTITY_MAX);
		}

		#endregion rUpdate



		#region rDraw

		protected void DrawPlatformer(DrawInfo info, Texture2D texture, Color color, float layer)
		{
			MonoDraw.DrawPlatformer(info, ColliderBounds(), texture, mPosition, color, GetGravityDir(), mPrevDirection, MonoDraw.LAYER_DEFAULT);
		}

		#endregion rDraw



		#region rUtility

		/// <summary>
		/// Set gravity direction
		/// </summary>
		/// <param name="dir">New gravity direction</param>
		public void SetGravity(CardinalDirection dir)
		{
			mGravityDirection = dir;
		}

		/// <summary>
		/// Get current gravity direction
		/// </summary>
		/// <returns>Current gravity direction</returns>
		public CardinalDirection GetGravityDir()
		{
			return mGravityDirection;
		}



		/// <summary>
		/// Get gravity normal.
		/// </summary>
		/// <returns>Get vector along gravity direction</returns>
		/// <exception cref="NotImplementedException">Must be a cardinal direction</exception>
		public Vector2 GravityVecNorm()
		{
			switch (GetGravityDir())
			{
				case CardinalDirection.Down:
					return new Vector2(0.0f, 1.0f);
				case CardinalDirection.Up:
					return new Vector2(0.0f, -1.0f);
				case CardinalDirection.Left:
					return new Vector2(-1.0f, 0.0f);
				case CardinalDirection.Right:
					return new Vector2(1.0f, 0.0f);
			}

			throw new NotImplementedException();
		}



		/// <summary>
		/// Set walk direction
		/// </summary>
		/// <param name="dir">New walk direction</param>
		public void SetWalkDirection(WalkDirection dir)
		{
			mWalkDirection = dir;
		}



		/// <summary>
		/// Set the "previous" walk direction
		/// </summary>
		/// <param name="dir">New walk direction</param>
		public void SetPrevWalkDirection(WalkDirection dir)
		{
			mPrevDirection = dir;
		}



		/// <summary>
		/// Change walk direction to be opposite.
		/// </summary>
		public void ReverseWalkDirection()
		{
			switch (mWalkDirection)
			{
				case WalkDirection.Left:
					mWalkDirection = WalkDirection.Right;
					break;
				case WalkDirection.Right:
					mWalkDirection = WalkDirection.Left;
					break;
				case WalkDirection.None:
				default:
					break;
			}
		}



		/// <summary>
		/// Make entity jump.
		/// </summary>
		protected void Jump()
		{
			float currentYVelocity = -Vector2.Dot(GravityVecNorm(), mVelocity);

			if (currentYVelocity < mJumpSpeed)
			{
				mVelocity = -mJumpSpeed * GravityVecNorm();
			}

			mUpdatesSinceGrounded = int.MaxValue;
		}



		/// <summary>
		/// Have we been grounded in the last x frames?
		/// </summary>
		public bool IsGroundedSince(int frames)
		{
			return mUpdatesSinceGrounded < frames;
		}



		/// <summary>
		/// Grounded property. 
		/// </summary>
		public bool pGrounded
		{
			get => mOnGround;
			set => mOnGround = value;
		}

		#endregion rUtility
	}
}
