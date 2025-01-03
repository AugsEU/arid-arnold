namespace AridArnold
{
	/// <summary>
	/// Represents collision between tile and an entity.
	/// </summary>
	class TileEntityCollision : SolidEntityCollision
	{
		Tile mTile;

		public TileEntityCollision(bool firstTime, CollisionResults result, Tile tile) : base(firstTime, result)
		{
			mTile = tile;
		}

		public override void PostCollisionReact(MovingEntity entity)
		{
			mTile.OnTouch(entity, mResult);
			base.PostCollisionReact(entity);
		}
	}
}
