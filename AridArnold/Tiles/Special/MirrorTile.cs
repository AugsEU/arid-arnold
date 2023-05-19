using AridArnold.Tiles.Basic;

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
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Mirror/Mirror");
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

				bool leftThresh = entityCentre.X > ourCentre.X - diffThresh || (((uint)mAdjacency & (uint)AdjacencyType.Ad4) == (uint)AdjacencyType.Ad4);
				bool rightThresh = entityCentre.X < ourCentre.X + diffThresh || (((uint)mAdjacency & (uint)AdjacencyType.Ad6) == (uint)AdjacencyType.Ad6);

				bool upThresh = entityCentre.Y > ourCentre.Y - diffThresh || (((uint)mAdjacency & (uint)AdjacencyType.Ad8) == (uint)AdjacencyType.Ad8);
				bool downThresh = entityCentre.Y < ourCentre.Y + diffThresh || (((uint)mAdjacency & (uint)AdjacencyType.Ad2) == (uint)AdjacencyType.Ad2);

				bool xThresh = leftThresh && rightThresh;
				bool yThresh = downThresh && upThresh;

				switch (mRotation)
				{
					case CardinalDirection.Up:
						if (platformingEntity.GetGravityDir() != CardinalDirection.Down && xThresh)
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
						if (platformingEntity.GetGravityDir() != CardinalDirection.Left && yThresh)
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
						if (platformingEntity.GetGravityDir() != CardinalDirection.Up && xThresh)
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
						if (platformingEntity.GetGravityDir() != CardinalDirection.Right && yThresh)
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
