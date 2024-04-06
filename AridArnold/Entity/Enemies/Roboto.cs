namespace AridArnold
{
	/// <summary>
	/// Robot powered by electricity
	/// </summary>
	internal class Roboto : AIEntity
	{
		#region rConstants

		const float ROBOTO_WIDTH_REDUCTION = 2.0f;
		const float ROBOTO_HEIGHT_REDUCTION = 2.0f;
		const float ROBOTO_JUMP_SPEED = 22.0f;
		const float ROBOTO_WALK_SPEED = 5.0f;
		const double ROBOTO_INT_TIME = 200.0;

		#endregion rConstants



		#region rMembers

		bool mPoweredOn;
		MonoTimer mTimeSinceOn;

		#endregion rMembers




		#region rInitialisation

		/// <summary>
		/// Load content for roboto.
		/// </summary>
		public Roboto(Vector2 pos) : base(pos, ROBOTO_WALK_SPEED, ROBOTO_JUMP_SPEED, ROBOTO_WIDTH_REDUCTION, ROBOTO_HEIGHT_REDUCTION)
		{
			mPoweredOn = false;
			mTimeSinceOn = new MonoTimer();
		}



		/// <summary>
		/// Load content for roboto.
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Enemies/Roboto/RobotoOff");
			mJumpUpTex = MonoData.I.MonoGameLoad<Texture2D>("Enemies/Roboto/RobotoWalk1");
			mJumpDownTex = MonoData.I.MonoGameLoad<Texture2D>("Enemies/Roboto/RobotoFall");

			mRunningAnimation = new Animator(Animator.PlayType.Repeat,
												("Enemies/Roboto/RobotoWalk1", 0.17f),
												("Enemies/Roboto/RobotoWalk2", 0.17f));
			mRunningAnimation.Play();

			mStandAnimation = new Animator(Animator.PlayType.OneShot,
											("Enemies/roboto/RobotoWalk1", 1.2f));
			mStandAnimation.Play();
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update roboto
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update(GameTime gameTime)
		{
			if (!mPoweredOn)
			{
				//Collider
				EntityManager.I.AddColliderSubmission(new EntityColliderSubmission(this));
			}
			else
			{
				mTimeSinceOn.Start();
			}

			if (CheckOffScreenDeath())
			{
				EntityManager.I.QueueDeleteEntity(this);
			}

			base.Update(gameTime);
		}



		/// <summary>
		/// Decide actions.
		/// </summary>
		protected override void DecideActions()
		{
			if (!mPoweredOn)
			{
				EMField emField = TileManager.I.GetEMField();

				EMField.ScanResults scanResults = emField.ScanAdjacent(mCentreOfMass);

				if (scanResults.mTotalPositiveElectric > 0.75f)
				{
					mPoweredOn = true;
				}

				mWalkDirection = WalkDirection.None;
			}
			else
			{
				if (mOnGround)
				{
					bool wallLeft = IsTouchingLeftWall();
					bool wallRight = IsTouchingRightWall();

					if (CheckSolid(1, 0) && CheckSolid(-1, 0))
					{
						mWalkDirection = WalkDirection.None;
					}
					else if (mWalkDirection == WalkDirection.None)
					{
						mWalkDirection = mPrevDirection;
					}
					else if (mWalkDirection == WalkDirection.Left)
					{
						if (wallLeft)
						{
							mWalkDirection = WalkDirection.Right;
						}
					}
					else if (mWalkDirection == WalkDirection.Right)
					{
						if (wallRight)
						{
							mWalkDirection = WalkDirection.Left;
						}
					}
				}
			}
		}


		/// <summary>
		/// Only kill the player if we are active.
		/// </summary>
		/// <param name="entity"></param>
		public override void OnCollideEntity(Entity entity)
		{
			if (mTimeSinceOn.GetElapsedMs() > ROBOTO_INT_TIME)
			{
				base.OnCollideEntity(entity);
			}
		}



		/// <summary>
		/// Turn the robot on.
		/// </summary>
		public void PowerOn()
		{
			mPoweredOn = true;
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get roboto texture depening on the state.
		/// </summary>
		public override Texture2D GetDrawTexture()
		{
			if (mPoweredOn == false)
			{
				return mTexture;
			}

			return base.GetDrawTexture();
		}

		#endregion rDraw
	}
}
