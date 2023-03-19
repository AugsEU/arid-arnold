namespace AridArnold
{
	abstract class ShooterEnemy : PlatformingEntity
	{
		#region rTypes

		enum State
		{
			Idle,
			ChargeGun,
		}

		#endregion rTypes



		#region rMembers

		protected Animator mIdleAnim;
		protected Animator mChargeGunAnim;
		protected Animator mShootGunAnim;
		protected PercentageTimer mShootTimer;
		StateMachine<State> mStateMachine;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Init futron-gun at position.
		/// </summary>
		public ShooterEnemy(Vector2 pos, float shootPhase, float shootFreq) : base(pos, 0.0f, 0.0f)
		{
			shootFreq *= 1000.0f;
			mShootTimer = new PercentageTimer(shootFreq);
			mShootTimer.SetPercentTime(-shootPhase);
			mShootTimer.Start();
			mStateMachine = new StateMachine<State>(State.Idle);
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
					if (mChargeGunAnim.IsPlaying() == false)
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
			SpawnBullet();

			mShootGunAnim.Play();

			mStateMachine.SetState(State.Idle);
		}



		/// <summary>
		/// Spawn projectile.
		/// </summary>
		protected abstract void SpawnBullet();

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get texture we should draw.
		/// </summary>
		protected override Texture2D GetDrawTexture()
		{
			if (mShootGunAnim.IsPlaying())
			{
				return mShootGunAnim.GetCurrentTexture();
			}
			else if (mChargeGunAnim.IsPlaying())
			{
				return mChargeGunAnim.GetCurrentTexture();
			}

			return mIdleAnim.GetCurrentTexture();
		}

		#endregion rDraw
	}
}
