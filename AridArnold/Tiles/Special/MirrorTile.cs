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
				PlatformingEntity platformingEntity = (PlatformingEntity)entity;

				if(ShouldTriggerMirror(platformingEntity))
				{
					ReflectEntity(platformingEntity);
				}
			}
		}



		/// <summary>
		/// Should an entity intersecting us trigger the mirror?
		/// </summary>
		bool ShouldTriggerMirror(PlatformingEntity entity)
		{
			const float SIDE_HITBOX = 4.0f;

			Rect2f ourBounds = GetBounds();
			Vector2 ourCentre = ourBounds.Centre;

			Rect2f entityBounds = entity.ColliderBounds();
			Vector2 entityCentre = entityBounds.Centre;

			Vector2 mirrorOut = -Util.GetNormal(mRotation);
			Vector2 ourCentreToEntity = entityCentre - ourCentre;

			float entityDot = Vector2.Dot(mirrorOut, ourCentreToEntity);

			if(entityDot < 0.0f)
			{
				return false;
			}

			bool leftThresh = entityCentre.X > ourCentre.X - SIDE_HITBOX || (((uint)mAdjacency & (uint)AdjacencyType.Ad4) == (uint)AdjacencyType.Ad4);
			bool rightThresh = entityCentre.X < ourCentre.X + SIDE_HITBOX || (((uint)mAdjacency & (uint)AdjacencyType.Ad6) == (uint)AdjacencyType.Ad6);

			bool upThresh = entityCentre.Y > ourCentre.Y - SIDE_HITBOX || (((uint)mAdjacency & (uint)AdjacencyType.Ad8) == (uint)AdjacencyType.Ad8);
			bool downThresh = entityCentre.Y < ourCentre.Y + SIDE_HITBOX || (((uint)mAdjacency & (uint)AdjacencyType.Ad2) == (uint)AdjacencyType.Ad2);

			bool xThresh = leftThresh && rightThresh;
			bool yThresh = downThresh && upThresh;

			switch (mRotation)
			{
				case CardinalDirection.Up:
					return entity.GetGravityDir() != CardinalDirection.Down && xThresh;
				case CardinalDirection.Right:
					return entity.GetGravityDir() != CardinalDirection.Left && yThresh;
				case CardinalDirection.Down:
					return entity.GetGravityDir() != CardinalDirection.Up && xThresh;
				case CardinalDirection.Left:
					return entity.GetGravityDir() != CardinalDirection.Right && yThresh;
				default:
					break;
			}

			throw new NotImplementedException();
		}



		/// <summary>
		/// Do the
		/// </summary>
		void ReflectEntity(PlatformingEntity entity)
		{
			Rect2f ourBounds = GetBounds();
			Vector2 ourCentre = ourBounds.Centre;

			Rect2f entityBounds = entity.ColliderBounds();
			float entityHeight = entityBounds.Height;
			float entityWidth = entityBounds.Width;

			switch (mRotation)
			{
				case CardinalDirection.Up:
					if (entity.GetGravityDir() == CardinalDirection.Left || entity.GetGravityDir() == CardinalDirection.Right)
					{
						entity.SetWalkDirection(WalkDirection.None);
					}
					entity.SetGravity(CardinalDirection.Down);

					Vector2 entityPos = entity.GetPos();
					entityPos.Y = ourCentre.Y - sTILE_SIZE * 0.5f - entityHeight;
					entity.SetPos(entityPos);
					break;

				case CardinalDirection.Right:
					if (entity.GetGravityDir() == CardinalDirection.Up || entity.GetGravityDir() == CardinalDirection.Down)
					{
						entity.SetWalkDirection(WalkDirection.None);
					}
					entity.SetGravity(CardinalDirection.Left);
					entity.ShiftPosition(new Vector2(ourBounds.Width + entityWidth, 0.0f));
					break;

				case CardinalDirection.Down:
					if (entity.GetGravityDir() == CardinalDirection.Left || entity.GetGravityDir() == CardinalDirection.Right)
					{
						entity.SetWalkDirection(WalkDirection.None);
					}
					entity.SetGravity(CardinalDirection.Up);
					entity.ShiftPosition(new Vector2(0.0f, ourBounds.Height + entityHeight));
					break;

				case CardinalDirection.Left:
					if (entity.GetGravityDir() == CardinalDirection.Up || entity.GetGravityDir() == CardinalDirection.Down)
					{
						entity.SetWalkDirection(WalkDirection.None);
					}
					entity.SetGravity(CardinalDirection.Right);
					entity.ShiftPosition(new Vector2(-(ourBounds.Width + entityWidth), 0.0f));
					break;
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
