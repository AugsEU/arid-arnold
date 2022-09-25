namespace AridArnold
{
	class PlatformTile : Tile
	{
		#region rInitialisation

		/// <summary>
		/// Platform tile constructor
		/// </summary>
		/// <param name="rotation"></param>
		public PlatformTile(CardinalDirection rotation) : base()
		{
			mRotation = rotation;
		}



		/// <summary>
		/// Load all textures and assets
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public override void LoadContent(ContentManager content)
		{
			mTexture = content.Load<Texture2D>("Tiles/" + ProgressManager.I.GetWorldData().platformTexture);
		}

		#endregion rInitialisation





		#region rCollision

		/// <summary>
		/// Resolve collision with an entity. Note: Some entities can pass through us.
		/// </summary>
		/// <param name="entity">Entity that is colliding with us</param>
		/// <param name="topLeft">Top left position of this tile</param>
		/// <param name="sideLength">Side length of this tile</param>
		/// <param name="gameTime">Frame time</param>
		/// <returns></returns>
		public override CollisionResults Collide(MovingEntity entity, Vector2 topLeft, float sideLength, GameTime gameTime)
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

			return Collision2D.MovingRectVsPlatform(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), topLeft, sideLength, mRotation);
		}



		/// <summary>
		/// Is this tile solid?
		/// </summary>
		/// <returns>True if a tile is solid</returns>
		public override bool IsSolid()
		{
			return true;
		}

		#endregion rCollision
	}
}
