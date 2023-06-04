namespace AridArnold
{
	/// <summary>
	/// Entity that can move and collide.
	/// </summary>
	abstract class MovingEntity : Entity
	{
		#region rConstants

		//Maximum number of collisions before we abort and assume we are stuck in an infinite loop.
		const int COLLISION_MAX_COUNT = 1024;

		#endregion rConstants





		#region rMembers

		protected Vector2 mPrevVelocity;
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
		/// Update Moving Entity.
		/// </summary>
		/// <param name="gameTime">Frame time.</param>
		public override void OrderedUpdate(GameTime gameTime)
		{
			mPrevVelocity = mVelocity;
			UpdateCollision(gameTime);

			mFallthrough = false;

			base.OrderedUpdate(gameTime);
		}


		/// <summary>
		/// Handle all collisions and physics.
		/// </summary>
		/// <param name="gameTime"></param>
		private void UpdateCollision(GameTime gameTime)
		{
			// List of all collisions that actually happened. A collision can be detected but
			// never actually happen. E.g. if we were going to collide with a wall, but the ground
			// is in the way.
			List<EntityCollision> collisionList = new List<EntityCollision>();

			EntityCollision currentCollision = EntityManager.I.GetNextCollision(gameTime, this);
			PlatformingEntity platformingEntity = (PlatformingEntity)this;

			while (currentCollision != null)
			{
				EntityCollision entityCollision = currentCollision;
				CollisionResults collisionResults = entityCollision.GetResult();

				Vector2 pushVec = collisionResults.normal * new Vector2(Math.Abs(mVelocity.X), Math.Abs(mVelocity.Y)) * (1.0f - collisionResults.t.Value) * 1.012f;
				mVelocity += pushVec;

				mVelocity += entityCollision.GetExtraVelocity(this);

				collisionList.Add(entityCollision);

				if (collisionList.Count > COLLISION_MAX_COUNT)
				{
					// Fail-safe, don't move
					mVelocity = Vector2.Zero;

					//Clear list of bogus
					collisionList.Clear();
					break;
				}

				currentCollision = EntityManager.I.GetNextCollision(gameTime, this);
			}

			ApplyVelocity(gameTime);

			foreach (EntityCollision entityCollision in collisionList)
			{
				entityCollision.PostCollisionReact(this);
			}
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
		/// Get velocity from previous frame
		/// </summary>
		public Vector2 GetPrevVelocity()
		{
			return mPrevVelocity;
		}


		
		/// <summary>
		/// Get current velocity
		/// </summary>
		public Vector2 GetVelocity()
		{
			return mVelocity;
		}



		/// <summary>
		/// Set velocity.
		/// </summary>
		public void SetVelocity(Vector2 vel)
		{
			mVelocity = vel;
		}



		/// <summary>
		/// Kind of like setting the velocity, but we override all other related to the velocity.
		/// </summary>
		public virtual void OverrideVelocity(Vector2 vel)
		{
			mVelocity = vel;
		}

		#endregion rUtility

	}
}
