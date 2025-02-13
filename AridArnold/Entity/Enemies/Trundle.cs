﻿namespace AridArnold
{
	/// <summary>
	/// Rat like creature with simple movement.
	/// </summary>
	internal class Trundle : AIEntity
	{
		#region rTypes

		enum State
		{
			Wait,
			WalkRight,
			WalkLeft,
			ChargeAtPlayer,
			Jump
		}

		#endregion rTypes





		#region rConstants

		const float TRUNDLE_WIDTH_REDUCTION = 6.0f;
		const float TRUNDLE_HEIGHT_REDUCTION = 3.0f;
		const float TRUNDLE_JUMP_SPEED = 22.0f;
		const float TRUNDLE_WALK_SPEED = 4.5f;

		#endregion rConstants





		#region rMembers

		StateMachine<State> mStateMachine;
		RandomSFXFactory mSFXFactory;
		SpacialSFX mCurrentSqueak = null;

		#endregion rMembers





		#region rInitialise

		/// <summary>
		/// Trundle constructor
		/// </summary>
		/// <param name="pos">Starting position</param>
		public Trundle(Vector2 pos) : base(pos + new Vector2(0.0f, 9.0f), TRUNDLE_WALK_SPEED, TRUNDLE_JUMP_SPEED, TRUNDLE_WIDTH_REDUCTION, TRUNDLE_HEIGHT_REDUCTION)
		{
			mStateMachine = new StateMachine<State>(State.Wait);

			SetDustIntensity(10.0f);
		}



		/// <summary>
		/// Load trundle textures and animations
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Enemies/Trundle/TrundleStand1");
			mJumpUpTex = MonoData.I.MonoGameLoad<Texture2D>("Enemies/Trundle/TrundleJumpUp");
			mJumpDownTex = MonoData.I.MonoGameLoad<Texture2D>("Enemies/Trundle/TrundleJumpDown");

			mRunningAnimation = new Animator(Animator.PlayType.Repeat,
												("Enemies/Trundle/TrundleWalk1", 0.12f),
												("Enemies/Trundle/TrundleWalk2", 0.15f),
												("Enemies/Trundle/TrundleWalk3", 0.12f),
												("Enemies/Trundle/TrundleWalk4", 0.12f));
			mRunningAnimation.Play();

			mStandAnimation = new Animator(Animator.PlayType.Repeat,
											("Enemies/Trundle/TrundleStand1", 0.4f),
											("Enemies/Trundle/TrundleStand2", 0.7f),
											("Enemies/Trundle/TrundleStand4", 0.5f),
											("Enemies/Trundle/TrundleStand3", 0.8f));
			mStandAnimation.Play();

			//Botch position a bit. Not sure what's happening here.
			mPosition.Y -= 2.0f;

			// SFX
			mSFXFactory = new RandomSFXFactory();
			mSFXFactory.AddSFX(AridArnoldSFX.RatSqueak1, 0.1f, -0.3f, 0.0f);
			mSFXFactory.AddSFX(AridArnoldSFX.RatSqueak2, 0.1f, -0.3f, 0.0f);
			mSFXFactory.AddSFX(AridArnoldSFX.RatSqueak3, 0.1f, -0.3f, 0.0f);
			mSFXFactory.AddSFX(AridArnoldSFX.RatSqueak4, 0.1f, -0.3f, 0.0f);
			mSFXFactory.AddSFX(AridArnoldSFX.RatSqueak5, 0.1f, -0.3f, 0.0f);
			mSFXFactory.AddSFX(AridArnoldSFX.RatSqueak6, 0.1f, -0.3f, 0.0f);
			mSFXFactory.AddSFX(AridArnoldSFX.RatSqueak7, 0.1f, -0.3f, 0.0f);

			SFXFactory jumpFactory = new SFXFactory(AridArnoldSFX.ArnoldJump, 0.84f, 0.8f, 0.9f);
			LoadSFX(jumpFactory, AridArnoldSFX.ArnoldWalk, 0.2f, 0.8f);
		}

		#endregion rInitialise





		#region rUpdate

		/// <summary>
		/// Update trundle
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mStateMachine.Update(gameTime);

			if (mCurrentSqueak is not null)
			{
				mCurrentSqueak.SetPosition(GetPos());
				mCurrentSqueak.SetVelocity(GetVelocity());

				if(mCurrentSqueak.GetBuffer().GetState() == SoundState.Stopped)
				{
					mCurrentSqueak = null;
				}
			}
			else if(mStateMachine.GetState() == State.Wait)
			{
				MonoRandom rng = RandomManager.I.GetDraw();
				if(rng.PercentChance(1.0f))
				{
					mCurrentSqueak = mSFXFactory.CreateSpacialSFX(mPosition);
					SFXManager.I.PlaySFX(mCurrentSqueak);
				}
			}

			base.Update(gameTime);

		}

		#endregion rUpdate





		#region rAIDecide

		/// <summary>
		/// Decide what state to go in.
		/// </summary>
		protected override void DecideActions()
		{
			if (CanWalkDirChange())
			{
				if (mRandom.PercentChance(0.15f))
				{
					mStateMachine.ForceGoToStateAndWait(State.Wait, 3500.0f);
				}

				if (KeepWalking() == false)
				{
					TryJumpUp();
					TryWalking();
				}
			}

			EnforceState();
		}



		/// <summary>
		/// Check if we can enter the jump state.
		/// </summary>
		void TryJumpUp()
		{
			if (mPrevDirection == WalkDirection.Left)
			{
				if (!CheckSolid(-1, -1) && CheckSolid(-1, 0))
				{
					mWalkDirection = WalkDirection.Left;
					mStateMachine.GoToStateAndWait(State.Jump, 100.0);
				}
			}
			else if (mPrevDirection == WalkDirection.Right)
			{
				if (!CheckSolid(1, -1) && CheckSolid(1, 0))
				{
					mWalkDirection = WalkDirection.Right;
					mStateMachine.GoToStateAndWait(State.Jump, 100.0);
				}
			}
		}



		/// <summary>
		/// Decide which direction to walk in
		/// </summary>
		void TryWalking()
		{
			bool canWalkLeft = CanWalkInDir(WalkDirection.Left);
			bool canWalkRight = CanWalkInDir(WalkDirection.Right);

			if (mRandom.PercentChance(10.0f))
			{
				mStateMachine.GoToStateAndWait(State.Wait, 1500.0f);
			}

			if (canWalkLeft && canWalkRight)
			{
				if (mPrevDirection == WalkDirection.Left)
				{
					mStateMachine.GoToStateAndWaitForever(State.WalkLeft);
				}
				else
				{
					mStateMachine.GoToStateAndWaitForever(State.WalkRight);
				}
			}
			else if (canWalkLeft)
			{
				mStateMachine.GoToStateAndWaitForever(State.WalkLeft);
			}
			else if (canWalkRight)
			{
				mStateMachine.GoToStateAndWaitForever(State.WalkRight);
			}
		}



		/// <summary>
		/// Check if we can keep walking.
		/// </summary>
		/// <returns>True if we can keep walking.</returns>
		bool KeepWalking()
		{
			if (!CanWalkDirChange())
			{
				return false;
			}

			State currState = mStateMachine.GetState();
			if (mWalkDirection == WalkDirection.Right || currState == State.WalkRight)
			{
				if (!CanWalkInDir(WalkDirection.Right))
				{
					mStateMachine.ForceGoToStateAndWait(State.Wait, 500.0);
					return false;
				}
				else
				{
					return true;
				}
			}
			else if (mWalkDirection == WalkDirection.Left || currState == State.WalkLeft)
			{
				if (!CanWalkInDir(WalkDirection.Left))
				{
					mStateMachine.ForceGoToStateAndWait(State.Wait, 500.0);
					return false;
				}
				else
				{
					return true;
				}
			}

			return false;
		}



		/// <summary>
		/// Can we walk in a certain direction.
		/// </summary>
		/// <param name="dir">Walk direction we wish to check.</param>
		/// <returns>True if we can walk in the given direction</returns>
		bool CanWalkInDir(WalkDirection dir)
		{
			const float GRADIENT = 1.952342523523f;

			if (!CanWalkDirChange())
			{
				return false;
			}

			Vector2 currentPosition = TileManager.I.RoundToTileCentre(mCentreOfMass);
			Vector2 stepDir = Vector2.Zero;

			switch (dir)
			{
				case WalkDirection.Left:
					if (CheckSolid(-1, 0))
					{
						return false;
					}

					if (CheckSolid(-1, 1))
					{
						return true;
					}

					stepDir = new Vector2(-1.0f / GRADIENT, 1.0f);
					break;
				case WalkDirection.Right:
					if (CheckSolid(1, 0))
					{
						return false;
					}

					if (CheckSolid(1, 1))
					{
						return true;
					}

					stepDir = new Vector2(1.0f / GRADIENT, 1.0f);
					break;
				case WalkDirection.None:
					return true;
			}

			// Found air in front of us
			stepDir = stepDir * TileManager.I.GetTileSize();
			Rect2f tileMapRect = TileManager.I.GetTileMapRectangle();

			// Step through the air and see if we will land safely. 
			while (Collision2D.BoxVsPoint(tileMapRect, currentPosition))
			{
				currentPosition += stepDir;

				Point tilePoint = TileManager.I.GetTileMapCoord(currentPosition);
				bool currentTileSolid = TileManager.I.GetTile(tilePoint).IsSolid();
				bool aboveTileSolid = TileManager.I.GetTile(tilePoint.X, tilePoint.Y - 1).IsSolid();

				if (currentTileSolid && !aboveTileSolid)
				{
					return true;
				}

				if (currentTileSolid && aboveTileSolid)
				{
					currentPosition.X -= stepDir.X;

					tilePoint = TileManager.I.GetTileMapCoord(currentPosition);
					currentTileSolid = TileManager.I.GetTile(tilePoint).IsSolid();
					aboveTileSolid = TileManager.I.GetTile(tilePoint.X, tilePoint.Y - 1).IsSolid();

					if (currentTileSolid && !aboveTileSolid)
					{
						return true;
					}
				}
			}

			return false;
		}

		#endregion rAIDecide





		#region rPerformActions

		/// <summary>
		/// Perform action indicated by the state.
		/// </summary>
		void EnforceState()
		{
			State currentState = mStateMachine.GetState();

			switch (currentState)
			{
				case State.Wait:
					if (CanWalkDirChange())
						mWalkDirection = WalkDirection.None;
					break;
				case State.WalkRight:
					if (CanWalkDirChange())
						mWalkDirection = WalkDirection.Right;
					break;
				case State.WalkLeft:
					if (CanWalkDirChange())
						mWalkDirection = WalkDirection.Left;
					break;
				case State.ChargeAtPlayer:
					ChargeAtPlayer();
					break;
				case State.Jump:
					HandleJumpState();
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// [NOT USED] Find the player and walk in their direction.
		/// </summary>
		void ChargeAtPlayer()
		{
			int entityNum = EntityManager.I.GetEntityNum();

			Arnold closestArnold = null;
			float closestDist = float.MaxValue;

			for (int i = 0; i < entityNum; i++)
			{
				Entity entity = EntityManager.I.GetEntity(i);

				if (entity is Arnold)
				{
					if (closestArnold == null)
					{
						closestArnold = (Arnold)entity;
					}
					else
					{
						float dist = (mPosition - entity.GetPos()).LengthSquared();
						if (dist < closestDist)
						{
							closestDist = dist;
							closestArnold = (Arnold)entity;
						}
					}
				}
			}

			float dx = closestArnold.GetPos().X - mPosition.X;

			if (Math.Abs(dx) < 32.0f)
			{
				mStateMachine.ForceGoToStateAndWait(State.Jump, 500.0f);
			}
			else if (dx < 0.0f)
			{
				mWalkDirection = WalkDirection.Left;
			}
			else
			{
				mWalkDirection = WalkDirection.Right;
			}

		}

		/// <summary>
		/// Jump and then get out of the jump state.
		/// </summary>
		void HandleJumpState()
		{
			if (mOnGround)
			{
				Jump();
				mStateMachine.ForceGoToStateAndWait(State.Wait, 500.0f);
			}
		}

		#endregion rPerformActions
	}
}
