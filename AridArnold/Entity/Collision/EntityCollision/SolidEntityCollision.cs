namespace AridArnold
{
	/// <summary>
	/// Represents collision between an entity and a solid hitbox.
	/// </summary>
	class SolidEntityCollision : EntityCollision
	{
		public SolidEntityCollision(bool firstTime, CollisionResults result) : base(firstTime, result)
		{
		}

		public override void PostCollisionReact(MovingEntity entity)
		{
			entity.ReactToCollision(mResult.normal);
		}
	}
}
