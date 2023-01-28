namespace AridArnold
{
	/// <summary>
	/// Objects can submit auxillary hitboxes to the entity manager for review.
	/// </summary>
	abstract class ColliderSubmission
	{
		protected HashSet<MovingEntity> mCollidedEntities = new HashSet<MovingEntity>();

		/// <summary>
		/// Can we collide with this entity?
		/// </summary>
		public abstract bool CanCollideWith(MovingEntity entity);



		/// <summary>
		/// Do collision check and get entity collision result. Can return null 
		/// </summary>
		public EntityCollision GetEntityCollision(GameTime gameTime, MovingEntity entity)
		{
			EntityCollision entityCollision = GetEntityCollisionInternal(gameTime, entity);

			if(entityCollision != null)
			{
				mCollidedEntities.Add(entity);
			}

			return entityCollision;
		}



		/// <summary>
		/// Do collision check and get entity collision result. Can return null
		/// </summary>
		protected abstract EntityCollision GetEntityCollisionInternal(GameTime gameTime, MovingEntity entity);
	}



	/// <summary>
	/// An entity submits themselves to be collided with other entities.
	/// </summary>
	class EntityColliderSubmission : ColliderSubmission
	{
		#region rMembers

		MovingEntity mEntity;

		#endregion rMembers



		#region rInitialisation

		/// <summary>
		/// Init EntityColliderSubmission with an entity.
		/// </summary>
		public EntityColliderSubmission(MovingEntity entity)
		{
			mEntity = entity;
		}

		#endregion rInitialisation


		#region rCollision

		/// <summary>
		/// Entity can collide with any
		/// </summary>
		public override bool CanCollideWith(MovingEntity entity)
		{
			return !object.ReferenceEquals(mEntity, entity);
		}



		/// <summary>
		/// Collision check with other entity.
		/// </summary>
		private CollisionResults CollideWith(GameTime gameTime, MovingEntity entity)
		{
			return Collision2D.MovingRectVsRect(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), mEntity.ColliderBounds());
		}

		
		/// <summary>
		/// Do collision with other entity
		/// </summary>
		protected override EntityCollision GetEntityCollisionInternal(GameTime gameTime, MovingEntity entity)
		{
			CollisionResults results = CollideWith(gameTime, entity);

			//Collision!
			if(results.t.HasValue)
			{
				return new EntityEntityCollision(!mCollidedEntities.Contains(entity), results, mEntity);
			}

			//No collision
			return null;
		}

		#endregion rCollision
	}



	/// <summary>
	/// Platform submits itself for collision
	/// </summary>
	class PlatformColliderSubmission : ColliderSubmission
	{
		#region rMembers

		Vector2 mVelocity;
		Vector2 mPosition;
		CardinalDirection mRotation;
		float mWidth;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Init EntityColliderSubmission with a top left coord and the width of the platform.
		/// </summary>
		public PlatformColliderSubmission(Vector2 velocity, Vector2 leftPos, float width, CardinalDirection rotation)
		{
			mVelocity = velocity;
			mPosition = leftPos;
			mWidth = width;
			mRotation = rotation;
		}

		#endregion rInitialisation





		#region rCollision

		/// <summary>
		/// Platforms can collide with all entities.
		/// </summary>
		public override bool CanCollideWith(MovingEntity entity)
		{
			return true;
		}



		/// <summary>
		/// Collision check with other entity.
		/// </summary>
		private CollisionResults CollideWith(GameTime gameTime, MovingEntity entity)
		{
			if (!entity.CollideWithPlatforms())
			{
				if (entity is PlatformingEntity)
				{
					PlatformingEntity platformingEntity = (PlatformingEntity)entity;

					if (mRotation == Util.InvertDirection(platformingEntity.GetGravityDir()))
					{
						return CollisionResults.None;
					}
				}
				else
				{
					return CollisionResults.None;
				}
			}

			return Collision2D.MovingRectVsPlatform(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), mPosition, mWidth, mRotation);
		}

		protected override EntityCollision GetEntityCollisionInternal(GameTime gameTime, MovingEntity entity)
		{
			CollisionResults results = CollideWith(gameTime, entity);

			//Collision!
			if (results.t.HasValue)
			{
				return new MovingPlatformCollision(!mCollidedEntities.Contains(entity), results, mVelocity);
			}

			//No collision
			return null;
		}

		#endregion rCollision
	}
}
