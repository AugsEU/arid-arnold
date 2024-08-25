
namespace AridArnold
{
	internal class Fireball : ProjectileEntity
	{

		#region rConstants

		const float FIREBALL_SPEED = 8.0f;
		const float MOVE_AMPLITUDE = 14.0f;
		const float MOVE_FREQUENCY = 0.35f;

		#endregion rConstants





		#region rMembers

		Animator mFireballAnim;
		Vector2 mForwardDir;
		float mPathAngle;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Init bullet with texture.
		/// </summary>
		public Fireball(Entity parent, Vector2 pos, CardinalDirection direction) : base(parent, pos, 0.0f)
		{
			mForwardDir = Util.GetNormal(direction);
			mPathAngle = 0.0f;
		}



		/// <summary>
		/// Load laser texture
		/// </summary>
		public override void LoadContent()
		{
			const float FT = 0.033f;
			const float EFT = 0.1f;
			mExplodingAnim = new Animator(Animator.PlayType.OneShot, ("Enemies/Papyras/Explode1", EFT)
																   , ("Enemies/Papyras/Explode2", EFT)
																   , ("Enemies/Papyras/Explode3", EFT));
			mFireballAnim = new Animator(Animator.PlayType.Repeat, ("Enemies/Papyras/FireBall1", FT)
																 , ("Enemies/Papyras/FireBall2", FT)
																 , ("Enemies/Papyras/FireBall3", FT)
																 , ("Enemies/Papyras/FireBall4", FT));
			mFireballAnim.Play();
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Enemies/Papyras/FireBall1");

			LoadSFX(new SpacialSFX(AridArnoldSFX.FireTravel, mPosition, 0.4f), new SpacialSFX(AridArnoldSFX.FireLand, mPosition, 0.4f));
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update bullet
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);
			mPathAngle += (dt * MOVE_FREQUENCY) % MathF.PI * 2.0f;
			Vector2 upDownComponent = -MathF.Cos(mPathAngle) * Util.GetNormal(GetGravityDir());
			mVelocity = FIREBALL_SPEED * mForwardDir + MOVE_AMPLITUDE * upDownComponent;

			mFireballAnim.Update(gameTime);

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
				if (entity != this && entity != mParent && entity is PlatformingEntity)
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
			return new Rect2f(mPosition + new Vector2(1.0f, 1.0f), 4.0f, 4.0f);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the fireball
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			SpriteEffects effect = SpriteEffects.None;
			Vector2 drawPos = mPosition;
			Texture2D texToDraw = mFireballAnim.GetCurrentTexture();
			float rotation = 0.0f;

			// Bodge position
			if (mState == ProjectileState.Exploding)
			{
				texToDraw = mExplodingAnim.GetCurrentTexture();
				drawPos = mExplosionCentre;

				CardinalDirection direction = Util.CardinalDirectionFromVector(mExplosionNormal);

				switch (direction)
				{
					case CardinalDirection.Up:
						drawPos.X += 4.0f;
						drawPos.Y -= texToDraw.Width;
						break;
					case CardinalDirection.Down:
						drawPos.X += 4.0f;
						break;
					case CardinalDirection.Right:
						//drawPos.X -= texToDraw.Width;
						drawPos.Y -= 3.0f;
						break;
					case CardinalDirection.Left:
						drawPos.Y -= 3.0f;
						drawPos.X -= 4.0f;
						break;
				}


				// Effect
				switch (direction)
				{
					case CardinalDirection.Down:
						rotation = MathF.PI / 2.0f;
						effect = SpriteEffects.FlipHorizontally;
						break;
					case CardinalDirection.Left:
						break;
					case CardinalDirection.Up:
						rotation = MathF.PI / 2.0f;
						break;
					case CardinalDirection.Right:
						rotation = 0.0f;
						effect = SpriteEffects.FlipHorizontally;
						break;
				}
			}

			drawPos = MonoMath.Round(drawPos);

			MonoDraw.DrawTexture(info, texToDraw, drawPos, null, Color.White, rotation, Vector2.Zero, 1.0f, effect, DrawLayer.Projectiles);
		}

		#endregion rDraw
	}
}
