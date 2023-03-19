namespace AridArnold
{
	internal class FutronGun : PlatformingEntity
	{
		#region rTypes

		enum State
		{
			Idle,
			ChargeGun,
		}

		#endregion rTypes





		#region rConstants

		static Vector2 BULLET_OFFSET = new Vector2(0.0f, 8.0f);

		#endregion rConstants





		#region rMembers

		Animator mIdleAnim;
		Animator mChargeGunAnim;
		Animator mShootGunAnim;
		Texture2D mBulletTexture;
		PercentageTimer mShootTimer;
		StateMachine<State> mStateMachine;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Init futron-gun at position.
		/// </summary>
		public FutronGun(Vector2 pos, float shootPhase, float shootFreq) : base(pos, 0.0f, 0.0f)
		{
			shootFreq *= 1000.0f;
			mShootTimer = new PercentageTimer(shootFreq);
			mShootTimer.SetPercentTime(-shootPhase);

			mStateMachine = new StateMachine<State>(State.Idle);
		}



		/// <summary>
		/// Load textures for futron-gun
		/// </summary>
		public override void LoadContent(ContentManager content)
		{
			const float FT = 0.2f;
			mIdleAnim = new Animator(content, Animator.PlayType.Repeat, ("Enemies/Futron-Gun/Idle1", FT),
																		("Enemies/Futron-Gun/Idle2", FT));

			mChargeGunAnim = new Animator(content, Animator.PlayType.OneShot, ("Enemies/Futron-Gun/Charge1", FT),
																			  ("Enemies/Futron-Gun/Charge2", FT),
																			  ("Enemies/Futron-Gun/Charge3", FT),
																			  ("Enemies/Futron-Gun/Charge4", FT),
																			  ("Enemies/Futron-Gun/Charge5", FT),
																			  ("Enemies/Futron-Gun/Charge6", FT),
																			  ("Enemies/Futron-Gun/Charge5", FT),
																			  ("Enemies/Futron-Gun/Charge6", FT));

			mShootGunAnim = new Animator(content, Animator.PlayType.OneShot, ("Enemies/Futron-Gun/Shoot1", 0.15f),
																			 ("Enemies/Futron-Gun/Shoot2", 0.15f),
																			 ("Enemies/Futron-Gun/Shoot3", 0.15f),
																			 ("Enemies/Futron-Gun/Shoot4", 0.15f));

			mTexture = content.Load<Texture2D>("Enemies/Futron-Gun/Idle1");
			mBulletTexture = content.Load<Texture2D>("Enemies/Futron-Gun/bullet");

			mIdleAnim.Play();
			mShootTimer.Start();
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			switch (mStateMachine.GetState())
			{
				case State.Idle:
					if (mShootTimer.GetPercentageF() >= 1.0f)
					{
						ChargeGun();
					}
					break;
				case State.ChargeGun:
					// Wait for charge anim and then shoot
					if(mChargeGunAnim.IsPlaying() == false)
					{
						mShootTimer.Reset();
						ShootGun();
					}
					break;
			}

			mIdleAnim.Update(gameTime);
			mChargeGunAnim.Update(gameTime);
			mShootGunAnim.Update(gameTime);

			base.Update(gameTime);
		}
		


		/// <summary>
		/// Charge gun
		/// </summary>
		private void ChargeGun()
		{
			mChargeGunAnim.Play();
			mStateMachine.SetState(State.ChargeGun);
		}



		/// <summary>
		/// Shoot gun.
		/// </summary>
		private void ShootGun()
		{
			// Spawn bullet
			CardinalDirection bulletDirection = Util.WalkDirectionToCardinal(mPrevDirection, GetGravityDir());
			Vector2 spawnPos = mPosition + Util.GetNormal(bulletDirection) * 12.0f + BULLET_OFFSET;
			LaserBullet bullet = new LaserBullet(spawnPos, bulletDirection, mBulletTexture);
			EntityManager.I.QueueRegisterEntity(bullet);

			mShootGunAnim.Play();

			mStateMachine.SetState(State.Idle);
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





		#region rDraw

		/// <summary>
		/// Get texture we should draw.
		/// </summary>
		protected override Texture2D GetDrawTexture()
		{
			if(mShootGunAnim.IsPlaying())
			{
				return mShootGunAnim.GetCurrentTexture();
			}
			else if(mChargeGunAnim.IsPlaying())
			{
				return mChargeGunAnim.GetCurrentTexture();
			}

			return mIdleAnim.GetCurrentTexture();
		}

		#endregion rDraw
	}
}
