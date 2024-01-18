using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AridArnold
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
			mShootGunAnim.Update(gameTime);
			mShootTimer.Start();

			base.Update(gameTime);
		}



		/// <summary>
		/// Decide what to do
		/// </summary>
		protected override void DecideActions()
		{
			bool jumping = mStateMachine.GetState() == State.Jump;
			bool shooting = mStateMachine.GetState() == State.ShootLaser;

			if (mOnGround && !shooting)
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
			return mShootTimer.GetPercentageF() >= 1.0f;
		}



		/// <summary>
		/// Can we keep walking in our direction
		/// </summary>
		bool CanKeepWalking()
		{
			if (GetPrevWalkDirection() == WalkDirection.Left)
			{
				return CheckSolid(-1, 1) && !CheckSolid(-1, 0);
			}
			else
			{
				return CheckSolid(1, 1) && !CheckSolid(1, 0);
			}

			throw new NotImplementedException();
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

			bool jumpZoneClear = !CheckSolid(dx, 1) && !CheckSolid(dx, 0) && !CheckSolid(dx, -1);
			bool landingZoneClear = CheckSolid(dx * 2, 1) && !CheckSolid(dx * 2, 0) && !CheckSolid(dx * 2, -1);

			Tile holeTile = GetNearbyTile(dx, 1);
			Vector2 holeCentre = holeTile.GetCentre();

			mHoleToJumpOver = holeCentre;

			return jumpZoneClear && landingZoneClear;
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
					if (mOnGround) mWalkDirection = WalkDirection.None;
					break;
				case State.WalkRight:
					if (mOnGround) mWalkDirection = WalkDirection.Right;
					break;
				case State.WalkLeft:
					if (mOnGround) mWalkDirection = WalkDirection.Left;
					break;
				case State.Jump:
					if (mOnGround && (GetCentrePos() - mHoleToJumpOver).Length() < 15.0f)
					{
						Jump();
						mStateMachine.ForceGoToStateAndWait(State.Wait, 500.0f);
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
			LaserBullet bullet = new LaserBullet(this, spawnPos, bulletDirection);
			EntityManager.I.QueueRegisterEntity(bullet);

			mStandAnimation.Play(); // Visual hack to make it look like recoil
			mStateMachine.ForceGoToStateAndWait(State.Wait, 400.0f);
			mShootTimer.Reset();
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get texture we should draw.
		/// </summary>
		/// <returns></returns>
		protected override Texture2D GetDrawTexture()
		{
			if(mStateMachine.GetState() == State.ShootLaser)
			{
				return mShootGunAnim.GetCurrentTexture();
			}

			return base.GetDrawTexture();
		}

		#endregion rDraw
	}
}
