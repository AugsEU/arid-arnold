namespace AridArnold
{
	/// <summary>
	/// Represents collision between tile and an entity.
	/// </summary>
	class TileEntityCollision : SolidEntityCollision
	{
		Point mTileCoord;

		public TileEntityCollision(bool firstTime, CollisionResults result, Point tileCoord) : base(firstTime, result)
		{
			mTileCoord = tileCoord;
		}

		public override void PostCollisionReact(MovingEntity entity)
		{
			TileManager.I.GetTile(mTileCoord).OnTouch(entity, mResult);

			base.PostCollisionReact(entity);
		}
	}
}
