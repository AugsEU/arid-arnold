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
			mUseRealPhysics = true;
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

		protected override void ReactToCollision(CollisionType collisionType)
		{
			switch (collisionType)
			{
				case CollisionType.Ground:
					float downVel = Vector2.Dot(mPrevVelocity, GravityVecNorm());
					downVel = MathF.Abs(downVel);
					float thresh = MathF.Abs(10.0f);

					if (downVel > thresh)
					{
						float volumeMod = MonoMath.SquashToRange((downVel - thresh) / 6.0f, -1.0f, 1.0f);
						volumeMod = Math.Clamp(volumeMod, 0.0f, 1.0f);

						SpacialSFX landSFX = new SpacialSFX(AridArnoldSFX.LibraryBlockLand, GetCentrePos(), 0.8f * volumeMod);
						landSFX.SetDistanceCutoff(400.0f);
						SFXManager.I.PlaySFX(landSFX);

						DiminishCameraShake cameraShake = new DiminishCameraShake(volumeMod * 2.7f, volumeMod * 5.0f, 100.0f);
						Camera cam = CameraManager.I.GetCamera(CameraManager.CameraInstance.ScreenCamera);
						cam.DoMovement(cameraShake);
					}
					break;
			}

			base.ReactToCollision(collisionType);
		}


		/// <summary>
		/// This entity can't be killed.
		/// </summary>
		public override void Kill()
		{
		}
	}
}
