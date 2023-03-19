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
		public LaserBullet(Vector2 pos, CardinalDirection direction, Texture2D texture) : base(pos)
		{
			mDirection = direction;
			mTexture = texture;
		}



		/// <summary>
		/// Load content.
		/// </summary>
		public override void LoadContent(ContentManager content)
		{
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update bullet
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);
			Vector2 dir = Util.GetNormal(mDirection);

			mPosition += dir * dt * LASER_SPEED;

			base.Update(gameTime);
		}






		/// <summary>
		/// Collider bounds.
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			return new Rect2f(mPosition, mTexture);
		}



		/// <summary>
		/// React to collision with an entity.
		/// </summary>
		/// <param name="entity"></param>
		public override void CollideWithEntity(Entity entity)
		{
			if (entity is Arnold)
			{
				//Kill the player on touching.
				EArgs args;
				args.sender = this;

				EventManager.I.SendEvent(EventType.KillPlayer, args);
				EntityManager.I.QueueDeleteEntity(this);
			}

			base.CollideWithEntity(entity);
		}

		#endregion rUpdate





		#region rDraw

		public override void Draw(DrawInfo info)
		{
			// To do: add up and down!

			SpriteEffects effect = SpriteEffects.None;
			Vector2 drawPos = mPosition;
			switch (mDirection)
			{
				case CardinalDirection.Right:
					break;
				case CardinalDirection.Left:
					effect = SpriteEffects.FlipHorizontally;
					drawPos.X += mTexture.Width;
					break;
			}

			drawPos = MonoMath.Round(drawPos);
			MonoDraw.DrawTexture(info, mTexture, drawPos, null, Color.White, 0.0f, Vector2.Zero, 1.0f, effect, MonoDraw.LAYER_TILE );
		}

		#endregion rDraw
	}
}
