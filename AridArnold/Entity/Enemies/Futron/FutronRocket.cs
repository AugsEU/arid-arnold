namespace AridArnold
{
	internal class FutronRocket : ShooterEnemy
	{
		#region rConstants

		static Vector2 BULLET_OFFSET = new Vector2(0.0f, 8.0f);

		#endregion rConstants





		#region rMembers

		//Texture2D mBulletTexture;
		float mLaunchSpeed;
		float mLaunchAngle;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Init futron-gun at position.
		/// </summary>
		public FutronRocket(Vector2 pos, float shootPhase, float shootFreq, float launchSpeed, float launchAngle) : base(pos, shootPhase, shootFreq)
		{
			mLaunchSpeed = launchSpeed;
			mLaunchAngle = launchAngle;
		}



		/// <summary>
		/// Load textures for futron-gun
		/// </summary>
		public override void LoadContent(ContentManager content)
		{
			const float FT = 0.2f;
			mIdleAnim = new Animator(content, Animator.PlayType.Repeat, ("Enemies/Futron-Rocket/Idle1", FT),
																		("Enemies/Futron-Rocket/Idle2", FT));

			mChargeGunAnim = new Animator(content, Animator.PlayType.OneShot, ("Enemies/Futron-Rocket/Charge1", FT),
																			  ("Enemies/Futron-Rocket/Charge2", FT),
																			  ("Enemies/Futron-Rocket/Charge3", FT),
																			  ("Enemies/Futron-Rocket/Charge4", FT),
																			  ("Enemies/Futron-Rocket/Charge5", FT),
																			  ("Enemies/Futron-Rocket/Charge6", FT),
																			  ("Enemies/Futron-Rocket/Charge5", FT),
																			  ("Enemies/Futron-Rocket/Charge6", FT));

			mShootGunAnim = new Animator(content, Animator.PlayType.OneShot, ("Enemies/Futron-Rocket/Shoot1", 0.15f),
																			 ("Enemies/Futron-Rocket/Shoot2", 0.15f),
																			 ("Enemies/Futron-Rocket/Shoot3", 0.15f),
																			 ("Enemies/Futron-Rocket/Shoot4", 0.15f));

			mTexture = content.Load<Texture2D>("Enemies/Futron-Rocket/Idle1");
			//mBulletTexture = content.Load<Texture2D>("Enemies/Futron-Gun/bullet");

			mIdleAnim.Play();
			mShootTimer.Start();
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Shoot gun.
		/// </summary>
		protected override void SpawnBullet()
		{
			//CardinalDirection bulletDirection = Util.WalkDirectionToCardinal(mPrevDirection, GetGravityDir());
			//Vector2 spawnPos = mPosition + Util.GetNormal(bulletDirection) * 12.0f + BULLET_OFFSET;
			//LaserBullet bullet = new LaserBullet(spawnPos, bulletDirection, mBulletTexture);
			//EntityManager.I.QueueRegisterEntity(bullet);
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
