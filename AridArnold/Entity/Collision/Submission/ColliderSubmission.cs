namespace AridArnold
{
	/// <summary>
	/// Objects can submit auxillary hitboxes to the entity manager for review.
	/// These objects decide if there is a collision then generate an entity collision to handle it.
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

			if (entityCollision != null)
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
}
