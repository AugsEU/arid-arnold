namespace AridArnold
{
	/// <summary>
	/// Represents collision between a moving platform and an entity.
	/// </summary>
	class MovingPlatformEntityCollision : SolidEntityCollision
	{
		Vector2 mAddedVel;

		public MovingPlatformEntityCollision(bool firstTime, CollisionResults result, Vector2 velocity) : base(firstTime, result)
		{
			mAddedVel = velocity;
		}

		protected override Vector2 GetExtraVelocityInternal(MovingEntity entity)
		{
			if (entity is PlatformingEntity)
			{
				PlatformingEntity platformingEntity = (PlatformingEntity)entity;
				Vector2 gravity = platformingEntity.GravityVecNorm();

				if (Vector2.Dot(gravity, mResult.normal) < -0.001f)
				{
					return mAddedVel;
				}
			}

			return base.GetExtraVelocityInternal(entity);
		}
	}
}
