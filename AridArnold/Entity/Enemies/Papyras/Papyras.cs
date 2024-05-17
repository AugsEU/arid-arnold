namespace AridArnold
{
	internal class Papyras : ShooterEnemy
	{
		#region rConstants

		static float BULLET_OFFSET = -4.0f;

		#endregion rConstants





		#region rInitialisation

		/// <summary>
		/// Init Papyras at position.
		/// </summary>
		public Papyras(Vector2 pos, float shootPhase, float shootFreq) : base(pos, shootPhase, shootFreq)
		{
			mGravity = 0.0f;

			// Don't want gravity orb to affect us
			LayerOptOut(InteractionLayer.kGravityOrb);
		}



		/// <summary>
		/// Load textures for Papyras
		/// </summary>
		public override void LoadContent()
		{
			const float FT = 0.3f;
			mIdleAnim = new Animator(Animator.PlayType.Repeat, ("Enemies/Papyras/Idle1", FT));

			mChargeGunAnim = new Animator(Animator.PlayType.OneShot, ("Enemies/Papyras/Charge1", FT),
																	 ("Enemies/Papyras/Idle1", FT),
																	 ("Enemies/Papyras/Charge3", FT),
																	 ("Enemies/Papyras/Charge2", FT));

			mShootGunAnim = new Animator(Animator.PlayType.OneShot, ("Enemies/Papyras/Shoot1", 0.15f),
																	("Enemies/Papyras/Shoot2", 0.15f));

			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Enemies/Papyras/Idle1");

			mIdleAnim.Play();

			// Hack
			switch(GetGravityDir())
			{
				case CardinalDirection.Left:
					mPosition += new Vector2(-2.0f, 1.0f);
					break;
				case CardinalDirection.Right:
					mPosition += new Vector2(-1.0f, 1.0f);
					break;
				case CardinalDirection.Up:
					mPosition.Y -= 1.0f;
					break;
			}
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update papyras
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			EntityManager.I.AddColliderSubmission(new EntityColliderSubmission(this));

			mVelocity = Vector2.Zero;
			base.Update(gameTime);
		}



		/// <summary>
		/// Do physics
		/// </summary>
		public override void OrderedUpdate(GameTime gameTime)
		{
			// Force grounded and no physics
			mOnGround = true;
		}



		/// <summary>
		/// Shoot gun.
		/// </summary>
		protected override void SpawnBullet()
		{
			CardinalDirection bulletDirection = Util.WalkDirectionToCardinal(mPrevDirection, GetGravityDir());
			Vector2 offset = BULLET_OFFSET * Util.GetNormal(GetGravityDir());
			Vector2 spawnPos = GetCentrePos() + Util.GetNormal(bulletDirection) * 11.0f + offset;
			Fireball bullet = new Fireball(this, spawnPos, bulletDirection);
			bullet.SetGravity(GetGravityDir());
			EntityManager.I.QueueRegisterEntity(bullet);
		}



		/// <summary>
		/// Get collider bounds
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			const float WIDTH_REDUCTION = 2.0f;
			const float HEIGHT_REDUCTION = 1.0f;

			return GetReducedTextureCollider(WIDTH_REDUCTION, HEIGHT_REDUCTION);
		}

		#endregion rUpdate
	}
}
