namespace AridArnold
{
	internal class FutronGun : ShooterEnemy
	{
		#region rConstants

		static float BULLET_OFFSET = 2.0f;

		#endregion rConstants





		#region rInitialisation

		/// <summary>
		/// Init futron-gun at position.
		/// </summary>
		public FutronGun(Vector2 pos, float shootPhase, float shootFreq) : base(pos, shootPhase, shootFreq)
		{
			mPosition.Y += 2.0f;
		}



		/// <summary>
		/// Load textures for futron-gun
		/// </summary>
		public override void LoadContent()
		{
			const float FT = 0.2f;
			mIdleAnim = new Animator(Animator.PlayType.Repeat, ("Enemies/Futron-Gun/Idle1", FT),
															   ("Enemies/Futron-Gun/Idle2", FT));

			mChargeGunAnim = new Animator(Animator.PlayType.OneShot, ("Enemies/Futron-Gun/Charge1", FT),
																	 ("Enemies/Futron-Gun/Charge2", FT),
																	 ("Enemies/Futron-Gun/Charge3", FT),
																	 ("Enemies/Futron-Gun/Charge4", FT),
																	 ("Enemies/Futron-Gun/Charge5", FT),
																	 ("Enemies/Futron-Gun/Charge6", FT),
																	 ("Enemies/Futron-Gun/Charge5", FT),
																	 ("Enemies/Futron-Gun/Charge6", FT));

			mShootGunAnim = new Animator(Animator.PlayType.OneShot, ("Enemies/Futron-Gun/Shoot1", 0.15f),
																	("Enemies/Futron-Gun/Shoot2", 0.15f),
																	("Enemies/Futron-Gun/Shoot3", 0.15f),
																	("Enemies/Futron-Gun/Shoot4", 0.15f));

			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Enemies/Futron-Gun/Idle1");

			mIdleAnim.Play();
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Shoot gun.
		/// </summary>
		protected override void SpawnBullet()
		{
			CardinalDirection bulletDirection = Util.WalkDirectionToCardinal(mPrevDirection, GetGravityDir());
			Vector2 offset = BULLET_OFFSET * Util.GetNormal(GetGravityDir());
			Vector2 spawnPos = GetCentrePos() + Util.GetNormal(bulletDirection) * 12.0f + offset;
			LaserBullet bullet = new LaserBullet(this, spawnPos, bulletDirection);
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
