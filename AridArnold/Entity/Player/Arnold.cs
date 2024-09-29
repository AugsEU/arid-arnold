//#define ARNOLD_DEBUG_SOUND

namespace AridArnold
{
	/// <summary>
	/// The playable character, our hero, our saviour, Arnold.
	/// </summary>
	class Arnold : PlatformingEntity
	{
		#region rConstants

		const double START_TIME = 500.0;
		const double USE_ITEM_TIME = 450.0;
		const int COYOTE_TIME = 8;
		const int ROSS_TIME = 5;
		const float JUMP_BUFFER_DURATION = 200.0f;

		const float ARNOLD_WALK_SPEED = 9.05f;
		const float ARNOLD_GRAVITY = 4.35f;
		const float ARNOLD_JUMP_SPEED = 25.0f;
		const float ARNOLD_AIR_SPEED_BOOST = 0.015f;
		const float HORSE_WALK_SPEED = 20.0f;

		static Vector2 ITEM_OFFSET = new Vector2(-2.0f, -15.0f);

		static int REVERSE_TIME_FRAMES = 120;

		#endregion rConstants





		#region rMembers

		protected Animator mRunningAnimation;

		protected Texture2D mJumpUpTex;
		protected Texture2D mJumpDownTex;
		protected Texture2D mUseItemTex;

		protected MonoTexturePack mYoungTexturePack;
		protected MonoTexturePack mOldTexturePack;
		protected MonoTexturePack mHorseTexturePack;

		//Various timers.
		protected PercentageTimer mTimerSinceStart;

		// Items
		Item mCurrItem;
		PercentageTimer mUseItemTimer;
		MonoRandom mChaosRandom;

		int mReverseTimeFrame = 0;
		int mReverseFramesMax = REVERSE_TIME_FRAMES;
		GhostFile mReverseTimeFile = null;

		// Inputs(can change based on camera angle etc)
		InputAction mLeftKey;
		InputAction mRightKey;
		InputAction mDownKey;

		// v1.1 Use jump buffer.
		InputBuffer mJumpBuffer;

		// RandomModes
		bool mHorseMode;
		bool mBouncyMode;

		GhostSkin mLoadedSkin = GhostSkin.kYoung;


#if ARNOLD_DEBUG_SOUND
		SpacialSFX mDebugSound = null;
#endif

#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Construct Arnold from position
		/// </summary>
		/// <param name="pos">Starting position</param>
		public Arnold(Vector2 pos, bool horseMode = false) : base(pos, ARNOLD_WALK_SPEED, ARNOLD_JUMP_SPEED, ARNOLD_GRAVITY)
		{
			mPrevDirection = WalkDirection.Right;

			mTimerSinceStart = new PercentageTimer(START_TIME);

			mUseItemTimer = new PercentageTimer(USE_ITEM_TIME);

			mLeftKey = InputAction.ArnoldLeft;
			mRightKey = InputAction.ArnoldRight;
			mDownKey = InputAction.ArnoldDown;

			mJumpBuffer = new InputBuffer(InputAction.ArnoldJump, JUMP_BUFFER_DURATION);

			mCurrItem = null;
			LayerOptIn(InteractionLayer.kPlayer);

			SetDustIntensity(15.0f);

			if(horseMode)
			{
				SetHorseMode();
			}

			mBouncyMode = false;
		}



		/// <summary>
		/// Load textures and assets
		/// </summary>
		public override void LoadContent()
		{
			InitTexturePacks();
			RefreshTexturePack();

			//Botch position a bit. Not sure what's happening here.
			mPosition.Y -= 1.0f;
			mPosition.X += 2.0f;

			mVelocity.Y = +0.01f;

			SFXFactory jumpFactory = new SFXFactory(AridArnoldSFX.ArnoldJump, 0.40f, 0.0f, 0.15f);
			LoadSFX(jumpFactory, AridArnoldSFX.ArnoldWalk, 0.14f);
			mSpacialJumpSFX = false;

			mChaosRandom = new MonoRandom(120);
		}



		/// <summary>
		/// Load young/old textures
		/// </summary>
		protected virtual void InitTexturePacks()
		{
			mYoungTexturePack = new MonoTexturePack("Arnold/YoungArnold.mtp");
			mOldTexturePack = new MonoTexturePack("Arnold/OldArnold.mtp");
			mHorseTexturePack = new MonoTexturePack("Arnold/HorseMode.mtp");
		}



		/// <summary>
		/// Load textures from a pack
		/// </summary>
		protected void LoadTexturePack(MonoTexturePack texturePack)
		{
			mTexture = texturePack.GetTexture("Stand");
			mJumpUpTex = texturePack.GetTexture("JumpUp");
			mJumpDownTex = texturePack.GetTexture("JumpDown");
			mUseItemTex = texturePack.GetTexture("UseItem");

			float playHead = mRunningAnimation is null ? 0.0f : mRunningAnimation.GetPlayHead();
			mRunningAnimation = new Animator(Animator.PlayType.Repeat,
												(texturePack.GetTexture("Run1"), 0.1f),
												(texturePack.GetTexture("Run2"), 0.1f),
												(texturePack.GetTexture("Run3"), 0.1f),
												(texturePack.GetTexture("Run4"), 0.15f));

			mRunningAnimation.Play();
			mRunningAnimation.SetPlayHead(playHead);
		}



		/// <summary>
		/// Make arnold into a horse
		/// </summary>
		public void SetHorseMode()
		{
			if(mHorseMode)
			{
				return;
			}

			mHorseMode = true;
			mWalkSpeed = HORSE_WALK_SPEED;

			if (mHorseTexturePack is not null)
			{
				RefreshTexturePack();
				mPosition += -GravityVecNorm() * 10.0f;
			}

			SFXManager.I.PlaySFX(AridArnoldSFX.HorseBorn, 0.4f);
		}

		#endregion rInitialisation





		#region rProperties

		/// <summary>
		/// Should this entity persist after we come back from a door
		/// </summary>
		public override bool PersistLevelEntry()
		{
			return true;
		}



		/// <summary>
		/// Are we a horse?
		/// </summary>
		public bool IsHorseMode()
		{
			return mHorseMode;
		}



		/// <summary>
		/// Are we in choas mode?
		/// </summary>
		public bool IsChaosMode()
		{
			Item currItem = ItemManager.I.GetActiveItem();
			return currItem is ChaosDrive;
		}

		#endregion rProperties





		#region rUpdate

		/// <summary>
		/// Update Arnold
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public override void Update(GameTime gameTime)
		{
			// We are listening...
			SFXManager.I.AddListener(this);

#if ARNOLD_DEBUG_SOUND
			if(mDebugSound is null)
			{
				mDebugSound = new SpacialSFX(AridArnoldSFX.FutronBomb, new Vector2(295.0f, 491.0f), 1.0f);
				mDebugSound.GetBuffer().SetLoop(true);

				SFXManager.I.PlaySFX(mDebugSound);
			}
#endif

			mTimerSinceStart.Update(gameTime);
			mUseItemTimer.Update(gameTime);

			//Start
			if (mTimerSinceStart.IsPlaying() == false)
			{
				mTimerSinceStart.Start();
			}

			if (mTimerSinceStart.GetPercentage() < 1.0)
			{
				return;
			}
			else if (mTimerSinceStart.GetPercentage() == 1.0)
			{
				mTimerSinceStart.Stop();
			}

			//Death
			if (mTimerSinceDeath.IsPlaying())
			{
				if (mTimerSinceDeath.GetPercentage() >= 0.95)
				{
					SendPlayerDeathEvent();
				}

				mTimerSinceDeath.Update(gameTime);
				return;
			}

			//Anim
			mRunningAnimation.Update(gameTime);

			// Reverse time
			if(mReverseTimeFile is not null)
			{
				while(mReverseTimeFile is not null)
				{
					HandleTimeReverse(gameTime);
					if(mVelocity.LengthSquared() < 1.0f)
					{
						// Do one more frame of reverse and keep reversing.
						mReverseFramesMax++;
					}
					else
					{
						break;
					}
				}
				return;
			}

			//Collider
			EntityManager.I.AddColliderSubmission(new EntityColliderSubmission(this));

			//Item
			if (mUseItemTimer.IsPlaying() && mCurrItem is not null)
			{
				if (mUseItemTimer.GetPercentage() == 1.0)
				{
					mUseItemTimer.FullReset();
					mCurrItem.Begin();
				}

				return;
			}

			if(mCurrItem is not null)
			{
				mCurrItem.Update(gameTime);
				mCurrItem.ActOnArnold(gameTime, this);

				// Check if no longer active, then bin it.
				if(!mCurrItem.IsActive())
				{
					mCurrItem = null;
				}
			}

			if (mOnGround == false)
			{
				SetPrevWalkDirFromVelocity();
				mWalkSpeed = (mHorseMode ? HORSE_WALK_SPEED : ARNOLD_WALK_SPEED) + ARNOLD_AIR_SPEED_BOOST;
			}
			else
			{
				mWalkSpeed = (mHorseMode ? HORSE_WALK_SPEED : ARNOLD_WALK_SPEED);
			}

			if (CheckOffScreenDeath())
			{
				Kill();
			}

			base.Update(gameTime);
		}


		/// <summary>
		/// Handle reverse
		/// </summary>
		void HandleTimeReverse(GameTime gameTime)
		{
			int frame = mReverseTimeFile.GetFrameCount() - mReverseTimeFrame;
			if (frame < 0 || mReverseTimeFrame >= mReverseFramesMax)
			{
				mReverseTimeFile = null;
				mReverseTimeFrame = 0;
				return;
			}

			List<GhostInfo> infos = mReverseTimeFile.ReadFrame(frame);

			if(infos.Count == 0)
			{
				mReverseTimeFile = null;
				mReverseTimeFrame = 0;
				return;
			}

			// Hack: Find closest one which should be us
			int bestIdx = 0;
			float bestIdxDist = (infos[0].mPosition - mPosition).LengthSquared();

			for(int i = 1; i < infos.Count; i++)
			{
				float newDist = (infos[i].mPosition - mPosition).LengthSquared();
				if(newDist < bestIdxDist)
				{
					bestIdx = i;
					bestIdxDist = newDist;
				}
			}

			// Set our info...
			GhostInfo ourInfo = infos[bestIdx];

			mPosition = ourInfo.mPosition;
			mVelocity = ourInfo.mVelocity;
			SetGrounded(ourInfo.mGrounded);
			SetGravity(ourInfo.mGravity);
			SetWalkDirection(ourInfo.mWalkDirection);
			SetPrevWalkDirection(ourInfo.mPrevWalkDirection);

			// Inc
			mReverseTimeFrame++;
		}



		/// <summary>
		/// Kill Arnold
		/// </summary>
		public override void Kill()
		{
			if(mTimerSinceDeath.IsPlaying() || mReverseTimeFile is not null)
			{
				return;
			}

			Item currItem = ItemManager.I.GetActiveItem();
			if(currItem is ReverseWatch)
			{
				mReverseTimeFile = GhostManager.I.GetOutputFile();
				if(mReverseTimeFile is not null)
				{
					ItemManager.I.PopActiveItem();
					mReverseTimeFrame = 0;
					GhostManager.I.StopRecording();
					mReverseFramesMax = REVERSE_TIME_FRAMES;
					SFXManager.I.PlaySFX(AridArnoldSFX.QuickAgeBackward, 0.2f);
					return;
				}
			}

			// Fade them all out as the level is about to reset.
			SFXManager.I.EndAllSFX(160.0f);

			CameraShake cameraShake = new CameraShake(1.0f, 2.0f, 30.0f);
			Camera cam = CameraManager.I.GetCamera(CameraManager.CameraInstance.ScreenCamera);
			cam.QueueMovement(cameraShake);

			SFXManager.I.PlaySFX(AridArnoldSFX.ArnoldDeath, 0.2f);

			base.Kill();
		}



		/// <summary>
		/// Ordered update for physics
		/// </summary>
		public override void OrderedUpdate(GameTime gameTime)
		{
			if (mTimerSinceStart.IsPlaying() || mTimerSinceDeath.IsPlaying() || mUseItemTimer.IsPlaying())
			{
				return;
			}

			if(mReverseTimeFile is not null)
			{
				return;
			}

			DoInputs(gameTime);

			mGravity = DEFAULT_GRAVITY;

			mWalkSpeed = mHorseMode ? HORSE_WALK_SPEED : ARNOLD_WALK_SPEED;

			Item currItem = ItemManager.I.GetActiveItem();
			if(currItem is MoonBoots)
			{
				mGravity = DEFAULT_GRAVITY * 0.7f;
			}
			else if(currItem is Trainers)
			{
				mWalkSpeed = mWalkSpeed * 1.2f;
			}

			if (IsChaosMode())
			{
				MonoRandom rng = mChaosRandom; // Shorter name
				mWalkSpeed = rng.GetFloatRange(ARNOLD_WALK_SPEED * 0.5f, 1.8f * ARNOLD_WALK_SPEED);
				mGravity = rng.GetFloatRange(DEFAULT_GRAVITY * 0.5f, 1.5f * DEFAULT_GRAVITY);

				const float MAX_WEIRD_SPEED = 40.0f;
				if(rng.PercentChance(0.1f))
				{
					float xv = rng.GetFloatRange(4.0f, MAX_WEIRD_SPEED) * (rng.GetIntRange(0, 1) * 2 - 1);
					float yv = rng.GetFloatRange(4.0f, MAX_WEIRD_SPEED) * (rng.GetIntRange(0, 1) * 2 - 1);

					mVelocity = new Vector2(xv, yv);

					if (mHorseMode)
					{
						if (rng.PercentChance(30.0f))
						{
							mHorseMode = false;
							RefreshTexturePack();
						}
					}
					else
					{
						if (rng.PercentChance(1.5f))
						{
							SetHorseMode();
						}
						else if (rng.PercentChance(0.1f))
						{
							mBouncyMode = rng.PercentChance(50.0f);
						}
					}
					
					
				}
			}

			base.OrderedUpdate(gameTime);
		}


		/// <summary>
		/// Handle inputs
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		private void DoInputs(GameTime gameTime)
		{
			mJumpBuffer.Update(gameTime);

			UpdateArnoldKeys();

			bool jumpHold = InputManager.I.KeyHeld(InputAction.ArnoldJump);
			bool fallthrough = InputManager.I.KeyHeld(mDownKey);

			bool jump = jumpHold;

			if(!OptionsManager.I.GetHoldJump())
			{
				jump = mJumpBuffer.QueryAction();
			}

			if (jumpHold && fallthrough)
			{
				if (!(mOnGround && mWalkDirection != WalkDirection.None))
				{
					FallThroughPlatforms();
				}
			}
			else if (CanWalkDirChange() || mOnGround)
			{
				HandleWalkInput();

				if (jump && mOnGround)
				{
					Jump();
					mJumpBuffer.NotifyActionDone();
				}
			}
			else if (IsGroundedSince(COYOTE_TIME) && !HasJumpedInTheLast(COYOTE_TIME + 1) && !HasFallThroughInLast(COYOTE_TIME + 1))
			{
				bool goingDown = Vector2.Dot(mVelocity, GravityVecNorm()) > 0.0f;
				if (jump && goingDown)
				{
					float originalJumpSpeed = mJumpSpeed;
					mJumpSpeed += 1.03f;
					Jump();
					mJumpBuffer.NotifyActionDone();
					mJumpSpeed = originalJumpSpeed;
				}
			}
			else if (HasJumpedInTheLast(ROSS_TIME) && mWalkDirection == WalkDirection.None)
			{
				HandleWalkInput();
			}

			// Items
			bool useItem = InputManager.I.KeyPressed(InputAction.UseItem);
			if (useItem && mCurrItem is null)
			{
				Item newItem = ItemManager.I.GetActiveItem();
				if (newItem is not null && newItem.CanUseItem(this))
				{
					SFXManager.I.PlaySFX(AridArnoldSFX.UseItem, 0.5f);
					mUseItemTimer.ResetStart();
					mCurrItem = ItemManager.I.PopActiveItem();
					mWalkDirection = WalkDirection.None;

					// Using an item bars us from recording.
					GhostManager.I.StopRecording();
				}
			}
		}



		/// <summary>
		/// Check for walk inputs
		/// </summary>
		private void HandleWalkInput()
		{
			if (InputManager.I.KeyHeld(mLeftKey))
			{
				mWalkDirection = WalkDirection.Left;
				mPrevDirection = mWalkDirection;
			}
			else if (InputManager.I.KeyHeld(mRightKey))
			{
				mWalkDirection = WalkDirection.Right;
				mPrevDirection = mWalkDirection;
			}
			else
			{
				mWalkDirection = WalkDirection.None;
			}
		}



		/// <summary>
		/// Deal with things touching us.
		/// </summary>
		public override void OnCollideEntity(Entity entity)
		{
			// HACK: Arnolds can collapse on themselves, so we add a force here to untangle them
			if (entity is Arnold)
			{
				if (mHandle < entity.GetHandle())
				{
					const float UNTANGLE_DISTANCE = 5.0f;

					Vector2 displacement = entity.GetPos() - mPosition;

					if (displacement.LengthSquared() < UNTANGLE_DISTANCE * UNTANGLE_DISTANCE)
					{
						mVelocity.Y -= 1.0f;
					}
				}
			}

			base.OnCollideEntity(entity);
		}



		/// <summary>
		/// Called when the time changes.
		/// </summary>
		protected override void OnTimeChange(GameTime gameTime)
		{
			RefreshTexturePack();
		}



		/// <summary>
		/// Refresh the textures so we update with age
		/// </summary>
		protected virtual void RefreshTexturePack()
		{
			if(mHorseMode)
			{
				LoadTexturePack(mHorseTexturePack);
				mLoadedSkin = GhostSkin.kHorse;
				return;
			}

			switch (TimeZoneManager.I.GetCurrentPlayerAge())
			{
				case 0:
					LoadTexturePack(mYoungTexturePack);
					mLoadedSkin = GhostSkin.kYoung;
					break;
				case 1:
					LoadTexturePack(mOldTexturePack);
					mLoadedSkin = GhostSkin.kOld;
					break;
				default:
					throw new Exception("Invalid Arnold age");
			}
		}


		/// <summary>
		/// React to collision with walls/ceilings/floors
		/// </summary>
		protected override void ReactToCollision(CollisionType collisionType)
		{
			if (mBouncyMode)
			{
				Vector2 downVec = GravityVecNorm();
				Vector2 sideVec = MonoMath.Perpendicular(downVec);

				Vector2 downVel = Vector2.Dot(downVec, mPrevVelocity) * downVec;
				Vector2 sideVel = Vector2.Dot(sideVec, mPrevVelocity) * sideVec;

				switch (collisionType)
				{ 
					case CollisionType.Ground:
						if (downVel.LengthSquared() > 120.0f)
							mVelocity -= downVel * 1.95f;
						else
							mVelocity = sideVel + -GravityVecNorm() * 30.0f;
						break;
					case CollisionType.Wall:
						OverrideVelocity(downVel - sideVel);
						ReverseWalkDirection();
						break;
					case CollisionType.Ceiling:
						break;
					default:
						break;
				}
			}
			else
			{
				switch (collisionType)
				{
					case CollisionType.Ground:
						float downVel = Vector2.Dot(mPrevVelocity, GravityVecNorm());
						downVel = MathF.Abs(downVel);
						float thresh = MathF.Abs(mJumpSpeed * 1.5f);

						if (downVel > thresh)
						{
							float volumeMod = MonoMath.SquashToRange(downVel - thresh, -1.0f, 1.0f);
							volumeMod = Math.Clamp(volumeMod, 0.0f, 1.0f);

							SFXManager.I.PlaySFX(AridArnoldSFX.ArnoldLand, 0.2f * volumeMod);
							CameraShake cameraShake = new CameraShake(volumeMod * 0.7f, 2.0f, 100.0f);
							Camera cam = CameraManager.I.GetCamera(CameraManager.CameraInstance.ScreenCamera);
							cam.QueueMovement(cameraShake);
						}
						break;
				}
			}

			base.ReactToCollision(collisionType);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get Arnold texture
		/// </summary>
		public override Texture2D GetDrawTexture()
		{
			Texture2D texture = mTexture;

			float vecAlongGrav = Vector2.Dot(GravityVecNorm(), mVelocity);

			if (mOnGround)
			{
				if (mWalkDirection != WalkDirection.None && mVelocity.LengthSquared() >= NOT_MOVING_SPEED * NOT_MOVING_SPEED)
				{
					texture = mRunningAnimation.GetCurrentTexture();
				}
				else if (mUseItemTimer.IsPlaying())
				{
					texture = mUseItemTex;
				}
			}
			else
			{
				if (vecAlongGrav >= 0.0f)
				{
					texture = mJumpDownTex;
				}
				else
				{
					texture = mJumpUpTex;
				}
			}

			return texture;
		}



		/// <summary>
		/// Colour to draw Arnold as
		/// </summary>
		/// <returns>Draw Colour</returns>
		public override Color GetDrawColor()
		{
			if (!mTimerSinceDeath.IsPlaying() && mTimerSinceStart.IsPlaying())
			{
				//TO DO
				double timeSinceStart = mTimerSinceStart.GetElapsedMs();

				if ((int)(timeSinceStart / FLASH_TIME) % 2 == 0)
				{
					return new Color(100, 255, 100);
				}
				else
				{
					return new Color(200, 255, 200);
				}
			}

			if(mReverseTimeFile is not null)
			{
				return new Color(60, 60, 60);
			}

			if(mBouncyMode)
			{
				return new Color(55, 148, 167);
			}

			if(IsChaosMode())
			{
				MonoRandom rng = RandomManager.I.GetDraw();
				return rng.GetColor();
			}

			return base.GetDrawColor();
		}



		/// <summary>
		/// Get draw layer for Arnold.
		/// </summary>
		public override DrawLayer GetDrawLayer()
		{
			return DrawLayer.Player;
		}



		/// <summary>
		/// Draw arnold
		/// </summary>
		/// <param name="info"></param>
		public override void Draw(DrawInfo info)
		{
			if (mOnGround && mUseItemTimer.IsPlaying())
			{
				// Texture
				Texture2D itemTex = mCurrItem.GetTexture();

				// Base position
				Vector2 itemPos = GetCentrePos() - GravityVecNorm() * 14.0f - new Vector2(itemTex.Width, itemTex.Height) * 0.5f;

				// Make item travel up a bit
				itemPos -= 6.0f * mUseItemTimer.GetPercentageF() * GravityVecNorm();

				float rot = Util.GetRotation(Util.InvertDirection(GetGravityDir()));

				MonoDraw.DrawTextureRotCent(info, itemTex, itemPos, rot);
			}
			else if(mCurrItem is not null && mCurrItem.IsActive())
			{
				mCurrItem.DrawOnArnold(info, this);
			}

			// Base actually draws arnold himself
			base.Draw(info);
		}

		#endregion rDraw





		#region rItem

		/// <summary>
		/// Can we use item
		/// </summary>
		public virtual bool CanUseItem()
		{
			return mOnGround;
		}



		/// <summary>
		/// Can we buy an item?
		/// </summary>
		public virtual bool CanBuyItem()
		{
			return mOnGround && !mUseItemTimer.IsPlaying();
		}


		/// <summary>
		/// Make Arnold bouncy.
		/// </summary>
		public void SetBouncyMode()
		{
			mBouncyMode = true;
		}

		#endregion rItem





		#region rUtility

		/// <summary>
		/// Send Death event.
		/// </summary>
		private void SendPlayerDeathEvent()
		{
			EventManager.I.TriggerEvent(EventType.PlayerDead);
		}



		/// <summary>
		/// Get collider bounds
		/// </summary>
		/// <returns>Collider bounds</returns>
		public override Rect2f ColliderBounds()
		{
			// Hack for horse mode
			if(mHorseMode)
			{
				Vector2 min = mPosition + new Vector2(2.0f, 3.0f);
				return new Rect2f(min, 18.0f, 20.0f);
			}

			if (GetGravityDir() == CardinalDirection.Left || GetGravityDir() == CardinalDirection.Right)
			{
				return new Rect2f(mPosition, mPosition + new Vector2(mTexture.Height, mTexture.Width));
			}

			return new Rect2f(mPosition, mPosition + new Vector2(mTexture.Width, mTexture.Height));
		}



		/// <summary>
		/// Calculate all the keys used for movement.
		/// </summary>
		private void UpdateArnoldKeys()
		{
			Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera);

			CardinalDirection camDir = Util.CardinalDirectionFromAngle(gameCam.GetCurrentSpec().mRotation);
			CardinalDirection gravityDir = GetGravityDir();

			// 16 cases
			switch ((camDir, gravityDir))
			{
				case (CardinalDirection.Up, CardinalDirection.Up):
					mLeftKey = InputAction.ArnoldLeft;
					mRightKey = InputAction.ArnoldRight;
					mDownKey = InputAction.ArnoldUp;
					break;
				case (CardinalDirection.Up, CardinalDirection.Right):
					mLeftKey = InputAction.ArnoldUp;
					mRightKey = InputAction.ArnoldDown;
					mDownKey = InputAction.ArnoldRight;
					break;
				case (CardinalDirection.Right, CardinalDirection.Left):
				case (CardinalDirection.Up, CardinalDirection.Down):
					mLeftKey = InputAction.ArnoldLeft;
					mRightKey = InputAction.ArnoldRight;
					mDownKey = InputAction.ArnoldDown;
					break;
				case (CardinalDirection.Up, CardinalDirection.Left):
					mLeftKey = InputAction.ArnoldUp;
					mRightKey = InputAction.ArnoldDown;
					mDownKey = InputAction.ArnoldLeft;
					break;
				case (CardinalDirection.Right, CardinalDirection.Up):
					break;
				case (CardinalDirection.Right, CardinalDirection.Right):
					break;
				case (CardinalDirection.Right, CardinalDirection.Down):
					break;
				case (CardinalDirection.Left, CardinalDirection.Right):
				case (CardinalDirection.Down, CardinalDirection.Up):
					mLeftKey = InputAction.ArnoldRight;
					mRightKey = InputAction.ArnoldLeft;
					mDownKey = InputAction.ArnoldDown;
					break;
				case (CardinalDirection.Down, CardinalDirection.Right):
					break;
				case (CardinalDirection.Down, CardinalDirection.Down):
					break;
				case (CardinalDirection.Down, CardinalDirection.Left):
					break;
				case (CardinalDirection.Left, CardinalDirection.Up):
					break;
				case (CardinalDirection.Left, CardinalDirection.Down):
					break;
				case (CardinalDirection.Left, CardinalDirection.Left):
					break;
			}
		}



		/// <summary>
		/// Hack to make ghost have the right texture
		/// </summary>
		public virtual GhostSkin GetGhostSkin()
		{
			return mLoadedSkin;
		}

		#endregion rUtility
	}
}
