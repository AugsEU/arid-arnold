namespace AridArnold
{
	/// <summary>
	/// Represents a tile that falls with gravity.
	/// Not really a "platforming entity" but we want to inherit gravity.
	/// </summary>
	internal class GravityTile : PlatformingEntity
	{
		const float KILL_THRESH = 1.0f;

		public GravityTile(Vector2 pos) : base(pos, 0, 0, 4.5f)
		{
		}

		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Library/GravityTile");
		}

		public override void Draw(DrawInfo info)
		{
			MonoDraw.DrawTextureDepth(info, mTexture, mPosition, DrawLayer.TileEffects);
		}

		public override void Update(GameTime gameTime)
		{
			mVelocity = new Vector2(MonoMath.ClampAbs(mVelocity.X, 60.0f), MonoMath.ClampAbs(mVelocity.Y, 60.0f));

			//Collider
			EntityManager.I.AddColliderSubmission(new EntityColliderSubmission(this));
			base.Update(gameTime);
		}

		public override void OnCollideEntity(Entity entity)
		{
			if (mVelocity.LengthSquared() > KILL_THRESH * KILL_THRESH)
			{
				entity.Kill();
			}

			base.OnCollideEntity(entity);
		}



		/// <summary>
		/// This entity can't be killed.
		/// </summary>
		public override void Kill()
		{
		}
	}
}
