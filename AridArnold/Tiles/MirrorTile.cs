namespace AridArnold
{
	class MirrorTile : PlatformTile
	{
		#region rInitialisation

		/// <summary>
		/// Mirror tile constructor
		/// </summary>
		/// <param name="rotation">Orientation of mirror</param>
		public MirrorTile(CardinalDirection rotation, Vector2 position) : base(rotation, position)
		{
		}


		/// <summary>
		/// Load all textures and assets
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public override void LoadContent(ContentManager content)
		{
			mTexture = content.Load<Texture2D>("Tiles/Mirror/mirror");
		}

		#endregion rInitialisation





		#region rCollision

		/// <summary>
		/// Flip gravity when an entity passes through this
		/// </summary>
		/// <param name="entity">Entity that intersected us</param>
		/// <param name="bounds">Our tile bounds</param>
		public override void OnEntityIntersect(Entity entity)
		{
			float diffThresh = 4.0f;

			if (entity is PlatformingEntity)
			{
				Rect2f ourBounds = GetBounds();

				PlatformingEntity platformingEntity = (PlatformingEntity)entity;

				Rect2f entityBounds = entity.ColliderBounds();
				float entityHeight = entityBounds.Height;
				float entityWidth = entityBounds.Width;

				Vector2 entityCentre = entityBounds.Centre;
				Vector2 ourCentre = ourBounds.Centre;
				float xDiff = Math.Abs(entityCentre.X - ourCentre.X);
				float yDiff = Math.Abs(entityCentre.Y - ourCentre.Y);

				switch (mRotation)
				{
					case CardinalDirection.Up:
						if (platformingEntity.GetGravityDir() != CardinalDirection.Down && xDiff < diffThresh)
						{
							if (platformingEntity.GetGravityDir() == CardinalDirection.Left || platformingEntity.GetGravityDir() == CardinalDirection.Right)
							{
								platformingEntity.SetWalkDirection(WalkDirection.None);
							}
							platformingEntity.SetGravity(CardinalDirection.Down);
							platformingEntity.ShiftPosition(new Vector2(0.0f, -(ourBounds.Height + entityHeight)));
						}
						break;
					case CardinalDirection.Right:
						if (platformingEntity.GetGravityDir() != CardinalDirection.Left && yDiff < diffThresh)
						{
							if (platformingEntity.GetGravityDir() == CardinalDirection.Up || platformingEntity.GetGravityDir() == CardinalDirection.Down)
							{
								platformingEntity.SetWalkDirection(WalkDirection.None);
							}
							platformingEntity.SetGravity(CardinalDirection.Left);
							platformingEntity.ShiftPosition(new Vector2(ourBounds.Width + entityWidth, 0.0f));
						}
						break;
					case CardinalDirection.Down:
						if (platformingEntity.GetGravityDir() != CardinalDirection.Up && xDiff < diffThresh)
						{
							if (platformingEntity.GetGravityDir() == CardinalDirection.Left || platformingEntity.GetGravityDir() == CardinalDirection.Right)
							{
								platformingEntity.SetWalkDirection(WalkDirection.None);
							}
							platformingEntity.SetGravity(CardinalDirection.Up);
							platformingEntity.ShiftPosition(new Vector2(0.0f, ourBounds.Height + entityHeight));
						}
						break;
					case CardinalDirection.Left:
						if (platformingEntity.GetGravityDir() != CardinalDirection.Right && yDiff < diffThresh)
						{
							if (platformingEntity.GetGravityDir() == CardinalDirection.Up || platformingEntity.GetGravityDir() == CardinalDirection.Down)
							{
								platformingEntity.SetWalkDirection(WalkDirection.None);
							}
							platformingEntity.SetGravity(CardinalDirection.Right);
							platformingEntity.ShiftPosition(new Vector2(-(ourBounds.Width + entityWidth), 0.0f));
						}
						break;
				}


			}
		}



		/// <summary>
		/// Resolve collision with an entity. Note: Only the top side of the mirror is solid.
		/// </summary>
		/// <param name="entity">Entity that is colliding with us</param>
		/// <param name="gameTime">Frame time</param>
		public override CollisionResults Collide(MovingEntity entity, GameTime gameTime)
		{
			return Collision2D.MovingRectVsPlatform(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), mPosition, sTILE_SIZE, mRotation);
		}

		#endregion rCollision
	}
}
