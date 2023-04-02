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

		#endregion rConstants



		#region rMembers

		bool mPoweredOn;

		#endregion rMembers




		#region rInitialisation

		/// <summary>
		/// Load content for roboto.
		/// </summary>
		public Roboto(Vector2 pos) : base(pos, ROBOTO_WALK_SPEED, ROBOTO_JUMP_SPEED, ROBOTO_WIDTH_REDUCTION, ROBOTO_HEIGHT_REDUCTION)
		{
			mPoweredOn = false;
		}



		/// <summary>
		/// Load content for roboto.
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Enemies/roboto/roboto-off");
			mJumpUpTex = MonoData.I.MonoGameLoad<Texture2D>("Enemies/roboto/roboto-walk0");
			mJumpDownTex = MonoData.I.MonoGameLoad<Texture2D>("Enemies/roboto/roboto-fall");

			mRunningAnimation = new Animator(Animator.PlayType.Repeat,
												("Enemies/roboto/roboto-walk0", 0.17f),
												("Enemies/roboto/roboto-walk1", 0.17f));
			mRunningAnimation.Play();

			mStandAnimation = new Animator(Animator.PlayType.OneShot,
											("Enemies/roboto/roboto-walk0", 1.2f));
			mStandAnimation.Play();

			//Botch position a bit. Not sure what's happening here.
			mPosition.Y += 0.0f;
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update roboto
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update(GameTime gameTime)
		{
			if(!mPoweredOn)
			{
				//Collider
				EntityManager.I.AddColliderSubmission(new EntityColliderSubmission(this));
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
					bool wallLeft = CheckSolid(-1, 0);
					bool wallRight = CheckSolid(1, 0);

					if (wallLeft && wallRight)
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
		public override void CollideWithEntity(Entity entity)
		{
			if(mPoweredOn)
			{
				base.CollideWithEntity(entity);
			}
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get roboto texture depening on the state.
		/// </summary>
		protected override Texture2D GetDrawTexture()
		{
			if(mPoweredOn == false)
			{
				return mTexture;
			}

			return base.GetDrawTexture();
		}

		#endregion rDraw
	}
}
