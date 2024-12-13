namespace AridArnold
{
	internal class LaserBullet : ProjectileEntity
	{
		#region rConstants

		const float LASER_SPEED = 12.0f;

		#endregion rConstants





		#region rMembers

		CardinalDirection mDirection;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Init bullet with texture.
		/// </summary>
		public LaserBullet(Entity parent, Vector2 pos, CardinalDirection direction) : base(parent, pos, 0.0f)
		{
			mDirection = direction;
			mVelocity = Util.GetNormal(direction) * LASER_SPEED;
		}



		/// <summary>
		/// Load laser texture
		/// </summary>
		public override void LoadContent()
		{
			const float EFT = 0.08f;
			mExplodingAnim = new Animator(Animator.PlayType.OneShot, ("Enemies/Futron-Gun/Explode1", EFT)
																   , ("Enemies/Futron-Gun/Explode2", EFT)
																   , ("Enemies/Futron-Gun/Explode3", EFT)
																   , ("Enemies/Futron-Gun/Explode4", EFT)
																   , ("Enemies/Futron-Gun/Explode5", EFT)
																   , ("Enemies/Futron-Gun/Explode6", EFT)
																   , ("Enemies/Futron-Gun/Explode7", EFT)
																   , ("Enemies/Futron-Gun/Explode8", EFT));
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Enemies/Futron-Gun/Bullet");

			if (mDirection == CardinalDirection.Left)
			{
				mPosition.X += mTexture.Width;
			}

			SpacialSFX laserTravel = new SpacialSFX(AridArnoldSFX.FutronLaser, mPosition, 0.5f, 0.0f, 0.1f);
			laserTravel.GetBuffer().SetLoop(true);
			SpacialSFX laserLand = new SpacialSFX(AridArnoldSFX.FutronLaserLand, mPosition, 0.3f, 0.0f, 0.1f);
			LoadSFX(laserTravel, laserLand);

			// Play this one to start things off.
			SFXManager.I.PlaySFX(AridArnoldSFX.FutronLaserShoot, 0.05f, 0.0f, 0.1f);
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update bullet
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			SetBulletDirection(Util.CardinalDirectionFromVector(mVelocity));
			mVelocity = LASER_SPEED * Vector2.Normalize(mVelocity); // Keep speed consistent
			base.Update(gameTime);
		}



		/// <summary>
		/// React to collision with an entity.
		/// </summary>
		/// <param name="entity"></param>
		public override void OnCollideEntity(Entity entity)
		{
			if (mState == ProjectileState.FreeMotion)
			{
				if (entity != this && entity != mParent)
				{
					KillEntity(entity);
				}
			}

			base.OnCollideEntity(entity);
		}



		/// <summary>
		/// Collider for bullet
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			switch (mDirection)
			{
				case CardinalDirection.Up:
				case CardinalDirection.Down:
					return new Rect2f(mPosition, mTexture.Height, mTexture.Width);
				case CardinalDirection.Right:
				case CardinalDirection.Left:
					return new Rect2f(mPosition, mTexture.Width, mTexture.Height);
			}

			throw new NotImplementedException();
		}


		/// <summary>
		/// Set direction
		/// </summary>
		private void SetBulletDirection(CardinalDirection newDir)
		{
			CardinalDirection currDir = mDirection;

			if (currDir != newDir)
			{
				Vector2 centre = GetCentrePos();
				mDirection = newDir;

				// Re-centre after collider changes.
				SetCentrePos(centre);
			}
		}

		#endregion rUpdate





		#region rDraw

		public override void Draw(DrawInfo info)
		{
			SpriteEffects effect = SpriteEffects.None;
			Vector2 drawPos = mPosition;
			Texture2D texToDraw = mTexture;

			// Bodge position
			if (mState == ProjectileState.Exploding)
			{
				texToDraw = mExplodingAnim.GetCurrentTexture();
				drawPos = mExplosionCentre;

				switch (mDirection)
				{
					case CardinalDirection.Up:
						drawPos.X += 6.0f;
						//drawPos.Y += texToDraw.Width;
						break;
					case CardinalDirection.Down:
						drawPos.X += 6.0f;
						// Only fish people need this?
						drawPos.Y -= texToDraw.Width;
						break;
					case CardinalDirection.Right:
						drawPos.X -= texToDraw.Width;
						drawPos.Y -= 3.0f;
						break;
					case CardinalDirection.Left:
						drawPos.Y -= 3.0f;
						break;
				}
			}
			else
			{
				switch (mDirection)
				{
					case CardinalDirection.Down:
					case CardinalDirection.Up:
						drawPos.X += mTexture.Height;
						drawPos.Y -= 1.0f;
						break;
					case CardinalDirection.Right:
						break;
					case CardinalDirection.Left:
						drawPos.Y -= 1.0f;
						drawPos.X += 1.0f;
						break;
				}
			}

			// Effect
			switch (mDirection)
			{
				case CardinalDirection.Down:
				case CardinalDirection.Right:
					break;
				case CardinalDirection.Up:
				case CardinalDirection.Left:
					effect = SpriteEffects.FlipHorizontally;
					break;
			}

			drawPos = MonoMath.Round(drawPos);

			float rotation = 0.0f;
			if (mDirection == CardinalDirection.Up || mDirection == CardinalDirection.Down)
			{
				rotation = MathF.PI / 2.0f;
			}

			MonoDraw.DrawTexture(info, texToDraw, drawPos, null, Color.White, rotation, Vector2.Zero, 1.0f, effect, DrawLayer.Tile);
		}

		#endregion rDraw
	}
}
