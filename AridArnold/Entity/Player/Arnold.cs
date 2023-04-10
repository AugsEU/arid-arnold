namespace AridArnold
{
	/// <summary>
	/// The playable character, our hero, our saviour, Arnold.
	/// </summary>
	class Arnold : PlatformingEntity
	{
		#region rConstants

		const double DEATH_TIME = 500.0;
		const double START_TIME = 500.0;
		const double FLASH_TIME = 100.0;
		const int COYOTE_TIME = 12;


		const float ARNOLD_WALK_SPEED = 9.0f;
		const float ARNOLD_GRAVITY = 4.35f;
		const float ARNOLD_JUMP_SPEED = 25.0f;
		const float ARNOLD_AIR_SPEED_BOOST = 0.015f;

		#endregion rConstants





		#region rMembers

		protected Animator mRunningAnimation;

		protected Texture2D mJumpUpTex;
		protected Texture2D mJumpDownTex;

		//Various timers.
		PercentageTimer mTimerSinceDeath;
		protected PercentageTimer mTimerSinceStart;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Construct Arnold from position
		/// </summary>
		/// <param name="pos">Starting position</param>
		public Arnold(Vector2 pos) : base(pos, ARNOLD_WALK_SPEED, ARNOLD_JUMP_SPEED, ARNOLD_GRAVITY)
		{
			mPrevDirection = WalkDirection.Right;

			EventManager.I.AddListener(EventType.KillPlayer, SignalPlayerDead);

			mTimerSinceDeath = new PercentageTimer(DEATH_TIME);
			mTimerSinceStart = new PercentageTimer(START_TIME);
		}



		/// <summary>
		/// Load textures and assets
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Arnold/arnold-stand");
			mJumpUpTex = MonoData.I.MonoGameLoad<Texture2D>("Arnold/arnold-jump-up");
			mJumpDownTex = MonoData.I.MonoGameLoad<Texture2D>("Arnold/arnold-jump-down");

			mRunningAnimation = new Animator(Animator.PlayType.Repeat,
												("Arnold/arnold-run0", 0.1f),
												("Arnold/arnold-run1", 0.1f),
												("Arnold/arnold-run2", 0.1f),
												("Arnold/arnold-run3", 0.15f));

			mRunningAnimation.Play();

			//Botch position a bit. Not sure what's happening here.
			mPosition.Y -= 2.0f;

			mVelocity.Y = +0.01f;
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update Arnold
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public override void Update(GameTime gameTime)
		{
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
				if (mTimerSinceDeath.GetPercentage() == 1.0)
				{
					SendPlayerDeathEvent();
					mTimerSinceDeath.FullReset();
				}

				return;
			}

			//Anim
			mRunningAnimation.Update(gameTime);

			//Collider
			EntityManager.I.AddColliderSubmission(new EntityColliderSubmission(this));

			if (mOnGround == false)
			{
				SetPrevWalkDirFromVelocity();
				mWalkSpeed = ARNOLD_WALK_SPEED + ARNOLD_AIR_SPEED_BOOST;
			}
			else
			{
				mWalkSpeed = ARNOLD_WALK_SPEED;
			}

			//Input
			DoInputs(gameTime);

			if (CheckOffScreenDeath())
			{
				Kill();
			}

			base.Update(gameTime);
		}


		/// <summary>
		/// Handle inputs
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		private void DoInputs(GameTime gameTime)
		{
			bool jump = InputManager.I.KeyHeld(AridArnoldKeys.ArnoldJump);
			bool fallthrough = InputManager.I.KeyHeld(GetFallthroughKey());

			if (jump && fallthrough)
			{
				if (!(mOnGround && mWalkDirection != WalkDirection.None))
				{
					FallThroughPlatforms();
				}
			}
			else if (mOnGround)
			{
				if (jump)
				{
					Jump();
				}

				HandleWalkInput();
			}
			else if (IsGroundedSince(COYOTE_TIME))
			{
				if (jump && mVelocity.Y > 0.0f)
				{
					Jump();
				}
			}
		}



		/// <summary>
		/// Check for walk inputs
		/// </summary>
		private void HandleWalkInput()
		{
			CardinalDirection gravDir = GetGravityDir();

			if (gravDir == CardinalDirection.Down || gravDir == CardinalDirection.Up)
			{
				if (InputManager.I.KeyHeld(AridArnoldKeys.ArnoldLeft))
				{
					mWalkDirection = WalkDirection.Left;
					mPrevDirection = mWalkDirection;
				}
				else if (InputManager.I.KeyHeld(AridArnoldKeys.ArnoldRight))
				{
					mWalkDirection = WalkDirection.Right;
					mPrevDirection = mWalkDirection;
				}
				else
				{
					mWalkDirection = WalkDirection.None;
				}
			}
			else if (gravDir == CardinalDirection.Left || gravDir == CardinalDirection.Right)
			{
				if (InputManager.I.KeyHeld(AridArnoldKeys.ArnoldUp))
				{
					mWalkDirection = WalkDirection.Left;
					mPrevDirection = mWalkDirection;
				}
				else if (InputManager.I.KeyHeld(AridArnoldKeys.ArnoldDown))
				{
					mWalkDirection = WalkDirection.Right;
					mPrevDirection = mWalkDirection;
				}
				else
				{
					mWalkDirection = WalkDirection.None;
				}
			}
		}



		/// <summary>
		/// Don't update if the death timer is playing.
		/// </summary>
		/// <param name="gameTime"></param>
		public override void OrderedUpdate(GameTime gameTime)
		{
			if (mTimerSinceDeath.IsPlaying())
			{
				return;
			}

			base.OrderedUpdate(gameTime);
		}


		/// <summary>
		/// Deal with things touching us.
		/// </summary>
		public override void CollideWithEntity(Entity entity)
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

			base.CollideWithEntity(entity);
		}



		/// <summary>
		/// Check if we died from going off-screen
		/// </summary>
		/// <returns>True if we should die</returns>
		/// <exception cref="NotImplementedException">Required valid cardinal direction</exception>
		private bool CheckOffScreenDeath()
		{
			switch (GetGravityDir())
			{
				case CardinalDirection.Up:
					return mPosition.Y < -mTexture.Height / 2.0f;
				case CardinalDirection.Right:
					return mPosition.X > TileManager.I.GetDrawWidth() + 2.0f * mTexture.Width;
				case CardinalDirection.Down:
					return mPosition.Y > TileManager.I.GetDrawHeight() + mTexture.Height / 2.0f;
				case CardinalDirection.Left:
					return mPosition.X < -mTexture.Width / 2.0f;
			}

			throw new NotImplementedException();
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get Arnold texture
		/// </summary>
		protected override Texture2D GetDrawTexture()
		{
			Texture2D texture = mTexture;

			float vecAlongGrav = Vector2.Dot(GravityVecNorm(), mVelocity);

			if (mOnGround)
			{
				if (mWalkDirection != WalkDirection.None && mVelocity.LengthSquared() >= mWalkSpeed)
				{
					texture = mRunningAnimation.GetCurrentTexture();
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
		protected override Color GetDrawColor()
		{
			if (mTimerSinceDeath.IsPlaying())
			{
				double timeSinceDeath = mTimerSinceDeath.GetElapsedMs();

				if ((int)(timeSinceDeath / FLASH_TIME) % 2 == 0)
				{
					return new Color(255, 51, 33);
				}
				else
				{
					return new Color(255, 128, 79);
				}
			}
			else if (mTimerSinceStart.IsPlaying())
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

			return Color.White;
		}



		/// <summary>
		/// Get draw layer for Arnold.
		/// </summary>
		protected override DrawLayer GetDrawLayer()
		{
			return DrawLayer.Player;
		}

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Kill Arnold
		/// </summary>
		public void Kill()
		{
			mTimerSinceDeath.Start();
		}



		/// <summary>
		/// Send Death event. TO DO: I don't think this is needed, why not just call Kill() directly?
		/// </summary>
		private void SendPlayerDeathEvent()
		{
			EArgs eArgs;
			eArgs.sender = this;

			EventManager.I.SendEvent(EventType.PlayerDead, eArgs);
		}



		/// <summary>
		/// Callback for the player death event.
		/// </summary>
		/// <param name="args">Event sender args</param>
		public void SignalPlayerDead(EArgs args)
		{
			Kill();
		}



		/// <summary>
		/// Get collider bounds
		/// </summary>
		/// <returns>Collider bounds</returns>
		public override Rect2f ColliderBounds()
		{
			if (GetGravityDir() == CardinalDirection.Left || GetGravityDir() == CardinalDirection.Right)
			{
				return new Rect2f(mPosition, mPosition + new Vector2(mTexture.Height, mTexture.Width));
			}

			return new Rect2f(mPosition, mPosition + new Vector2(mTexture.Width, mTexture.Height));
		}



		/// <summary>
		/// Get key needed to fall through platforms
		/// TO DO: Use input manager
		/// </summary>
		/// <returns></returns>
		/// <exception cref="NotImplementedException">Requires valid cardinal direction</exception>
		private AridArnoldKeys GetFallthroughKey()
		{
			switch (GetGravityDir())
			{
				case CardinalDirection.Down:
					return AridArnoldKeys.ArnoldDown;
				case CardinalDirection.Up:
					return AridArnoldKeys.ArnoldUp;
				case CardinalDirection.Left:
					return AridArnoldKeys.ArnoldLeft;
				case CardinalDirection.Right:
					return AridArnoldKeys.ArnoldRight;
			}

			throw new NotImplementedException();
		}

		#endregion rUtility
	}
}
