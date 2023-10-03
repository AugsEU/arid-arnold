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
		const float DEFAULT_ICE_GRIP = 0.9f;
		const float MAX_VELOCITY = 65.0f;

		protected const float NOT_MOVING_SPEED = 1.0f;


		#endregion rConstants





		#region rMembers

		protected bool mOnGround;

		//Magniture of motion vectors
		protected float mWalkSpeed;
		protected float mJumpSpeed;
		protected float mGravity;
		protected float mIceGrip;

		int mUpdatesSinceGrounded;

		private CardinalDirection mGravityDirection;
		protected WalkDirection mWalkDirection;
		protected WalkDirection mPrevDirection;

		// Ice
		bool mIceWalking;
		Vector2 mIceVelocity;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Platforming entity constructor
		/// </summary>
		/// <param name="pos">Starting position.</param>
		/// <param name="walkSpeed">Ground walk speed.</param>
		/// <param name="jumpSpeed">Initial upwards velocity when jumping.</param>
		/// <param name="gravity">Gravity acceleration.</param>
		public PlatformingEntity(Vector2 pos, float walkSpeed = DEFAULT_WALK_SPEED, float jumpSpeed = DEFAULT_JUMP_SPEED, float gravity = DEFAULT_GRAVITY, float iceGrip = DEFAULT_ICE_GRIP) : base(pos)
		{
			mVelocity = Vector2.Zero;
			mWalkDirection = WalkDirection.None;
			mPrevDirection = mWalkDirection;
			mIceGrip = iceGrip;

			mWalkSpeed = walkSpeed;
			mJumpSpeed = jumpSpeed;
			mGravity = gravity;

			mGravityDirection = CardinalDirection.Down;

			mUpdatesSinceGrounded = int.MaxValue;

			// Ice
			mIceWalking = false;
			mIceVelocity = Vector2.Zero;
		}

		#endregion





		#region rUpdate

		/// <summary>
		/// Update platforming entity
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);

			SetSideVelocityFromDirection(mWalkDirection);

			// Ice
			if (mIceWalking)
			{
				Vector2 toVelocity = mVelocity - mIceVelocity;
				if (toVelocity.LengthSquared() > float.Epsilon)
				{
					toVelocity.Normalize();
					mIceVelocity += toVelocity * mIceGrip * dt;

					switch (mGravityDirection)
					{
						case CardinalDirection.Up:
						case CardinalDirection.Down:
							mVelocity.X = mIceVelocity.X;
							break;
						case CardinalDirection.Left:
						case CardinalDirection.Right:
							mVelocity.Y = mIceVelocity.Y;
							break;
					}
				}
			}
			else
			{
				GetAGrip();
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

			mIceWalking = false;

			base.Update(gameTime);
		}


		/// <summary>
		/// Set side velocity from a direction
		/// </summary>
		private void SetSideVelocityFromDirection(WalkDirection dir)
		{
			Vector2 downVec = GravityVecNorm();
			Vector2 sideVec = new Vector2(MathF.Abs(downVec.Y), MathF.Abs(downVec.X));

			float component = Vector2.Dot(mVelocity, sideVec);

			mVelocity = mVelocity - component * sideVec;

			switch (dir)
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

			ReactToCollision(collisionType);
		}



		/// <summary>
		/// React to collision with walls/ceilings/floors
		/// </summary>
		protected virtual void ReactToCollision(CollisionType collisionType)
		{
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
				case CollisionType.Wall:
					GetAGrip();
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



		/// <summary>
		/// Inform this entity it is on ice.
		/// </summary>
		public void SetIceWalking()
		{
			mIceWalking = true;
		}



		/// <summary>
		/// Are we on ice?
		/// </summary>
		public bool GetIceWalking()
		{
			return mIceWalking;
		}



		/// <summary>
		/// Grip ice instantly
		/// </summary>
		protected void GetAGrip()
		{
			if (this is PlantPot)
			{
				MonoDebug.DLog("Plant pot grip");
			}
			mIceVelocity = mVelocity;
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the platformer.
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		public override void Draw(DrawInfo info)
		{
			Texture2D textureToDraw = GetDrawTexture();
			Rect2f originalTextureRect = ColliderBounds();
			Color colorToDraw = GetDrawColor();
			CardinalDirection gravityDir = GetGravityDir();
			DrawLayer drawLayer = GetDrawLayer();

			MonoDraw.DrawPlatformer(info, originalTextureRect, textureToDraw, colorToDraw, gravityDir, mPrevDirection, drawLayer);
		}



		/// <summary>
		/// Get texture to draw for this platformer.
		/// </summary>
		protected virtual Texture2D GetDrawTexture()
		{
			return mTexture;
		}



		/// <summary>
		/// Get color to apply when drawing.
		/// </summary>
		protected virtual Color GetDrawColor()
		{
			return Color.White;
		}



		/// <summary>
		/// Get the layer that this platformer should be drawn on.
		/// </summary>
		protected virtual DrawLayer GetDrawLayer()
		{
			return DrawLayer.Default;
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
		public WalkDirection GetWalkDirection()
		{
			return mWalkDirection;
		}



		/// <summary>
		/// Get velocity relative to "down"
		/// </summary>
		public Vector2 GetVelocityRelativeToGravity()
		{
			float currentYVelocity = -Vector2.Dot(GravityVecNorm(), mVelocity);
			float currentXVelocity = -Vector2.Dot(MonoMath.Perpendicular(GravityVecNorm()), mVelocity);

			return new Vector2(currentXVelocity, currentYVelocity);
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
		public WalkDirection GetPrevWalkDirection()
		{
			return mPrevDirection;
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
		/// Set previous walk direction from the velocity we have. Prevents jumping backwards with mirrors.
		/// </summary>
		public void SetPrevWalkDirFromVelocity()
		{
			const float AIR_DIR_THRESH = 0.25f;
			switch (GetGravityDir())
			{
				case CardinalDirection.Down:
				case CardinalDirection.Up:
					if (mVelocity.X > AIR_DIR_THRESH)
					{
						mPrevDirection = WalkDirection.Right;
					}
					else if (mVelocity.X < -AIR_DIR_THRESH)
					{
						mPrevDirection = WalkDirection.Left;
					}
					break;
				case CardinalDirection.Right:
				case CardinalDirection.Left:
					if (mVelocity.Y > AIR_DIR_THRESH)
					{
						mPrevDirection = WalkDirection.Right;
					}
					else if (mVelocity.Y < -AIR_DIR_THRESH)
					{
						mPrevDirection = WalkDirection.Left;
					}
					break;
			}
		}



		/// <summary>
		/// Make entity jump.
		/// </summary>
		protected void Jump()
		{
			Vector2 gravVelocity = GetVelocityRelativeToGravity();

			if (mIceWalking)
			{
				if(MathF.Abs(GetVelocityRelativeToGravity().X) >= NOT_MOVING_SPEED)
				{
					mWalkDirection = mPrevDirection;
				}
				mIceWalking = false;
			}

			if (gravVelocity.Y < mJumpSpeed)
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
		/// Get collider based on texture, but reduced.
		/// Width is reduced centre-aligned
		/// Height is reduced from the top
		/// </summary>
		/// <param name="widthReduction">Amount of width to reduce</param>
		/// <param name="heightReduction">Amount of height to reduce</param>
		protected Rect2f GetReducedTextureCollider(float widthReduction, float heightReduction)
		{
			float height = mTexture.Height - heightReduction;
			float width = mTexture.Width - widthReduction;

			Vector2 effectivePosition;
			Vector2 effectiveSize;

			if (GetGravityDir() == CardinalDirection.Left || GetGravityDir() == CardinalDirection.Right)
			{
				effectivePosition = mPosition + new Vector2(widthReduction, heightReduction / 2.0f);
				effectiveSize = new Vector2(height, width);
			}
			else
			{
				effectivePosition = mPosition + new Vector2(widthReduction / 2.0f, heightReduction);
				effectiveSize = new Vector2(width, height);
			}

			return new Rect2f(effectivePosition, effectivePosition + effectiveSize);
		}



		/// <summary>
		/// Grounded property. 
		/// </summary>
		public bool OnGround()
		{
			return mOnGround;
		}



		/// <summary>
		/// Set grounded.
		/// </summary>
		public void SetGrounded(bool value)
		{
			mOnGround = value;
		}



		/// <summary>
		/// Overriding the velocity should
		/// </summary>
		public override void OverrideVelocity(Vector2 vel)
		{
			base.OverrideVelocity(vel);

			SetPrevWalkDirFromVelocity();
			mWalkDirection = WalkDirection.None;
		}



		/// <summary>
		/// Check if we died from going off-screen
		/// </summary>
		/// <returns>True if we should die</returns>
		/// <exception cref="NotImplementedException">Required valid cardinal direction</exception>
		public bool CheckOffScreenDeath()
		{
			switch (GetGravityDir())
			{
				case CardinalDirection.Up:
					return mPosition.Y < -mTexture.Height / 2.0f;
				case CardinalDirection.Right:
					return mPosition.X > TileManager.I.GetDrawWidth() + 2.0f * mTexture.Width;
				case CardinalDirection.Down:
					return mPosition.Y > TileManager.I.GetDrawHeight() + mTexture.Height / 2.0f;
				case CardinalDirection.Left:
					return mPosition.X < -mTexture.Width / 2.0f;
			}

			throw new NotImplementedException();
		}



		/// <summary>
		/// If you were at the position, which way would you need to walk to get to me in my reference frame?
		/// </summary>
		public WalkDirection DirectionNeededToWalkToMe(Vector2 pos)
		{
			Vector2 down = GravityVecNorm();
			Vector2 toPosition = pos - GetCentrePos();

			float cross = down.X * toPosition.Y - down.Y * toPosition.X;

			if(cross > 0.0f)
			{
				return WalkDirection.Right;
			}
			else if(cross < 0.0f)
			{
				return WalkDirection.Left;
			}
			
			return WalkDirection.None;
		}


		/// <summary>
		/// Gets 3 points at our "feet" to check against.
		/// 0 - Left
		/// 1 - Middle
		/// 2 - Right
		/// </summary>
		public Vector2[] GetFeetCheckPoints()
		{
			const float FEET_SHIFT = 2.0f;
			Vector2[] retValue = new Vector2[3];
			Rect2f collider = ColliderBounds();
			Vector2 gravityVec = GravityVecNorm();

			switch (mGravityDirection)
			{
				case CardinalDirection.Up:
					retValue[0] = new Vector2(collider.min.X + FEET_SHIFT, collider.min.Y);
					retValue[2] = new Vector2(collider.max.X - FEET_SHIFT, collider.min.Y);
					break;
				case CardinalDirection.Right:
					retValue[0] = new Vector2(collider.max.X, collider.max.Y - FEET_SHIFT);
					retValue[2] = new Vector2(collider.max.X, collider.min.Y + FEET_SHIFT);
					break;
				case CardinalDirection.Down:
					retValue[0] = new Vector2(collider.min.X + FEET_SHIFT, collider.max.Y);
					retValue[2] = new Vector2(collider.max.X - FEET_SHIFT, collider.max.Y);
					break;
				case CardinalDirection.Left:
					retValue[0] = new Vector2(collider.min.X, collider.min.Y + FEET_SHIFT);
					retValue[2] = new Vector2(collider.min.X, collider.max.Y - FEET_SHIFT);
					break;
				default:
					break;
			}

			retValue[1] = 0.5f * (retValue[0] + retValue[2]);

			for(int i = 0; i < 3; ++i)
			{
				retValue[i] += gravityVec;
			}

			return retValue;
		}

		#endregion rUtility
	}
}
