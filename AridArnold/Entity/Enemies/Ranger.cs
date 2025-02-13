﻿namespace AridArnold
{
	internal class Ranger : AIEntity
	{
		#region rTypes

		enum State
		{
			Wait,
			WalkRight,
			WalkLeft,
			Jump,
			ShootLaser,
		}

		#endregion rTypes





		#region rConstants

		const float RANGER_WIDTH_REDUCTION = 6.0f;
		const float RANGER_HEIGHT_REDUCTION = 3.0f;
		const float RANGER_JUMP_SPEED = 25.0f;
		const float RANGER_WALK_SPEED = 4.5f;
		static float BULLET_OFFSET = -6.0f;

		#endregion rConstants





		#region rMembers

		Animator mShootGunAnim;
		StateMachine<State> mStateMachine;
		PercentageTimer mShootTimer;
		Vector2 mHoleToJumpOver = Vector2.Zero;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Spawn ranger at specific time
		/// </summary>
		/// <param name="pos">Spawn pos</param>
		/// <param name="shootPhase">How far in the shoot cycle to start</param>
		/// <param name="shootFreq">How long the shoot cycle is</param>
		public Ranger(Vector2 pos, float shootPhase, float shootFreq) : base(pos, RANGER_WALK_SPEED, RANGER_JUMP_SPEED, RANGER_WIDTH_REDUCTION, RANGER_HEIGHT_REDUCTION)
		{
			shootFreq *= 1000.0f;
			mStateMachine = new StateMachine<State>(State.Wait);
			mShootTimer = new PercentageTimer(shootFreq);
			mShootTimer.SetPercentTime(-shootPhase);
		}



		/// <summary>
		/// Load ranger content
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Enemies/Ranger/Idle1");
			mJumpUpTex = MonoData.I.MonoGameLoad<Texture2D>("Enemies/Ranger/JumpUp");
			mJumpDownTex = MonoData.I.MonoGameLoad<Texture2D>("Enemies/Ranger/JumpDown");

			mRunningAnimation = new Animator(Animator.PlayType.Repeat,
												("Enemies/Ranger/Run1", 0.1f),
												("Enemies/Ranger/Run2", 0.1f),
												("Enemies/Ranger/Run3", 0.1f),
												("Enemies/Ranger/Run4", 0.1f));
			mRunningAnimation.Play();

			mStandAnimation = new Animator(Animator.PlayType.Repeat,
											("Enemies/Ranger/Idle1", 0.3f),
											("Enemies/Ranger/Idle2", 0.2f),
											("Enemies/Ranger/Idle3", 0.25f),
											("Enemies/Ranger/Idle2", 0.2f));
			mStandAnimation.Play();

			mShootGunAnim = new Animator(Animator.PlayType.OneShot,
											("Enemies/Ranger/Charge1", 0.2f),
											("Enemies/Ranger/Charge2", 0.2f),
											("Enemies/Ranger/Charge3", 0.2f));

			mPosition.Y += 3.0f;
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update ranger
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mStateMachine.Update(gameTime);

			mShootGunAnim.Update(gameTime);
			mShootTimer.Start();
			mShootTimer.Update(gameTime);

			base.Update(gameTime);
		}



		/// <summary>
		/// Decide what to do
		/// </summary>
		protected override void DecideActions()
		{
			bool jumping = mStateMachine.GetState() == State.Jump;
			bool shooting = mStateMachine.GetState() == State.ShootLaser;

			if (CanWalkDirChange() && !shooting)
			{
				if (ShouldShootLaser() && !jumping)
				{
					mStateMachine.SetState(State.ShootLaser);
					mShootGunAnim.Play();
				}
				else if (CanKeepWalking() && !jumping)
				{
					State walkState = GetPrevWalkDirection() == WalkDirection.Right ? State.WalkRight : State.WalkLeft;
					mStateMachine.SetState(walkState);
				}
				else if (CanJump())
				{
					mStateMachine.SetState(State.Jump);
				}
				else if (CanTurnAround())
				{
					SetPrevWalkDirection(GetPrevWalkDirection() == WalkDirection.Right ? WalkDirection.Left : WalkDirection.Right);
					mStateMachine.GoToStateAndWait(State.Wait, 100.0);
				}
				else
				{
					mStateMachine.GoToStateAndWait(State.Wait, 500.0);
				}
			}

			EnforceState();
		}



		/// <summary>
		/// Decide if we should shoot a laser
		/// </summary>
		bool ShouldShootLaser()
		{
			bool solidInFront = false;
			switch (GetPrevWalkDirection())
			{
				case WalkDirection.Left:
					solidInFront = CheckSolid(-1, 0);
					break;
				case WalkDirection.Right:
					solidInFront = CheckSolid(1, 0);
					break;
			}
			return mShootTimer.GetPercentageF() >= 1.0f && !solidInFront;
		}



		/// <summary>
		/// Can we keep walking in our direction
		/// </summary>
		bool CanKeepWalking()
		{
			int dx = GetPrevWalkDirection() == WalkDirection.Left ? -1 : 1;

			return (CheckSolid(dx, 1) || CheckSolid(dx, 2)) && !CheckSolid(dx, 0);
		}



		/// <summary>
		/// Can we jump?
		/// </summary>
		bool CanJump()
		{
			int dx = 0;
			if (GetPrevWalkDirection() == WalkDirection.Left)
			{
				dx = -1;
			}
			else
			{
				dx = 1;
			}

			bool aboveClear = !CheckSolid(0, -1) && !CheckSolid(0, -2);

			return aboveClear && (CanJumpOverHole(dx) || CanJumpUpStep(dx));
		}


		/// <summary>
		/// Can we jump over a hole in-front of us?
		/// </summary>
		bool CanJumpOverHole(int dx)
		{
			bool jumpZoneClear = !CheckSolid(dx, 1) && !CheckSolid(dx, 0) && !CheckSolid(dx, -1);
			bool landingZoneClear = CheckSolid(dx * 2, 1) && !CheckSolid(dx * 2, 0) && !CheckSolid(dx * 2, -1);

			Tile holeTile = GetNearbyTile(dx, 1);
			Vector2 holeCentre = holeTile.GetCentre();

			mHoleToJumpOver = holeCentre;

			return jumpZoneClear && landingZoneClear;
		}



		/// <summary>
		/// Can we jump up a step in-front of us?
		/// </summary>
		bool CanJumpUpStep(int dx)
		{
			bool landingZoneClear = CheckSolid(dx, 0) && !CheckSolid(dx, -1) && !CheckSolid(dx, -2);

			Tile holeTile = GetNearbyTile(dx, 0);
			Vector2 holeCentre = holeTile.GetCentre();

			mHoleToJumpOver = holeCentre;

			return landingZoneClear;
		}



		/// <summary>
		/// Check if we can turn around
		/// </summary>
		bool CanTurnAround()
		{
			if (GetPrevWalkDirection() == WalkDirection.Left)
			{
				return CheckSolid(1, 1) && !CheckSolid(1, 0);
			}
			else
			{
				return CheckSolid(-1, 1) && !CheckSolid(-1, 0);
			}

			throw new NotImplementedException();
		}



		/// <summary>
		/// Do things
		/// </summary>
		void EnforceState()
		{
			switch (mStateMachine.GetState())
			{
				case State.Wait:
					if (CanWalkDirChange()) mWalkDirection = WalkDirection.None;
					break;
				case State.WalkRight:
					if (CanWalkDirChange()) mWalkDirection = WalkDirection.Right;
					break;
				case State.WalkLeft:
					if (CanWalkDirChange()) mWalkDirection = WalkDirection.Left;
					break;
				case State.Jump:
					mWalkDirection = mPrevDirection;
					if (mOnGround && (GetCentrePos() - mHoleToJumpOver).Length() < 17.0f)
					{
						Jump();
						mStateMachine.ForceGoToStateAndWait(State.Wait, 800.0f);
					}
					break;
				case State.ShootLaser:
					if (mOnGround) mWalkDirection = WalkDirection.None;
					if (!mShootGunAnim.IsPlaying())
					{
						ShootBullet();
					}
					break;
				default:
					break;
			}
		}


		/// <summary>
		/// 
		/// </summary>
		void ShootBullet()
		{
			CardinalDirection bulletDirection = Util.WalkDirectionToCardinal(mPrevDirection, GetGravityDir());
			Vector2 offset = BULLET_OFFSET * Util.GetNormal(GetGravityDir());
			Vector2 spawnPos = GetCentrePos() + Util.GetNormal(bulletDirection) * 4.0f + offset;
			if (mPrevDirection == WalkDirection.Left)
			{
				spawnPos += Util.GetNormal(bulletDirection) * 12.0f;
			}
			LaserBullet bullet = new LaserBullet(this, spawnPos, bulletDirection);
			EntityManager.I.QueueRegisterEntity(bullet);

			mStateMachine.ForceGoToStateAndWait(State.Wait, 900.0f);
			mShootTimer.Reset();
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get texture we should draw.
		/// </summary>
		/// <returns></returns>
		public override Texture2D GetDrawTexture()
		{
			if (mStateMachine.GetState() == State.ShootLaser)
			{
				return mShootGunAnim.GetCurrentTexture();
			}

			return base.GetDrawTexture();
		}

		#endregion rDraw
	}
}
