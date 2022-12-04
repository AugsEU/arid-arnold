namespace AridArnold
{
	/// <summary>
	/// A direction we can walk in.
	/// </summary>
	enum WalkDirection
	{
		Left,
		Right,
		None,
	}



	/// <summary>
	/// Comparison class to sort Entity for the OrderedUpdate
	/// </summary>
	class EntityUpdateSorter : IComparer<Entity>
	{
		public int Compare(Entity a, Entity b)
		{
			return b.GetUpdateOrder().CompareTo(a.GetUpdateOrder());
		}
	}



	/// <summary>
	/// Represents a moving entity in the game world.
	/// </summary>
	abstract class Entity
	{
		#region rMembers

		protected Vector2 mPosition;
		protected Vector2 mCentreOfMass;
		protected Texture2D mTexture;
		protected float mUpdateOrder;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Entity constructor
		/// </summary>
		/// <param name="pos">Starting position.</param>
		public Entity(Vector2 pos)
		{
			mPosition = pos;
			mCentreOfMass = pos;
			mUpdateOrder = 0.0f;
		}



		/// <summary>
		/// LoadContent for entity such as textures
		/// </summary>
		/// <param name="content">Monogame Content Manager</param>
		public abstract void LoadContent(ContentManager content);

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update entity. No guarantees on order.
		/// </summary>
		/// <param name="gameTime">Frame time.</param>
		public virtual void Update(GameTime gameTime)
		{
			TileManager.I.EntityTouchTiles(this);

			mCentreOfMass = ColliderBounds().Centre;

			//Calculate order for OrderedUpdate
			CalculateUpdateOrder();
		}


		/// <summary>
		/// Update entity with entity update order done by mUpdateOrder.
		/// </summary>
		public virtual void OrderedUpdate(GameTime gameTime)
		{

		}



		/// <summary>
		/// React to a collision with this entity.
		/// </summary>
		/// <param name="entity"></param>
		public virtual void CollideWithEntity(Entity entity)
		{
			//Default: Do nothing.
		}



		/// <summary>
		/// Get collider submission for this frame. Entities that want to collide will have to submit one.
		/// </summary>
		public virtual ColliderSubmission GetColliderSubmission()
		{
			return null;
		}



		/// <summary>
		/// Calculate update order. This is supposed to be storred in mUpdateOrder
		/// </summary>
		protected virtual void CalculateUpdateOrder()
		{
		}


		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw entity
		/// </summary>
		/// <param name="info">Info needed for drawing.</param>
		public abstract void Draw(DrawInfo info);

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Get the collider for this entity
		/// </summary>
		/// <returns></returns>
		public abstract Rect2f ColliderBounds();



		/// <summary>
		/// Kill this entity.
		/// </summary>
		public virtual void Kill()
		{

		}



		/// <summary>
		/// Move position by relative amount.
		/// </summary>
		/// <param name="shift">Relative change in position.</param>
		public void ShiftPosition(Vector2 shift)
		{
			mPosition += shift;
		}



		/// <summary>
		/// Position property.
		/// </summary>
		public Vector2 pPosition
		{
			get { return mPosition; }
			set { mPosition = value; }
		}



		/// <summary>
		/// Get the centre of this entity
		/// </summary>
		public Vector2 GetCentrePos()
		{
			Rect2f collider = ColliderBounds();

			return (collider.min + collider.max) / 2.0f;
		}



		/// <summary>
		/// Get the update ordering. Higher value means update will happen first.
		/// </summary>
		public float GetUpdateOrder()
		{
			return mUpdateOrder;
		}

		#endregion rUtility
	}





	/// <summary>
	/// Entity that can move and collide.
	/// </summary>
	abstract class MovingEntity : Entity
	{
		#region rMembers

		protected Vector2 mVelocity;
		protected bool mFallthrough;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Create moving entity
		/// </summary>
		/// <param name="pos">Starting position</param>
		public MovingEntity(Vector2 pos) : base(pos)
		{
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update Moving Entity. No guarantees on order.
		/// </summary>
		/// <param name="gameTime">Frame time.</param>
		public override void OrderedUpdate(GameTime gameTime)
		{
			List<Vector2> collidedNormals = EntityManager.I.UpdateCollisionEntity(gameTime, this);

			ApplyVelocity(gameTime);

			mFallthrough = false;

			foreach(Vector2 v in collidedNormals)
			{
				ReactToCollision(v);
			}

			base.OrderedUpdate(gameTime);
		}



		/// <summary>
		/// Move position by velocity
		/// </summary>
		/// <param name="gameTime">Frame time.</param>
		protected void ApplyVelocity(GameTime gameTime)
		{
			mPosition += VelocityToDisplacement(gameTime);
		}



		/// <summary>
		/// Resolve collision with specific normal.
		/// </summary>
		/// <param name="collisionNormal"></param>
		public abstract void ReactToCollision(Vector2 collisionNormal);

		#endregion rUpdate





		#region rUtility

		/// <summary>
		/// Should we collide with platform tiles?
		/// </summary>
		/// <returns></returns>
		public bool CollideWithPlatforms()
		{
			return !mFallthrough;
		}



		/// <summary>
		/// Convert velocity into actual position displacement.
		/// </summary>
		/// <param name="gameTime">Frame time.</param>
		/// <returns></returns>
		public Vector2 VelocityToDisplacement(GameTime gameTime)
		{
			return mVelocity * Util.GetDeltaT(gameTime);
		}



		/// <summary>
		/// Make entity fall through platforms.
		/// </summary>
		protected void FallThroughPlatforms()
		{
			mFallthrough = true;
		}



		/// <summary>
		/// Velocity Property.
		/// </summary>
		public Vector2 pVelocity
		{
			get { return mVelocity; }
			set { mVelocity = value; }
		}

		#endregion rUtility

	}

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

		private CardinalDirection mCardinalDirection;
		protected WalkDirection mWalkDirection;

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

			mWalkSpeed = walkSpeed;
			mJumpSpeed = jumpSpeed;
			mGravity = gravity;

			mCardinalDirection = CardinalDirection.Down;
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

			ApplyGravity(gameTime);
			mOnGround = false;

			mVelocity = new Vector2(Util.ClampAbs(mVelocity.X, MAX_VELOCITY), Util.ClampAbs(mVelocity.Y, MAX_VELOCITY));

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
					mVelocity = GravityVecNorm() * mGravity;
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
		}

		#endregion rUpdate





		#region rUtility

		/// <summary>
		/// Set gravity direction
		/// </summary>
		/// <param name="dir">New gravity direction</param>
		public void SetGravity(CardinalDirection dir)
		{
			mCardinalDirection = dir;
		}

		/// <summary>
		/// Get current gravity direction
		/// </summary>
		/// <returns>Current gravity direction</returns>
		public CardinalDirection GetGravityDir()
		{
			return mCardinalDirection;
		}



		/// <summary>
		/// Get gravity normal.
		/// </summary>
		/// <returns>Get vector along gravity direction</returns>
		/// <exception cref="NotImplementedException">Must be a cardinal direction</exception>
		protected Vector2 GravityVecNorm()
		{
			switch (GetGravityDir())
			{
				case AridArnold.CardinalDirection.Down:
					return new Vector2(0.0f, 1.0f);
				case AridArnold.CardinalDirection.Up:
					return new Vector2(0.0f, -1.0f);
				case AridArnold.CardinalDirection.Left:
					return new Vector2(-1.0f, 0.0f);
				case AridArnold.CardinalDirection.Right:
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
