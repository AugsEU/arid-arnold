using Microsoft.Xna.Framework.Graphics;

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
		public LaserBullet(Vector2 pos, CardinalDirection direction) : base(pos)
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
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Enemies/Futron-Gun/bullet");

			if (mDirection == CardinalDirection.Left)
			{
				mPosition.X += mTexture.Width;
			}
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update bullet
		/// </summary>
		public override void Update(GameTime gameTime)
		{
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
				if (entity is Arnold)
				{
					KillPlayer((MovingEntity)entity);
				}
			}

			base.OnCollideEntity(entity);
		}

		#endregion rUpdate





		#region rDraw

		public override void Draw(DrawInfo info)
		{
			// To do: add up and down!

			SpriteEffects effect = SpriteEffects.None;
			Vector2 drawPos = mPosition;
			Texture2D texToDraw = mTexture;

			if (mState == ProjectileState.Exploding)
			{
				texToDraw = mExplodingAnim.GetCurrentTexture();
				drawPos = mExplosionCentre;
				drawPos.Y -= 3.0f;
			}

			switch (mDirection)
			{
				case CardinalDirection.Right:
					break;
				case CardinalDirection.Left:
					effect = SpriteEffects.FlipHorizontally;
					break;
			}

			drawPos = MonoMath.Round(drawPos);

			MonoDraw.DrawTexture(info, texToDraw, drawPos, null, Color.White, 0.0f, Vector2.Zero, 1.0f, effect, DrawLayer.Tile);
		}

		#endregion rDraw
	}
}
