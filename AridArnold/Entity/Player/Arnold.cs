using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Security.Authentication.ExtendedProtection;

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
		const double USE_ITEM_TIME = 600.0;
		const int COYOTE_TIME = 12;


		const float ARNOLD_WALK_SPEED = 9.0f;
		const float ARNOLD_GRAVITY = 4.35f;
		const float ARNOLD_JUMP_SPEED = 25.0f;
		const float ARNOLD_AIR_SPEED_BOOST = 0.015f;

		static Vector2 ITEM_OFFSET = new Vector2(-2.0f, -15.0f);

		#endregion rConstants





		#region rMembers

		protected Animator mRunningAnimation;

		protected Texture2D mJumpUpTex;
		protected Texture2D mJumpDownTex;
		protected Texture2D mUseItemTex;

		protected MonoTexturePack mYoungTexturePack;
		protected MonoTexturePack mOldTexturePack;

		//Various timers.
		PercentageTimer mTimerSinceDeath;
		protected PercentageTimer mTimerSinceStart;

		// Items
		Item mItemToUse;
		PercentageTimer mUseItemTimer;

		// Inputs(can change based on camera angle etc)
		AridArnoldKeys mLeftKey;
		AridArnoldKeys mRightKey;
		AridArnoldKeys mDownKey;

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

			mUseItemTimer = new PercentageTimer(USE_ITEM_TIME);

			mLeftKey = AridArnoldKeys.ArnoldLeft;
			mRightKey = AridArnoldKeys.ArnoldRight;
			mDownKey = AridArnoldKeys.ArnoldDown;

			mItemToUse = null;

			EventManager.I.AddListener(EventType.TimeChanged, OnTimeChange);
		}



		/// <summary>
		/// Load textures and assets
		/// </summary>
		public override void LoadContent()
		{
			InitTexturePacks();
			LoadTexturePack(mYoungTexturePack);

			//Botch position a bit. Not sure what's happening here.
			mPosition.Y -= 2.0f;

			mVelocity.Y = +0.01f;
		}



		/// <summary>
		/// Load young/old textures
		/// </summary>
		protected virtual void InitTexturePacks()
		{
			mYoungTexturePack = new MonoTexturePack("Arnold/YoungArnold.mtp");
			mOldTexturePack = new MonoTexturePack("Arnold/OldArnold.mtp");
		}



		/// <summary>
		/// Load textures from a pack
		/// </summary>
		void LoadTexturePack(MonoTexturePack texturePack)
		{
			mTexture = texturePack.GetTexture("Stand");
			mJumpUpTex = texturePack.GetTexture("JumpUp");
			mJumpDownTex = texturePack.GetTexture("JumpDown");
			mUseItemTex = texturePack.GetTexture("UseItem");

			mRunningAnimation = new Animator(Animator.PlayType.Repeat,
												(texturePack.GetTexture("Run1"), 0.1f),
												(texturePack.GetTexture("Run2"), 0.1f),
												(texturePack.GetTexture("Run3"), 0.1f),
												(texturePack.GetTexture("Run4"), 0.15f));

			mRunningAnimation.Play();
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

		#endregion rProperties





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

			//Item
			if(mUseItemTimer.IsPlaying())
			{
				if (mUseItemTimer.GetPercentage() == 1.0)
				{
					Item usingItem = ItemManager.I.PopActiveItem();
					MonoDebug.Assert(usingItem == mItemToUse);

					usingItem.UseItem(this);
					mItemToUse = usingItem;
					mUseItemTimer.FullReset();
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
			UpdateArnoldKeys();

			bool jump = InputManager.I.KeyHeld(AridArnoldKeys.ArnoldJump);
			bool fallthrough = InputManager.I.KeyHeld(mDownKey);

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

			// Items
			bool useItem = InputManager.I.KeyPressed(AridArnoldKeys.UseItem);
			if(CanUseItem() && useItem)
			{
				Item activeItem = ItemManager.I.GetActiveItem();
				if(activeItem is not null && activeItem.CanUseItem(this))
				{
					mUseItemTimer.Reset();
					mUseItemTimer.Start();
					mItemToUse = activeItem;
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
		void OnTimeChange(EArgs args)
		{
			RefreshTexturePack();
		}



		/// <summary>
		/// Refresh the textures so we update with age
		/// </summary>
		void RefreshTexturePack()
		{
			switch (TimeZoneManager.I.GetCurrentPlayerAge())
			{
				case 0:
					LoadTexturePack(mYoungTexturePack);
					break;
				case 1:
					LoadTexturePack(mOldTexturePack);
					break;
				default:
					throw new Exception("Invalid Arnold age");
			}
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
				else if(mUseItemTimer.IsPlaying())
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



		/// <summary>
		/// Draw arnold
		/// </summary>
		/// <param name="info"></param>
		public override void Draw(DrawInfo info)
		{
			if (mOnGround && mUseItemTimer.IsPlaying())
			{
				Vector2 itemPos = mPosition + ITEM_OFFSET;
				itemPos.Y -= 6.0f * mUseItemTimer.GetPercentageF();

				MonoDraw.DrawTextureDepth(info, mItemToUse.GetTexture(), itemPos, DrawLayer.Player);
			}

			// Base actually draws arnold himself
			base.Draw(info);
		}

		#endregion rDraw





		#region rItem

		/// <summary>
		/// Can we use item
		/// </summary>
		protected virtual bool CanUseItem()
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

		#endregion rItem





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
			EventManager.I.SendEvent(EventType.PlayerDead, new EArgs(this));
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
					mLeftKey = AridArnoldKeys.ArnoldLeft;
					mRightKey = AridArnoldKeys.ArnoldRight;
					mDownKey = AridArnoldKeys.ArnoldUp;
					break;
				case (CardinalDirection.Up, CardinalDirection.Right):
					mLeftKey = AridArnoldKeys.ArnoldUp;
					mRightKey = AridArnoldKeys.ArnoldDown;
					mDownKey = AridArnoldKeys.ArnoldRight;
					break;
				case (CardinalDirection.Right, CardinalDirection.Left):
				case (CardinalDirection.Up, CardinalDirection.Down):
					mLeftKey = AridArnoldKeys.ArnoldLeft;
					mRightKey = AridArnoldKeys.ArnoldRight;
					mDownKey = AridArnoldKeys.ArnoldDown;
					break;
				case (CardinalDirection.Up, CardinalDirection.Left):
					mLeftKey = AridArnoldKeys.ArnoldUp;
					mRightKey = AridArnoldKeys.ArnoldDown;
					mDownKey = AridArnoldKeys.ArnoldLeft;
					break;
				case (CardinalDirection.Right, CardinalDirection.Up):
					break;
				case (CardinalDirection.Right, CardinalDirection.Right):
					break;
				case (CardinalDirection.Right, CardinalDirection.Down):
					break;
				case (CardinalDirection.Left, CardinalDirection.Right):
				case (CardinalDirection.Down, CardinalDirection.Up):
					mLeftKey = AridArnoldKeys.ArnoldRight;
					mRightKey = AridArnoldKeys.ArnoldLeft;
					mDownKey = AridArnoldKeys.ArnoldDown;
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

		#endregion rUtility
	}
}
