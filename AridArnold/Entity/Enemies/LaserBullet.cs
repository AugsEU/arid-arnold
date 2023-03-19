namespace AridArnold
{
	internal class LaserBullet : Entity
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

			Rect2f collider = ColliderBounds();
			bool shouldDelete = CheckHitSolid(ref collider) || CheckOffScreen(ref collider);

			base.Update(gameTime);

			if(shouldDelete) EntityManager.I.QueueDeleteEntity(this);
		}



		/// <summary>
		/// Returns true if we are off screen.
		/// </summary>
		bool CheckOffScreen(ref Rect2f collider)
		{
			// Check X
			if(collider.max.X < -Tile.sTILE_SIZE || collider.min.X > TileManager.I.GetDrawWidth() + Tile.sTILE_SIZE * 4.0f)
			{
				return true;
			}

			// Check Y
			if(collider.max.Y < -Tile.sTILE_SIZE || collider.min.Y > TileManager.I.GetDrawHeight() + Tile.sTILE_SIZE * 4.0f)
			{
				return true;
			}

			return false;
		}



		/// <summary>
		/// Returns true if we are touching a solid tile.
		/// </summary>
		bool CheckHitSolid(ref Rect2f collider)
		{
			return TileManager.I.DoesRectTouchTiles(collider);
		}



		/// <summary>
		/// Collider bounds.
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			return new Rect2f(mPosition, mTexture);
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
