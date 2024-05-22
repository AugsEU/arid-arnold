namespace GMTK2023
{
	#region rTypes

	public enum AITeam
	{
		Ally,
		Enemy
	}

	#endregion rTypes

	internal class AIEntity : Motorbike
	{
		#region rConstants

		const float MAX_SPEED = 20.0f;
		const float TARGET_REACHED_DIST = 40.0f;
		const double MIN_DIRECTION_CHANGE_TIME = 300.0;
		const double DIRECT_SHOT_TIMEOUT = 1000.0;

		const double DEATH_FLASH_TIME = 800.0;

		#endregion rConstants


		#region rMembers

		Animator[][] mTeamSkins;
		AITeam mCurrentTeam;
		Vector2 mCurrentTarget;

		bool mStopped;
		MonoTimer mStopTimer;
		float mStopDuration;

		EightDirection mPrevDirection;
		PercentageTimer mDirChangeTimer;

		MonoTimer mShootBulletTimer;
		float mShootDuration;

		PercentageTimer mDirectShotTimer;
		PercentageTimer mDeadTimer;

		bool mIsDead = false;

		bool mBeingGrappled = false;
		bool mIsUzi = false;

		Texture2D mPentagram;

		#endregion rMembers


		#region rInit

		public AIEntity(Vector2 pos, float angle) : base(pos, angle, MAX_SPEED)
		{
			mCurrentTarget = Vector2.Zero;
			mStopped = false;
			mStopTimer = new MonoTimer();
			mStopTimer.Start();
			mStopDuration = 0.0f;
			mPrevDirection = GetCurrentDir();
			mDirChangeTimer = new PercentageTimer(MIN_DIRECTION_CHANGE_TIME);
			mDirChangeTimer.Start();

			mShootBulletTimer = new MonoTimer();
			mShootBulletTimer.Start();
			mShootDuration = RandomManager.I.GetWorld().GetFloatRange(3500.0f, 12000.0f);

			mDirectShotTimer = new PercentageTimer(DIRECT_SHOT_TIMEOUT + RandomManager.I.GetWorld().GetFloatRange(200.0f, 800.0f));
			mDirectShotTimer.Start();

			mDeadTimer = new PercentageTimer(DEATH_FLASH_TIME);

			if (RunManager.I.GetRounds() > 3)
			{
				mIsUzi = RandomManager.I.GetWorld().PercentChance(30.0f);
			}
		}

		public override void LoadContent()
		{
			mTeamSkins = new Animator[2][];
			mTeamSkins[(int)AITeam.Ally] = new Animator[8];
			mTeamSkins[(int)AITeam.Ally][(int)EightDirection.Up] = MonoData.I.LoadAnimator("Enemies/AllyUp");
			mTeamSkins[(int)AITeam.Ally][(int)EightDirection.UpLeft] = MonoData.I.LoadAnimator("Enemies/AllyUpLeft");
			mTeamSkins[(int)AITeam.Ally][(int)EightDirection.Left] = MonoData.I.LoadAnimator("Enemies/AllyLeft");
			mTeamSkins[(int)AITeam.Ally][(int)EightDirection.DownLeft] = MonoData.I.LoadAnimator("Enemies/AllyDownLeft");
			mTeamSkins[(int)AITeam.Ally][(int)EightDirection.Down] = MonoData.I.LoadAnimator("Enemies/AllyDown");
			mTeamSkins[(int)AITeam.Ally][(int)EightDirection.DownRight] = MonoData.I.LoadAnimator("Enemies/AllyDownRight");
			mTeamSkins[(int)AITeam.Ally][(int)EightDirection.Right] = MonoData.I.LoadAnimator("Enemies/AllyRight");
			mTeamSkins[(int)AITeam.Ally][(int)EightDirection.UpRight] = MonoData.I.LoadAnimator("Enemies/AllyUpRight");

			if (mIsUzi)
			{
				mTeamSkins[(int)AITeam.Enemy] = new Animator[8];
				mTeamSkins[(int)AITeam.Enemy][(int)EightDirection.Up] = MonoData.I.LoadAnimator("Enemies/EnemyUziUp");
				mTeamSkins[(int)AITeam.Enemy][(int)EightDirection.UpLeft] = MonoData.I.LoadAnimator("Enemies/EnemyUziUpLeft");
				mTeamSkins[(int)AITeam.Enemy][(int)EightDirection.Left] = MonoData.I.LoadAnimator("Enemies/EnemyUziLeft");
				mTeamSkins[(int)AITeam.Enemy][(int)EightDirection.DownLeft] = MonoData.I.LoadAnimator("Enemies/EnemyUziDownLeft");
				mTeamSkins[(int)AITeam.Enemy][(int)EightDirection.Down] = MonoData.I.LoadAnimator("Enemies/EnemyUziDown");
				mTeamSkins[(int)AITeam.Enemy][(int)EightDirection.DownRight] = MonoData.I.LoadAnimator("Enemies/EnemyUziDownRight");
				mTeamSkins[(int)AITeam.Enemy][(int)EightDirection.Right] = MonoData.I.LoadAnimator("Enemies/EnemyUziRight");
				mTeamSkins[(int)AITeam.Enemy][(int)EightDirection.UpRight] = MonoData.I.LoadAnimator("Enemies/EnemyUziUpRight");
			}
			else
			{
				mTeamSkins[(int)AITeam.Enemy] = new Animator[8];
				mTeamSkins[(int)AITeam.Enemy][(int)EightDirection.Up] = MonoData.I.LoadAnimator("Enemies/EnemyUp");
				mTeamSkins[(int)AITeam.Enemy][(int)EightDirection.UpLeft] = MonoData.I.LoadAnimator("Enemies/EnemyUpLeft");
				mTeamSkins[(int)AITeam.Enemy][(int)EightDirection.Left] = MonoData.I.LoadAnimator("Enemies/EnemyLeft");
				mTeamSkins[(int)AITeam.Enemy][(int)EightDirection.DownLeft] = MonoData.I.LoadAnimator("Enemies/EnemyDownLeft");
				mTeamSkins[(int)AITeam.Enemy][(int)EightDirection.Down] = MonoData.I.LoadAnimator("Enemies/EnemyDown");
				mTeamSkins[(int)AITeam.Enemy][(int)EightDirection.DownRight] = MonoData.I.LoadAnimator("Enemies/EnemyDownRight");
				mTeamSkins[(int)AITeam.Enemy][(int)EightDirection.Right] = MonoData.I.LoadAnimator("Enemies/EnemyRight");
				mTeamSkins[(int)AITeam.Enemy][(int)EightDirection.UpRight] = MonoData.I.LoadAnimator("Enemies/EnemyUpRight");
			}

			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Enemies/AllyUp");

			mPentagram = MonoData.I.MonoGameLoad<Texture2D>("Effects/pent");

			SetTeam(AITeam.Enemy);
		}

		#endregion rInit





		#region rUpdate

		public override void Update(GameTime gameTime)
		{
			if (mDeadTimer.IsPlaying())
			{
				if (mDeadTimer.GetPercentageF() >= 1.0f)
				{
					EntityManager.I.QueueDeleteEntity(this);
				}
				return;
			}

			if (mCurrentTarget == Vector2.Zero)
			{
				GetNewTarget();
			}

			if (mBeingGrappled)
			{
				mSpeed = Math.Max(0.0f, mMaxSpeed * 0.5f);
			}

			ConsiderStop();

			GoToTarget(gameTime);
			SetAcelerate(!mStopped);

			if (mShootBulletTimer.GetElapsedMs() > mShootDuration)
			{
				ShootBullet();
				mShootBulletTimer.Reset();

				if (mIsUzi)
				{
					mShootDuration = RandomManager.I.GetWorld().GetFloatRange(500.0f, 3000.0f);
				}
				else
				{
					mShootDuration = RandomManager.I.GetWorld().GetFloatRange(1500.0f, 5000.0f);
				}
			}

			if (mDirectShotTimer.GetPercentageF() >= 1.0f && CanTakeDirectShot() && mCurrentTeam == AITeam.Enemy)
			{
				ShootBullet();
				mDirectShotTimer.Reset();
				mShootBulletTimer.Reset();
			}

			base.Update(gameTime);
		}

		bool CanTakeDirectShot()
		{
			List<Entity> playerList = EntityManager.I.GetAllOfType(typeof(Player));

			if (playerList.Count == 0)
			{
				return false;
			}

			Player player = (Player)playerList[0];

			Vector2 toPlayer = player.GetCentrePos() - mCentreOfMass;
			float distToPlayer = toPlayer.Length();
			Vector2 directShotPos = player.GetCentrePos() + Vector2.Normalize(player.GetVelocity()) * 50.25f;

			float toPlayerAngle = MathF.Atan2(toPlayer.Y, toPlayer.X);
			float angleDiff = MonoMath.GetAngleDiff(GetCurrentAngle(), toPlayerAngle);

			if (Collision2D.BoxVsPoint(new Rect2f(GameScreen.PLAYABLE_AREA), directShotPos) && distToPlayer < 500.0f && angleDiff < MathF.PI * 0.25f)
			{
				return true;
			}

			return false;
		}

		void GoToTarget(GameTime gameTime)
		{
			if ((mCentreOfMass - mCurrentTarget).LengthSquared() < TARGET_REACHED_DIST * TARGET_REACHED_DIST)
			{
				GetNewTarget();
			}

			Vector2 toTarget = mCurrentTarget - mCentreOfMass;
			float angle = -MathF.Atan2(toTarget.Y, toTarget.X);
			EightDirection dir = Util.GetDirectionFromAngle(angle);

			if (dir != mPrevDirection && mDirChangeTimer.GetPercentageF() >= 1.0f)
			{
				TargetDirection(dir);
				mPrevDirection = dir;
				mDirChangeTimer.Reset();
				mDirChangeTimer.Start();
			}
		}

		void ConsiderStop()
		{
			if (mStopDuration == 0.0f)
			{
				if (mStopped)
				{
					mStopDuration = RandomManager.I.GetWorld().GetFloatRange(1500.0f, 2500.0f);
				}
				else
				{
					mStopDuration = RandomManager.I.GetWorld().GetFloatRange(10500.0f, 25500.0f);
				}
			}

			if (mStopTimer.GetElapsedMs() > mStopDuration)
			{
				mStopped = !mStopped;
				mStopDuration = 0.0f;
				mStopTimer.Reset();
			}
		}


		void ShootBullet()
		{
			if (mBeingGrappled)
			{
				return;
			}

			EntityManager.I.QueueRegisterEntity(new Bullet(mPosition, GetCurrentDir(), GetTeam()));
		}

		public override void Kill()
		{
			mIsDead = true;
			mDeadTimer.Start();
		}

		#endregion rUpdate

		#region rDraw

		public override void Draw(DrawInfo info)
		{
			if (mDeadTimer.IsPlaying())
			{
				float flash = mDeadTimer.GetPercentageF() % 0.15f;
				if (flash > 0.075f)
				{
					return;
				}
			}

			if (mBeingGrappled)
			{
				MonoDraw.DrawTextureDepth(info, mPentagram, mCentreOfMass - new Vector2(mPentagram.Width, mPentagram.Height) * 0.5f, DrawLayer.SubEntity);
			}

			base.Draw(info);
		}


		#endregion rDraw

		#region rUtil

		public void SetTeam(AITeam aiTeam)
		{
			if (aiTeam == AITeam.Ally && mCurrentTeam == AITeam.Enemy)
			{
				SoundManager.I.PlaySFX(SoundManager.SFXType.Convert, 0.7f);
			}

			mCurrentTeam = aiTeam;
			mDirectionTextures = mTeamSkins[(int)aiTeam];
		}

		public AITeam GetTeam()
		{
			return mCurrentTeam;
		}

		void GetNewTarget()
		{
			if (mCurrentTarget != Vector2.Zero)
			{
				AITargetManager.I.ReportReachedPoint(mCurrentTarget);
			}

			mCurrentTarget = AITargetManager.I.GiveMeATarget();

			if (RandomManager.I.GetWorld().PercentChance(10.0f))
			{
				mStopDuration = RandomManager.I.GetWorld().GetFloatRange(1500.0f, 2500.0f);
				mStopped = true;
				mStopTimer.Reset();
			}
		}

		public bool IsDead()
		{
			return mIsDead;
		}

		public void SetBeingGrappled(bool grap)
		{
			mBeingGrappled = grap;
		}

		#endregion rUtil
	}
}
