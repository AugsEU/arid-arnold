namespace AridArnold
{
	class MushroomTile : InteractableTile
	{
		#region rConstants

		const float DOWN_VEL_THRESH = 0.5f;

		#endregion rConstants

		#region rMembers

		Animator mBounceAnim;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Mushroom tile constructor
		/// </summary>
		/// <param name="rotation">Rotation of mushroom</param>
		public MushroomTile(CardinalDirection rotation, Vector2 position) : base(position)
		{
			mRotation = rotation;
		}



		/// <summary>
		/// Load all textures and assets
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/buk/Mushroom");
			mBounceAnim = new Animator(Animator.PlayType.OneShot,
											("Tiles/buk/MushroomBounce1", 0.05f),
											("Tiles/buk/MushroomBounce2", 0.1f),
											("Tiles/buk/MushroomBounce1", 0.05f),
											("Tiles/buk/MushroomBounce3", 0.05f),
											("Tiles/buk/MushroomBounce4", 0.05f),
											("Tiles/buk/MushroomBounce3", 0.05f));
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update the tile
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public override void Update(GameTime gameTime)
		{
			mBounceAnim.Update(gameTime);
		}

		#endregion rUpdate




		#region rDraw

		/// <summary>
		/// Get texture for this tile
		/// </summary>
		/// <returns>Texture reference</returns>
		public override Texture2D GetTexture()
		{
			if (mBounceAnim.IsPlaying())
			{
				return mBounceAnim.GetCurrentTexture();
			}

			return mTexture;
		}

		#endregion rDraw





		#region rCollision

		/// <summary>
		/// Make the intersecting entity bounce.
		/// </summary>
		/// <param name="entity">Entity that intersected us</param>
		public override void OnEntityIntersect(Entity entity)
		{
			float minVel = 19.5f;
			float alpha = 1.4f;

			if (entity is PlatformingEntity)
			{
				PlatformingEntity platformingEntity = (PlatformingEntity)entity;
				if(platformingEntity.IsUsingRealPhysics())
				{
					alpha = 1.0f;
					minVel = 0.0f;
				}
				Rect2f entityBounds = platformingEntity.ColliderBounds();
				Rect2f bounds = GetBounds();

				Vector2 entityVel = platformingEntity.GetVelocity();
				Vector2 entityPos = platformingEntity.GetPos();

				// TO DO: Make this nicer.
				bool didBounce = false;
				switch (mRotation)
				{
					case CardinalDirection.Up:
					{
						if (!platformingEntity.IsGroundedSince(4))
						{
							if (entityVel.Y > DOWN_VEL_THRESH)
							{
								float bounceVel = MathF.Max(minVel, entityVel.Y);
								platformingEntity.SetVelocity(new Vector2(entityVel.X, -bounceVel * alpha));
								didBounce = true;
							}

							if (didBounce)
							{
								float newY = bounds.min.Y - entityBounds.Height;
								platformingEntity.SetPos(new Vector2(entityPos.X, newY));
								platformingEntity.AllowWalkChangeFor(1);
							}
						}
					}
					break;
					case CardinalDirection.Left:
					case CardinalDirection.Right:
					{
						bool valid = (CardinalDirection.Left == mRotation) != (entityVel.X < 0.0f);

						if (valid)
						{
							float bounceVel = platformingEntity.OnGround() ? -minVel * alpha : entityVel.Y;
							platformingEntity.SetVelocity(new Vector2(-entityVel.X, bounceVel));
							platformingEntity.ReverseWalkDirection();

							didBounce = true;
						}
					}
					break;
					case CardinalDirection.Down:
					{
						if (!platformingEntity.IsGroundedSince(4))
						{
							if (entityVel.Y < 0.0f)
							{
								platformingEntity.SetVelocity(new Vector2(entityVel.X, -entityVel.Y * alpha));
								didBounce = true;
							}
						}
					}

					break;
				}

				if (didBounce)
				{
					mBounceAnim.Play();
				}
			}
		}



		/// <summary>
		/// Get bounds of this tile.
		/// </summary>
		/// <param name="topLeft">Top left position of tile.</param>
		/// <param name="sideLength">Side length of tile</param>
		/// <returns>Collision rectangle</returns>
		/// <exception cref="NotImplementedException">Requires a valid cardinal direction.</exception>
		protected override Rect2f CalculateBounds()
		{
			float heightReduction = 6.0f;

			switch (mRotation)
			{
				case CardinalDirection.Up:
					return new Rect2f(mPosition + new Vector2(0.0f, heightReduction), mPosition + new Vector2(sTILE_SIZE, sTILE_SIZE));
				case CardinalDirection.Right:
					return new Rect2f(mPosition, mPosition + new Vector2(sTILE_SIZE - heightReduction, sTILE_SIZE));
				case CardinalDirection.Down:
					return new Rect2f(mPosition, mPosition + new Vector2(sTILE_SIZE, sTILE_SIZE - heightReduction));
				case CardinalDirection.Left:
					return new Rect2f(mPosition + new Vector2(heightReduction, 0.0f), mPosition + new Vector2(sTILE_SIZE, sTILE_SIZE));
			}

			throw new NotImplementedException();
		}

		#endregion rCollision
	}
}
