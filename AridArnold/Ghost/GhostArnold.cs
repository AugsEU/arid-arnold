namespace AridArnold
{
	/// <summary>
	/// Ghost of Arnold's fastest attempt.
	/// </summary>
	class GhostArnold : Arnold
	{
		#region rMembers

		GhostSkin mCurrSkin;
		MonoTexturePack mRobotTexturePack;

		#endregion rMembers


		#region rInitialisation

		/// <summary>
		/// Constructor 
		/// </summary>
		/// <param name="startPos">Starting position</param>
		public GhostArnold(Vector2 startPos) : base(startPos)
		{
			mCurrSkin = GetGhostSkin();
		}



		/// <summary>
		/// Start level with default parameters
		/// </summary>
		public void StartLevel()
		{
			mPrevDirection = WalkDirection.Right;
			mVelocity = Vector2.Zero;
		}


		protected override void InitTexturePacks()
		{
			mRobotTexturePack = new MonoTexturePack("Arnold/Androld.mtp");
			base.InitTexturePacks();
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update Ghost Arnold
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public override void Update(GameTime gameTime)
		{
			mRunningAnimation.Update(gameTime);
		}

		/// <summary>
		/// Physics update
		/// </summary>
		public override void OrderedUpdate(GameTime gameTime)
		{
			// None physics.
			return;
		}

		#endregion rUpdate





		#region rUtility

		/// <summary>
		/// Set ghost info for this frame.
		/// </summary>
		/// <param name="info"></param>
		public void SetGhostInfo(GhostInfo info)
		{
			mPosition = info.mPosition;

			mVelocity = info.mVelocity;
			mOnGround = info.mGrounded;
			SetWalkDirection(info.mWalkDirection);
			SetPrevWalkDirection(info.mPrevWalkDirection);
			SetGravity(info.mGravity);
			SetEnabled(info.mEnabled);

			if(info.mSkin != mCurrSkin)
			{
				mCurrSkin = info.mSkin;
				RefreshTexturePack();
			}
		}



		/// <summary>
		/// Get colour to draw this ghost as.
		/// </summary>
		/// <returns></returns>
		public override Color GetDrawColor()
		{
			//Slight green.
			return new Color(0.0f, 0.7f, 0.0f, 0.9f);
		}


		/// <summary>
		/// Refresh texture packs.
		/// </summary>
		protected override void RefreshTexturePack()
		{
			switch (mCurrSkin)
			{
				case GhostSkin.kYoung:
					LoadTexturePack(mYoungTexturePack);
					break;
				case GhostSkin.kOld:
					LoadTexturePack(mOldTexturePack);
					break;
				case GhostSkin.kRobot:
					LoadTexturePack(mRobotTexturePack);
					break;
				case GhostSkin.kHorse:
					LoadTexturePack(mHorseTexturePack);
					break;
				default:
					break;
			}
		}

		#endregion
	}
}
