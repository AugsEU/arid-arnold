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



		/// <summary>
		/// Create an appropriate platform texture.
		/// </summary>
		public static PlatformTile InstantiatePlatformTile(Vector2 pos, int param)
		{
			CardinalDirection rot = (CardinalDirection)param;
			int worldIndex = ProgressManager.I.GetLevelPoint().mWorldIndex;

			switch (worldIndex)
			{
				case 0: //Iron Works
					return new StaticPlatformTile("IronWorks/platform", rot, pos);
				case 1: //Land of mirrors
					return new StaticPlatformTile("Mirror/gold-platform", rot, pos);
				case 2: //Buk's Cave
					return new StaticPlatformTile("Buk/cave-platform", rot, pos);
				case 3: //The Lab
					return new LabPlatform(rot, pos);
				default:
					break;
			}

			throw new NotImplementedException();
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
	/// Platform with static texture.
	/// </summary>
	class StaticPlatformTile : PlatformTile
	{
		string mTexturePath;

		/// <summary>
		/// Platform tile constructor
		/// </summary>
		/// <param name="rotation"></param>
		public StaticPlatformTile(string texturePath, CardinalDirection rotation, Vector2 position) : base(rotation, position)
		{
			mRotation = rotation;
			mTexturePath = texturePath;
		}



		/// <summary>
		/// Load all textures and assets
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public override void LoadContent(ContentManager content)
		{
			mTexture = content.Load<Texture2D>("Tiles/" + mTexturePath);
		}
	}



	/// <summary>
	/// Platform with static texture.
	/// </summary>
	class AnimPlatformTile : PlatformTile
	{
		protected Animator mAnimation;

		/// <summary>
		/// Platform tile constructor
		/// </summary>
		/// <param name="rotation"></param>
		public AnimPlatformTile(CardinalDirection rotation, Vector2 position) : base(rotation, position)
		{
			mRotation = rotation;
		}



		/// <summary>
		/// Update platform animations.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mAnimation.Update(gameTime);

			base.Update(gameTime);
		}



		/// <summary>
		/// Get texture for this tile
		/// </summary>
		public override Texture2D GetTexture()
		{
			return mAnimation.GetCurrentTexture();
		}
	}
}
