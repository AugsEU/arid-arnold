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
			WalkAround,
			ShootLaser,
			Jump
		}

		#endregion rTypes





		#region rConstants

		const float RANGER_WIDTH_REDUCTION = 6.0f;
		const float RANGER_HEIGHT_REDUCTION = 3.0f;
		const float RANGER_JUMP_SPEED = 22.0f;
		const float RANGER_WALK_SPEED = 4.5f;

		#endregion rConstants





		#region rMembers

		Animator mShootGunAnim;
		StateMachine<State> mStateMachine;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Spawn ranger at specific time
		/// </summary>
		/// <param name="pos">Spawn pos</param>
		/// <param name="shootPhase">How far in the shoot cycle to start</param>
		/// <param name="shootFreq">How long the shoot cycle is</param>
		public Ranger(Vector2 pos) : base(pos, RANGER_WALK_SPEED, RANGER_JUMP_SPEED, RANGER_WIDTH_REDUCTION, RANGER_HEIGHT_REDUCTION)
		{
			mStateMachine = new StateMachine<State>(State.WalkAround);
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
												("Enemies/Ranger/Run1", 0.2f),
												("Enemies/Ranger/Run2", 0.2f),
												("Enemies/Ranger/Run3", 0.2f),
												("Enemies/Ranger/Run4", 0.2f));
			mRunningAnimation.Play();

			mStandAnimation = new Animator(Animator.PlayType.Repeat,
											("Enemies/Ranger/Idle1", 0.3f),
											("Enemies/Ranger/Idle2", 0.3f),
											("Enemies/Ranger/Idle3", 0.3f),
											("Enemies/Ranger/Idle2", 0.3f));
			mStandAnimation.Play();

			mShootGunAnim = new Animator(Animator.PlayType.OneShot,
											("Enemies/Ranger/Charge1", 0.2f),
											("Enemies/Ranger/Charge1", 0.2f),
											("Enemies/Ranger/Charge1", 0.2f));

			mPosition.Y -= 2.0f;
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update ranger
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mShootGunAnim.Update(gameTime);

			base.Update(gameTime);
		}



		/// <summary>
		/// Decide what to do
		/// </summary>
		protected override void DecideActions()
		{
			if (mOnGround)
			{
				mStateMachine.ForceGoToStateAndWait(State.WalkAround, 3500.0f);
			}

			EnforceState();
		}



		/// <summary>
		/// Do things
		/// </summary>
		void EnforceState()
		{

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
