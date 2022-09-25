namespace AridArnold
{
	class MirrorTile : PlatformTile
	{
		#region rInitialisation

		/// <summary>
		/// Mirror tile constructor
		/// </summary>
		/// <param name="rotation">Orientation of mirror</param>
		public MirrorTile(CardinalDirection rotation) : base(rotation)
		{
		}


		/// <summary>
		/// Load all textures and assets
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public override void LoadContent(ContentManager content)
		{
			mTexture = content.Load<Texture2D>("Tiles/mirror");
		}

		#endregion rInitialisation





		#region rCollision

		/// <summary>
		/// Flip gravity when an entity passes through this
		/// </summary>
		/// <param name="entity">Entity that intersected us</param>
		/// <param name="bounds">Our tile bounds</param>
		public override void OnEntityIntersect(Entity entity, Rect2f bounds)
		{
			if (entity is PlatformingEntity)
			{
				PlatformingEntity platformingEntity = (PlatformingEntity)entity;

				switch (mRotation)
				{
					case CardinalDirection.Up:
						if (platformingEntity.GetGravityDir() != CardinalDirection.Down)
						{
							if (platformingEntity.GetGravityDir() == CardinalDirection.Left || platformingEntity.GetGravityDir() == CardinalDirection.Right)
							{
								platformingEntity.SetWalkDirection(WalkDirection.None);
							}
							platformingEntity.SetGravity(CardinalDirection.Down);
							platformingEntity.ShiftPosition(new Vector2(0.0f, -(bounds.Height + 16.0f)));
						}
						break;
					case CardinalDirection.Right:
						if (platformingEntity.GetGravityDir() != CardinalDirection.Left)
						{
							if (platformingEntity.GetGravityDir() == CardinalDirection.Up || platformingEntity.GetGravityDir() == CardinalDirection.Down)
							{
								platformingEntity.SetWalkDirection(WalkDirection.None);
							}
							platformingEntity.SetGravity(CardinalDirection.Left);
							platformingEntity.ShiftPosition(new Vector2(bounds.Width + 16.0f, 0.0f));
						}
						break;
					case CardinalDirection.Down:
						if (platformingEntity.GetGravityDir() != CardinalDirection.Up)
						{
							if (platformingEntity.GetGravityDir() == CardinalDirection.Left || platformingEntity.GetGravityDir() == CardinalDirection.Right)
							{
								platformingEntity.SetWalkDirection(WalkDirection.None);
							}
							platformingEntity.SetGravity(CardinalDirection.Up);
							platformingEntity.ShiftPosition(new Vector2(0.0f, bounds.Height + 16.0f));
						}
						break;
					case CardinalDirection.Left:
						if (platformingEntity.GetGravityDir() != CardinalDirection.Right)
						{
							if (platformingEntity.GetGravityDir() == CardinalDirection.Up || platformingEntity.GetGravityDir() == CardinalDirection.Down)
							{
								platformingEntity.SetWalkDirection(WalkDirection.None);
							}
							platformingEntity.SetGravity(CardinalDirection.Right);
							platformingEntity.ShiftPosition(new Vector2(-(bounds.Width + 16.0f), 0.0f));
						}
						break;
				}


			}
		}



		/// <summary>
		/// Resolve collision with an entity. Note: Only the top side of the mirror is solid.
		/// </summary>
		/// <param name="entity">Entity that is colliding with us</param>
		/// <param name="topLeft">Top left position of this tile</param>
		/// <param name="sideLength">Side length of this tile</param>
		/// <param name="gameTime">Frame time</param>
		public override CollisionResults Collide(MovingEntity entity, Vector2 topLeft, float sideLength, GameTime gameTime)
		{
			return Collision2D.MovingRectVsPlatform(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), topLeft, sideLength, mRotation);
		}

		#endregion rCollision
	}
}
