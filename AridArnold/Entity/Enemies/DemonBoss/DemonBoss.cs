
using Microsoft.Xna.Framework.Input;

namespace AridArnold
{
	internal class DemonBoss : Entity
	{
		#region rTypes

		enum JawPhase
		{
			kClosed,
			kOpening,
			kShooting,
			kClosing,
		}

		#endregion rTypes





		#region rConstants

		// Jaw
		const float JAW_TIME = 1600.0f;
		static Vector2 JAW_CLOSED_OFFSET = new Vector2(35.0f, 81.0f);
		static Vector2 JAW_OPEN_OFFSET = new Vector2(35.0f, 103.0f);
		static Vector2 JAW_FIRE_BALL_OFFSET = new Vector2(56.0f, 112.0f);

		// Floating
		const float FLOAT_FREQ = 5.0f;
		const float FLOAT_AMP = 3.0f;
		const float HIT_TIME = 800.0f;

		// Eye
		static Vector2[] EYE_POSITIONS = { new Vector2(59.0f, 51.0f), new Vector2(38.0f, 74.0f), new Vector2(80.0f, 74.0f) };
		static Vector2[] GIBLET_POSITION = { Vector2.Zero, new Vector2(-5.0f, -5.0f), new Vector2(5.0f, -5.0f), new Vector2(-5.0f, 5.0f), new Vector2(5.0f, 5.0f) };
		static float EYE_LOOK_RADIUS = 9.0f;
		static float EYE_HIT_RADIUS = 15.0f;

		#endregion rConstants





		#region rMembers

		// Floating
		Vector2 mBasePos;
		MonoTimer mFloatTimer;

		// Jaw
		JawPhase mJawPhase;
		Texture2D mJawTexture;
		PercentageTimer mJawTimer;
		DemonBossFireBall mCurrFileball = null;

		// Eyes
		Texture2D mEyeTexture;
		Texture2D mDeadEyeTexture;
		bool[] mEyesAlive;
		PercentageTimer mEyeHitTimer;

		// Emitter
		BoxSmokeEmitter mParticleEmitter;

		//State
		MonoTimer mDeadTimer;

		// Unlock
		KeyItemTile mKeyItemTile;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create the demon boss
		/// </summary>
		public DemonBoss(Vector2 pos) : base(pos)
		{
			mDeadTimer = new MonoTimer();

			mBasePos = pos;
			mFloatTimer = new MonoTimer();
			mFloatTimer.Start();

			mJawPhase = JawPhase.kClosed;
			mJawTimer = new PercentageTimer(JAW_TIME);
			mJawTimer.ResetStart();

			mEyeHitTimer = new PercentageTimer(HIT_TIME);

			Vector2 emitMin = pos + new Vector2(15.0f, 20.0f);
			mParticleEmitter = new BoxSmokeEmitter(new Rect2f(emitMin, 83.0f, 20.0f), 0.4f);

			// All eyes alive.
			mEyesAlive = new bool[EYE_POSITIONS.Length];
			for(int i = 0; i < mEyesAlive.Length; i++)
			{
				mEyesAlive[i] = true;
			}

			mKeyItemTile = TileManager.I.GetTile<KeyItemTile>();
			mKeyItemTile.pEnabled = false;
		}



		/// <summary>
		/// Create the demon boss
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("DemonBoss/Face");
			mJawTexture = MonoData.I.MonoGameLoad<Texture2D>("DemonBoss/Jaw");

			mEyeTexture = MonoData.I.MonoGameLoad<Texture2D>("DemonBoss/Iris");
			mDeadEyeTexture = MonoData.I.MonoGameLoad<Texture2D>("DemonBoss/DeadEye");
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update the entity
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mEyeHitTimer.Update(gameTime);
			mParticleEmitter.Update(gameTime);
			mFloatTimer.Update(gameTime);
			mJawTimer.Update(gameTime);
			mDeadTimer.Update(gameTime);

			bool dead = AreWeDead();
			if(dead && !mDeadTimer.IsPlaying())
			{
				BeginDeath();
				mDeadTimer.Start();
			}
			
			if(!dead)
			{
				if (mKeyItemTile is not null)
				{
					mKeyItemTile.pEnabled = false;
				}

				float floatT = mFloatTimer.GetElapsedMsF() / 1000.0f;
				float deltaY = MathF.Sin(floatT * FLOAT_FREQ) * FLOAT_AMP;
				if (mEyeHitTimer.IsPlaying())
				{
					float t = mEyeHitTimer.GetPercentageF();
					if (t >= 1.0f)
					{
						mEyeHitTimer.FullReset();
					}

					deltaY += MathF.Min(15.0f * t, 2.0f - 2.0f * t) * 20.0f;
				}
				mPosition.Y = mBasePos.Y + deltaY;

				UpdateJaw(gameTime);
			}
			else
			{
				float t = mDeadTimer.GetElapsedMsF() / 1000.0f;
				mPosition.Y = mBasePos.Y + 8.0f * t * t;
				mParticleEmitter.MoveTo(GetPos());

				if(mKeyItemTile is not null && mDeadTimer.GetElapsedMs() > 2000.0)
				{
					mKeyItemTile.pEnabled = true;
					mKeyItemTile = null;
				}
			}
			

			base.Update(gameTime);
		}



		/// <summary>
		/// Jaw logic
		/// </summary>
		void UpdateJaw(GameTime gameTime)
		{
			MonoRandom rng = RandomManager.I.GetWorld();
			if (mJawTimer.GetPercentageF() >= 1.0f)
			{
				// Do jaw phase..
				switch (mJawPhase)
				{
					case JawPhase.kClosed:
						if (rng.PercentChance(50.0f))
						{
							mJawPhase = JawPhase.kOpening;
							mJawTimer.ResetStart();
						}
						break;
					case JawPhase.kOpening:
						mJawPhase = JawPhase.kShooting;
						mJawTimer.ResetStart();

						SFXManager.I.PlaySFX(AridArnoldSFX.BigFireShoot, 0.2f);
						mCurrFileball = new DemonBossFireBall(CalcFireBallChargePos());
						EntityManager.I.QueueRegisterEntity(mCurrFileball);
						break;
					case JawPhase.kShooting:
						// Shoot bullet
						Arnold arnold = EntityManager.I.FindArnold();

						if (arnold is not null)
						{
							mCurrFileball.TargetEntity(arnold);
							mCurrFileball = null; // Unlink us

							mJawPhase = JawPhase.kClosing;
							mJawTimer.ResetStart();
						}
						break;
					case JawPhase.kClosing:
						mJawPhase = JawPhase.kClosed;
						mJawTimer.ResetStart();
						break;
				}
			}

			if(mCurrFileball is not null)
			{
				mCurrFileball.SetPos(CalcFireBallChargePos());
			}
		}


		/// <summary>
		/// Calculate where the fireball should charge
		/// </summary>
		Vector2 CalcFireBallChargePos()
		{
			return GetPos() + JAW_FIRE_BALL_OFFSET;
		}



		/// <summary>
		/// Collisions with gravity tiles
		/// </summary>
		public override void OnCollideEntity(Entity entity)
		{
			if(entity is GravityTile)
			{
				Vector2 theirCentre = entity.GetCentrePos();

				for(int i = 0; i < EYE_POSITIONS.Length; i++)
				{
					Vector2 eyeAbsPos = GetPos() + EYE_POSITIONS[i];
					Vector2 toThem = eyeAbsPos - theirCentre;
					if(toThem.Length() < EYE_HIT_RADIUS)
					{
						OnHitEye(entity, i);
					}
				}
			}
			base.OnCollideEntity(entity);
		}


		/// <summary>
		/// Once eye is hit
		/// </summary>
		void OnHitEye(Entity hitter, int eyeIdx)
		{
			// Destroy hitter
			EntityManager.I.QueueDeleteEntity(hitter);

			// Destroy eye
			mEyesAlive[eyeIdx] = false;

			mEyeHitTimer.ResetStart();

			// Spawn giblets
			Vector2 eyePos = EYE_POSITIONS[eyeIdx] + GetPos();
			for(int i = 0; i < GIBLET_POSITION.Length; i++)
			{
				Vector2 gibletVelocity = GIBLET_POSITION[i];
				Vector2 gibletPos = GIBLET_POSITION[i] + eyePos;

				SpawnEyeGiblet(gibletPos, gibletVelocity);
			}

			// Sound
			SFXManager.I.PlaySFX(AridArnoldSFX.BossHit, 0.4f, 0.0f, 0.05f);

			// Shake
			Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera);
			DiminishCameraShake shakeMove = new DiminishCameraShake(9.0f, 10.0f, 100.0f);
			gameCam.QueueMovement(shakeMove);
		}



		/// <summary>
		/// Are we dead?
		/// </summary>
		bool AreWeDead()
		{
			for(int i = 0; i < mEyesAlive.Length; i++)
			{
				if (mEyesAlive[i])
				{
					return false;
				}
			}

			return true;
		}



		/// <summary>
		/// Called once at the start of death
		/// </summary>
		void BeginDeath()
		{
			EventManager.I.TriggerEvent(EventType.DemonBossKilled);

			mJawPhase = JawPhase.kShooting;
			int entityNum = EntityManager.I.GetEntityNum();
			for(int i = 0; i < entityNum; i++) 
			{
				Entity entity = EntityManager.I.GetEntity(i);
				if(entity is DemonBossFireBall)
				{
					EntityManager.I.QueueDeleteEntity(entity);
				}
			}
			mCurrFileball = null;

			// Sound
			SFXManager.I.PlaySFX(AridArnoldSFX.BossDeath, 0.6f);

			// Shake
			Camera screenCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.ScreenCamera);
			DiminishCameraShake screenShakeMove = new DiminishCameraShake(30.0f, 10.0f, 100.0f);
			screenCam.DoMovement(screenShakeMove);

			Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera);
			DiminishCameraShake gameShakeMove = new DiminishCameraShake(9.0f, 10.0f, 100.0f);
			gameCam.QueueMovement(gameShakeMove);
		}

		#endregion rUpdate




		#region rDraw

		/// <summary>
		/// Draw the demon boss
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			Vector2 texPos = mPosition;
			Vector2 jawPos = texPos + GetJawPos();

			MonoDraw.DrawTextureDepthColor(info, mJawTexture, jawPos, CalcDrawColor(), DrawLayer.Default);
			MonoDraw.DrawTextureDepthColor(info, mTexture, texPos, CalcDrawColor(), DrawLayer.Default);

			DrawEyes(info);
		}



		/// <summary>
		/// Draw the eyes to look at arnold.
		/// </summary>
		void DrawEyes(DrawInfo info)
		{
			Arnold arnold = EntityManager.I.FindArnold();

			for(int i = 0; i < EYE_POSITIONS.Length; i++)
			{
				if (mEyesAlive[i])
				{
					Vector2 absEyePos = GetPos() + EYE_POSITIONS[i];
					Vector2 eyeLookTo = arnold is null ? absEyePos : arnold.GetCentrePos();
					Vector2 eyePositionDelta = eyeLookTo - absEyePos;

					// Clamp to radius..
					if (eyePositionDelta.LengthSquared() > EYE_LOOK_RADIUS)
					{
						eyePositionDelta.Normalize();
						eyePositionDelta *= EYE_LOOK_RADIUS;
					}

					Vector2 finalEyePosition = absEyePos + eyePositionDelta;
					finalEyePosition.X -= mEyeTexture.Width * 0.5f;
					finalEyePosition.Y -= mEyeTexture.Height * 0.5f;
					MonoDraw.DrawTextureDepthColor(info, mEyeTexture, finalEyePosition, CalcDrawColor(), DrawLayer.Default);
				}
				else
				{
					Vector2 deadEyePos = GetPos() + EYE_POSITIONS[i];
					deadEyePos.X -= mDeadEyeTexture.Width * 0.5f;
					deadEyePos.Y -= mDeadEyeTexture.Height * 0.5f;
					MonoDraw.DrawTextureDepthColor(info, mDeadEyeTexture, deadEyePos, CalcDrawColor(), DrawLayer.Default);
				}
			}
		}



		/// <summary>
		/// Get jaw relative position
		/// </summary>
		public Vector2 GetJawPos()
		{
			float t = 0.0f;
			switch (mJawPhase)
			{
				case JawPhase.kOpening:
					t = mJawTimer.GetPercentageF();
					break;
				case JawPhase.kShooting:
					t = 1.0f;
					break;
				case JawPhase.kClosing:
					t = 1.0f - mJawTimer.GetPercentageF();
					break;
			}

			return MonoMath.Lerp(JAW_CLOSED_OFFSET, JAW_OPEN_OFFSET, t);
		}



		/// <summary>
		/// Get draw color for flashing effect
		/// </summary>
		Color CalcDrawColor()
		{
			if(mEyeHitTimer.GetElapsedMsF() % 120.0f > 60.0f)
			{
				return Color.DarkGray;
			}

			if (mDeadTimer.GetElapsedMsF() % 120.0f > 60.0f)
			{
				return Color.DarkGray;
			}

			return Color.White;
		}


		void SpawnEyeGiblet(Vector2 pos, Vector2 vel)
		{
			Vector2 size = new Vector2(4.0f, 6.0f);
			GibletFX giblet = new GibletFX(pos, size, vel, new Color(255, 228, 217));

			FXManager.I.AddFX(giblet);
		}

		#endregion rDraw
	}
}
