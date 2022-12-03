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

		#endregion rConstants





		#region rMembers

		protected WalkDirection mPrevDirection;
		protected Animator mRunningAnimation;

		protected Texture2D mJumpUpTex;
		protected Texture2D mJumpDownTex;

		int mTimeSinceGrounded;

		//Various timers.
		PercentageTimer mTimerSinceDeath;
		PercentageTimer mTimerSinceStart;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Construct Arnold from position
		/// </summary>
		/// <param name="pos">Starting position</param>
		public Arnold(Vector2 pos) : base(pos)
		{
			mPrevDirection = WalkDirection.Right;

			EventManager.I.AddListener(EventType.KillPlayer, SignalPlayerDead);

			mTimerSinceDeath = new PercentageTimer(DEATH_TIME);
			mTimerSinceStart = new PercentageTimer(START_TIME);

			mTimeSinceGrounded = int.MaxValue;
		}



		/// <summary>
		/// Load textures and assets
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public override void LoadContent(ContentManager content)
		{
			mTexture = content.Load<Texture2D>("Arnold/arnold-stand");
			mJumpUpTex = content.Load<Texture2D>("Arnold/arnold-jump-up");
			mJumpDownTex = content.Load<Texture2D>("Arnold/arnold-jump-down");

			mRunningAnimation = new Animator(content, Animator.PlayType.Repeat,
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

			if (mOnGround)
			{
				mTimeSinceGrounded = 0;
			}

			//Anim
			mRunningAnimation.Update(gameTime);

			if (mOnGround == false)
			{
				SetDirFromVelocity();
			}

			//Input
			DoInputs(gameTime);

			if (CheckOffScreenDeath())
			{
				Kill();
			}

			base.Update(gameTime);

			if (mTimeSinceGrounded != int.MaxValue)
			{
				mTimeSinceGrounded++;
			}
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
					mTimeSinceGrounded = int.MaxValue;
				}

				HandleWalkInput();
			}
			else if (mTimeSinceGrounded < COYOTE_TIME)
			{
				if (jump)
				{
					Jump();
					mTimeSinceGrounded = int.MaxValue;
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
		/// Set previous walk direction from the velocity we have. Prevents jumping backwards with mirrors.
		/// </summary>
		protected void SetDirFromVelocity()
		{
			const float AIR_DIR_THRESH = 0.25f;
			switch (GetGravityDir())
			{
				case CardinalDirection.Down:
				case CardinalDirection.Up:
					if (mVelocity.X > AIR_DIR_THRESH)
					{
						mPrevDirection = WalkDirection.Right;
					}
					else if (mVelocity.X < -AIR_DIR_THRESH)
					{
						mPrevDirection = WalkDirection.Left;
					}
					break;
				case CardinalDirection.Right:
				case CardinalDirection.Left:
					if (mVelocity.Y > AIR_DIR_THRESH)
					{
						mPrevDirection = WalkDirection.Right;
					}
					else if (mVelocity.Y < -AIR_DIR_THRESH)
					{
						mPrevDirection = WalkDirection.Left;
					}
					break;
			}
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


		/// <summary>
		/// Get collider submission for this frame. Entities that want to collide will have to submit one.
		/// </summary>
		public override ColliderSubmission GetColliderSubmission()
		{
			return new EntityColliderSubmission(this);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw Arnold
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		public override void Draw(DrawInfo info)
		{
			SpriteEffects effect = SpriteEffects.None;

			if (mPrevDirection == WalkDirection.Left)
			{
				effect = SpriteEffects.FlipHorizontally;
			}

			Texture2D texture = mTexture;

			float vecAlongGrav = Vector2.Dot(GravityVecNorm(), mVelocity);

			if (mOnGround)
			{
				if (mVelocity.LengthSquared() > 0.001f)
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

			int xDiff = texture.Width - mTexture.Width;
			int yDiff = texture.Height - mTexture.Height;

			float rotation = 0.0f;

			switch (GetGravityDir())
			{
				case CardinalDirection.Down:
					rotation = 0.0f;
					xDiff = xDiff / 2;
					break;
				case CardinalDirection.Up:
					rotation = MathHelper.Pi;
					yDiff = 1;
					xDiff = xDiff / 2;
					effect = effect ^ SpriteEffects.FlipHorizontally;
					break;
				case CardinalDirection.Left:
					Util.Swap(ref xDiff, ref yDiff);
					xDiff = -2 - xDiff / 2;
					yDiff += 2;

					rotation = MathHelper.PiOver2;
					break;
				case CardinalDirection.Right:
					rotation = MathHelper.PiOver2 * 3.0f;
					effect = effect ^ SpriteEffects.FlipHorizontally;

					Util.Swap(ref xDiff, ref yDiff);
					xDiff = (int)(xDiff / 1.5f) - 1;
					yDiff += 2;

					break;
			}

			Vector2 rotationOffset = MonoDraw.CalcRotationOffset(rotation, texture.Width, texture.Height);

			MonoDraw.DrawTexture(info, texture, new Rectangle((int)MathF.Round(mPosition.X) - xDiff, (int)mPosition.Y + 1 - yDiff, texture.Width, texture.Height), null, GetDrawColour(), rotation, rotationOffset, effect, MonoDraw.LAYER_DEFAULT);
		}



		/// <summary>
		/// Colour to draw Arnold as
		/// </summary>
		/// <returns>Draw Colour</returns>
		protected virtual Color GetDrawColour()
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

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Kill Arnold
		/// </summary>
		public override void Kill()
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

			base.Kill();
		}



		/// <summary>
		/// Callback for the player death event.
		/// </summary>
		/// <param name="args">Event sender args</param>
		public virtual void SignalPlayerDead(EArgs args)
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
