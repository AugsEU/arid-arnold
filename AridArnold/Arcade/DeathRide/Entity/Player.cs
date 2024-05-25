using System;

namespace DeathRide
{
	internal class Player : Motorbike
	{
		#region rConstants
		protected const float MAX_SPEED = 30.0f;
		const float GRAPPLE_EXTEND_SPEED = 120.0f;
		const float GRAPPLE_MAX_LENGTH = 125.0f;
		const float GRAPPLE_LOCK_DISTANCE = 25.0f;
		const float GRAPPLE_RADIAL_SPIN_SPEED = MAX_SPEED + 20.0f;
		const float GRAPPLE_RADIAL_SPIN_START_ANGLE = 0.9f;
		const float GRAPPLE_RADIAL_SPIN_MIN_RADIUS = 25.0f;
		const float GRAPPLE_ANGLE_TO_CHANGE_TEAM = MathF.PI * 1.75f;

		const double BLINK_SPEED = 20.0;
		const double BLINK_LENGTH = 2000.0;

		const float FIRE_DISTANCE = 10.0f;

		public const int MAX_HEALTH = 5;

		#endregion rConstants





		#region rMembers

		bool mGrappleInAction;
		float mGrappleLength;
		Vector2 mGrappleDir;
		AIEntity mGrappledEntity;
		AIEntity mBestEntityToGrapple;
		float mRadialAngleSpeed;
		float mRadialAngle;
		float mTotalRadialAngleTravelled;
		Vector2 mLastFirePos = Vector2.Zero;

		MonoTimer mImmunityTimer;

		bool mOnFire = false;
		int mFireCombo = 0;


		int mHealth;

		#endregion rMembers


		#region rInit

		public Player(Vector2 pos, float angle) : base(pos, angle, MAX_SPEED)
		{
			mGrappledEntity = null;
			mGrappleInAction = false;
			mRadialAngleSpeed = 0.0f;
			mRadialAngle = 0.0f;
			mTotalRadialAngleTravelled = 0.0f;

			mImmunityTimer = new MonoTimer();

			mHealth = MAX_HEALTH;
		}

		public override void LoadContent()
		{
			mDirectionTextures = new Animator[8];
			mDirectionTextures[(int)EightDirection.Up] = MonoData.I.LoadAnimator("Player/PlayerUp");
			mDirectionTextures[(int)EightDirection.UpLeft] = MonoData.I.LoadAnimator("Player/PlayerUpLeft");
			mDirectionTextures[(int)EightDirection.Left] = new Animator(Animator.PlayType.Repeat, ("Player/PlayerLeft", 0.1f), ("Player/PlayerLeftTwo", 0.1f));
			mDirectionTextures[(int)EightDirection.DownLeft] = MonoData.I.LoadAnimator("Player/PlayerDownLeft");
			mDirectionTextures[(int)EightDirection.Down] = MonoData.I.LoadAnimator("Player/PlayerDown");
			mDirectionTextures[(int)EightDirection.DownRight] = MonoData.I.LoadAnimator("Player/PlayerDownRight");
			mDirectionTextures[(int)EightDirection.Right] = new Animator(Animator.PlayType.Repeat, ("Player/PlayerRight", 0.1f), ("Player/PlayerRightTwo", 0.1f));
			mDirectionTextures[(int)EightDirection.UpRight] = MonoData.I.LoadAnimator("Player/PlayerUpRight");
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Player/PlayerUp");

			foreach (Animator anim in mDirectionTextures)
			{
				anim.Play();
			}
		}

		#endregion rInit


		#region rUpdate

		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);

			HandleKeys(gameTime);
			HandleMouse(gameTime);

			UpdateGrapple(gameTime);

			base.Update(gameTime);

			if (MathF.Abs(mRadialAngleSpeed) > 0.0f && mGrappledEntity is not null)
			{
				mRadialAngle += dt * mRadialAngleSpeed;
				mTotalRadialAngleTravelled += MathF.Abs(dt * mRadialAngleSpeed);
				ForceAngle(-mRadialAngle - MathF.Sign(mRadialAngleSpeed) * MathF.PI / 2.0f);
				mSpeed = GRAPPLE_RADIAL_SPIN_SPEED;
				Vector2 calculatedPos = mGrappledEntity.GetCentrePos();
				calculatedPos.X += mGrappleLength * MathF.Cos(mRadialAngle);
				calculatedPos.Y += mGrappleLength * MathF.Sin(mRadialAngle);

				SetCentrePos(calculatedPos);
				ForceInBounds(GameScreen.PLAYABLE_AREA);

				mGrappleDir = mGrappledEntity.GetCentrePos() - mCentreOfMass;

				if (mTotalRadialAngleTravelled > GRAPPLE_ANGLE_TO_CHANGE_TEAM)
				{
					if (mGrappledEntity.GetTeam() == AITeam.Enemy)
					{
						Vector2 scrollPos = mCentreOfMass;
						mFireCombo++;
						if (mFireCombo > 1)
						{
							if (mFireCombo % 3 == 0)
							{
								AddHealth(1);
								FXManager.I.AddTextScroller(Color.Wheat, scrollPos, "Combo: +1HP");
								
							}
							else
							{
								FXManager.I.AddTextScroller(Color.Wheat, scrollPos, "Combo: " + mFireCombo);
							}
							scrollPos.Y += 16.0f;
						}

						int score = mGrappledEntity.GiveScore();
						score *= mFireCombo;
						score *= (1 + (RunManager.I.GetRounds() / 2));

						SoundManager.I.PlaySFX(SoundManager.SFXType.Convert, 0.7f, 0.0f, 0.0f);

						if (score > 0)
						{
							FXManager.I.AddTextScroller(Color.IndianRed, scrollPos, "+" + score);
							RunManager.I.AddScore(score);
						}
					}
					mGrappledEntity.SetTeam(AITeam.Ally);
				}
			}

			if(mImmunityTimer.IsPlaying() && mImmunityTimer.GetElapsedMs() > BLINK_LENGTH)
			{
				mImmunityTimer.FullReset();
			}

			if (mSpeed > mMaxSpeed + 3.0f)
			{
				mOnFire = true;
				Vector2 toLastFire = mCentreOfMass - mLastFirePos;
				if (toLastFire.Length() > FIRE_DISTANCE)
				{
					FXManager.I.AddFlame(mCentreOfMass + new Vector2(0.0f, 6.0f), DrawLayer.SubEntity);
					mLastFirePos = mCentreOfMass;
				}
			}
			else
			{
				mOnFire = false;
				mFireCombo = 0;
			}
		}

		void HandleKeys(GameTime gameTime)
		{
			bool up = AridArnold.InputManager.I.KeyHeld(AridArnold.AridArnoldKeys.ArnoldUp);
			bool down = AridArnold.InputManager.I.KeyHeld(AridArnold.AridArnoldKeys.ArnoldDown);
			bool left = AridArnold.InputManager.I.KeyHeld(AridArnold.AridArnoldKeys.ArnoldLeft);
			bool right = AridArnold.InputManager.I.KeyHeld(AridArnold.AridArnoldKeys.ArnoldRight);

			SetAcelerate(true);

			if (up)
			{
				if (left)
				{
					TargetDirection(EightDirection.UpLeft);
				}
				else if (right)
				{
					TargetDirection(EightDirection.UpRight);
				}
				else
				{
					TargetDirection(EightDirection.Up);
				}
			}
			else if (left)
			{
				if (down)
				{
					TargetDirection(EightDirection.DownLeft);
				}
				else
				{
					TargetDirection(EightDirection.Left);
				}
			}
			else if (down)
			{
				if (right)
				{
					TargetDirection(EightDirection.DownRight);
				}
				else
				{
					TargetDirection(EightDirection.Down);
				}
			}
			else if (right)
			{
				TargetDirection(EightDirection.Right);
			}
			else
			{
				SetAcelerate(false);
			}
		}

		void HandleMouse(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);

			if (AridArnold.InputManager.I.KeyPressed(AridArnold.AridArnoldKeys.UseItem))
			{
				BeginGrapple();
			}
				
			if (!AridArnold.InputManager.I.KeyHeld(AridArnold.AridArnoldKeys.UseItem))
			{
				EndGrapple();
			}

			if (mGrappleInAction && mGrappledEntity is null)
			{
				AimGrapple();
			}
		}

		void BeginGrapple()
		{
			mGrappleInAction = true;
			mGrappleLength = 0.0f;
			mBestEntityToGrapple = FindBestEntityToAimFor();
			mGrappleDir = MonoMath.GetVectorFromAngle(-GetCurrentAngle());
			if (mGrappledEntity is not null)
			{
				mGrappledEntity.SetBeingGrappled(false);
			}
			mGrappledEntity = null;
			mRadialAngleSpeed = 0.0f;
			mTotalRadialAngleTravelled = 0.0f;
		}

		void AimGrapple()
		{
			if (mBestEntityToGrapple is not null)
			{
				mGrappleDir = (mBestEntityToGrapple.GetCentrePos() - GetCentrePos());
			}
		}

		AIEntity FindBestEntityToAimFor()
		{
			AIEntity bestEntity = null;
			float bestEntityScore = 0.0f;

			for(int e = 0; e < EntityManager.I.GetEntityNum(); e++)
			{
				Entity entity = EntityManager.I.GetEntity(e);
				if(entity is AIEntity enemy)
				{
					float enemyScore = RateEnemyForGrapple(enemy);

					if (bestEntity is null || enemyScore > bestEntityScore)
					{
						bestEntity = enemy;
						bestEntityScore = enemyScore;
					}
				}
			}

			return bestEntity;
		}

		float RateEnemyForGrapple(AIEntity enemy)
		{
			float dist = Vector2.Distance(GetCentrePos(), enemy.GetCentrePos());

			Vector2 ourDir = MonoMath.GetVectorFromAngle(-GetCurrentAngle());
			Vector2 theirDir = MonoMath.GetVectorFromAngle(-enemy.GetCurrentAngle());

			float angleFactor = Vector2.Dot(ourDir, theirDir); // Between -1 and 1
			angleFactor = 1.0f + angleFactor * 0.2f; // Between 0.8 and 1.2

			float teamFactor = enemy.GetTeam() == AITeam.Enemy ? 1.2f : 0.65f; // Prefer enemies over allies

			return (1000.0f / (1.0f + dist)) * angleFactor * teamFactor;
		}

		void EndGrapple()
		{
			mGrappleInAction = false;
			if (mGrappledEntity is not null)
			{
				mGrappledEntity.SetBeingGrappled(false);
			}
			mGrappledEntity = null;
			mRadialAngleSpeed = 0.0f;

			if (mTotalRadialAngleTravelled > MathF.PI)
			{
				mSpeed += 10.0f;
			}

			mTotalRadialAngleTravelled = 0.0f;
		}

		void UpdateGrapple(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);
			if (mGrappleInAction)
			{
				if (mGrappledEntity is not null && mGrappledEntity.IsDead())
				{
					EndGrapple();
					return;
				}

				if (mGrappledEntity is null)
				{
					if (mGrappleLength >= GRAPPLE_MAX_LENGTH)
					{
						EndGrapple();
					}
					else
					{
						float speed = Math.Max(GRAPPLE_EXTEND_SPEED - (mGrappleLength / GRAPPLE_MAX_LENGTH) * GRAPPLE_EXTEND_SPEED * 0.85f, 3.0f);
						mGrappleLength += dt * speed;
					}

					Vector2 grappleHead = GetGrappleHead();
					AIEntity nearestToGrappleHead = EntityManager.I.GetNearestOfType<AIEntity>(GetGrappleHead());
					float distanceToGrappleHead = (grappleHead - nearestToGrappleHead.GetCentrePos()).Length();

					if (distanceToGrappleHead < GRAPPLE_LOCK_DISTANCE)
					{
						mGrappledEntity = nearestToGrappleHead;
						mGrappledEntity.SetBeingGrappled(true);
					}
				}
				else if (mRadialAngleSpeed == 0.0f)
				{
					mGrappleDir = (mGrappledEntity.GetCentrePos() - GetCentrePos());
					mGrappleLength = mGrappleDir.Length();

					Vector2 velocity = GetVelocity();

					if (velocity.LengthSquared() > 0.0f)
					{
						Vector2 ourVelocityDir = Vector2.Normalize(GetVelocity());
						Vector2 ourGrappleDir = Vector2.Normalize(mGrappleDir);

						float angleDiff = MonoMath.GetAngleDiff(ourVelocityDir, ourGrappleDir);

						bool angleIsGood = MathF.Abs(angleDiff) > GRAPPLE_RADIAL_SPIN_START_ANGLE;
						bool grappleIsBigEnough = mGrappleLength > GRAPPLE_RADIAL_SPIN_MIN_RADIUS;
						bool grappleIsTooBig = mGrappleLength > GRAPPLE_MAX_LENGTH;

						if ((angleIsGood && grappleIsBigEnough) || grappleIsTooBig)
						{
							float crossProduct = ourVelocityDir.X * ourGrappleDir.Y - ourVelocityDir.Y * ourGrappleDir.X;
							mRadialAngleSpeed = MathF.Sign(crossProduct) * GRAPPLE_RADIAL_SPIN_SPEED / mGrappleLength;
							mRadialAngle = MathF.Atan2(-mGrappleDir.Y, -mGrappleDir.X);
						}
					}
					else if (mGrappleLength > GRAPPLE_MAX_LENGTH)
					{
						EndGrapple();
					}
				}
			}
			else
			{
				float speed = Math.Max(GRAPPLE_EXTEND_SPEED - (mGrappleLength / GRAPPLE_MAX_LENGTH) * GRAPPLE_EXTEND_SPEED * 0.25f, 3.0f);
				speed *= 1.5f;
				mGrappleLength -= dt * speed;
			}
		}

		#endregion rUpdate





		#region rDraw

		public override void Draw(DrawInfo info)
		{
			// Grapple
			if (mGrappleLength > 0.0f)
			{
				Vector2 grappleStart = mCentreOfMass;
				Vector2 grappleEnd = GetGrappleHead();

				if (mGrappledEntity is not null)
				{
					grappleEnd = mGrappledEntity.GetCentrePos();
				}

				MonoDraw.DrawLine(info, grappleStart, grappleEnd, Color.Black, 2.0f, DrawLayer.SubEntity);
			}

			if((mImmunityTimer.GetElapsedMs() % (BLINK_SPEED * 2.0) > BLINK_SPEED))
			{
				return;
			}

			base.Draw(info);
		}

		#endregion rDraw


		#region rUtility

		Vector2 GetGrappleHead()
		{
			Vector2 grappleDir = mGrappleDir;
			grappleDir.Normalize();
			return mCentreOfMass + grappleDir * mGrappleLength;
		}

		public void AddHealth(int delta)
		{
			mHealth += delta;
			mHealth = Math.Min(mHealth, MAX_HEALTH);
			if (mHealth <= 0)
			{
				Kill();
			}
			else if (delta < 0 && !mImmunityTimer.IsPlaying())
			{
				SoundManager.I.PlaySFX(SoundManager.SFXType.PlayerHit, 0.6f);
				Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.ScreenCamera);

				gameCam.QueueMovement(new DiminishCameraShake(1.5f, 3.0f, 25.0f));

				mImmunityTimer.Start();
			}
		}

		public void SetHealth(int health)
		{
			mHealth = Math.Min(health, MAX_HEALTH);
		}

		public int GetHealth()
		{
			return mHealth;
		}


		#endregion rUtility
	}
}
