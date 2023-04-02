namespace AridArnold
{
	class PlatformTile : Tile
	{
		#region rInitialisation

		/// <summary>
		/// Platform tile constructor
		/// </summary>
		/// <param name="rotation"></param>
		public PlatformTile(CardinalDirection rotation, Vector2 position) : base(position)
		{
			mRotation = rotation;
		}

		#endregion rInitialisation





		#region rCollision

		/// <summary>
		/// Resolve collision with an entity. Note: Some entities can pass through us.
		/// </summary>
		/// <param name="entity">Entity that is colliding with us</param>
		/// <param name="gameTime">Frame time</param>
		/// <returns></returns>
		public override CollisionResults Collide(MovingEntity entity, GameTime gameTime)
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

			return Collision2D.MovingRectVsPlatform(entity.ColliderBounds(), entity.VelocityToDisplacement(gameTime), mPosition, sTILE_SIZE, mRotation);
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


	/// <summary>
	/// Platform tile that is animated.
	/// </summary>
	class AnimatedPlatformTile : PlatformTile
	{
		#region rMembers

		Animator mAnimation;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Init Animated platform with position and rotation.
		/// </summary>
		public AnimatedPlatformTile(CardinalDirection rotation, Vector2 position) : base(rotation, position)
		{

		}

		/// <summary>
		/// Load assets for this tile.
		/// </summary>
		public override void LoadContent()
		{
			mAnimation = MonoData.I.LoadAnimator("platform");
			base.LoadContent();
		}

		#endregion rIntialisation





		#region rUpdate

		/// <summary>
		/// Update platform animations.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mAnimation.Update(gameTime);

			base.Update(gameTime);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get texture for this tile
		/// </summary>
		public override Texture2D GetTexture()
		{
			return mAnimation.GetCurrentTexture();
		}

		#endregion rDraw
	}
}
