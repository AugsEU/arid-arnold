namespace AridArnold
{
	/// <summary>
	/// Objects can submit auxillary hitboxes to the entity manager for review.
	/// </summary>
	abstract class ColliderSubmission
	{
		/// <summary>
		/// Can we collide with this entity?
		/// </summary>
		public abstract bool CanCollideWith(MovingEntity entity);



		/// <summary>
		/// Do collision check and get entity collision result. Can return null
		/// </summary>
		public abstract EntityCollision GetEntityCollision(GameTime gameTime, MovingEntity entity);
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
		public override EntityCollision GetEntityCollision(GameTime gameTime, MovingEntity entity)
		{
			CollisionResults results = CollideWith(gameTime, entity);

			//Collision!
			if(results.t.HasValue)
			{
				return new EntityEntityCollision(results, mEntity);
			}

			//No collision
			return null;
		}

		#endregion rCollision
	}



	class PlatformColliderSubmission : ColliderSubmission
	{
		#region rMembers

		Vector2 mVelocity;
		Vector2 mPosition;
		float mWidth;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Init EntityColliderSubmission with a top left coord and the width of the platform.
		/// </summary>
		public PlatformColliderSubmission(Vector2 velocity, Vector2 leftPos, float width)
		{
			mVelocity = velocity;
			mPosition = leftPos;
			mWidth = width;
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
			return Collision2D.MovingRectVsPlatform(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), mPosition, mWidth, CardinalDirection.Up);
		}

		public override EntityCollision GetEntityCollision(GameTime gameTime, MovingEntity entity)
		{
			CollisionResults results = CollideWith(gameTime, entity);

			//Collision!
			if (results.t.HasValue)
			{
				return new MovingPlatformCollision(results, mVelocity);
			}

			//No collision
			return null;
		}

		#endregion rCollision
	}
}
