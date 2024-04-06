namespace AridArnold
{

	/// <summary>
	/// Generic class for an AI entity in the game.
	/// </summary>
	internal abstract class AIEntity : PlatformingEntity
	{
		#region rMembers

		private float mCWidthReduction;
		private float mCHeightReduction;


		protected Animator mRunningAnimation;
		protected Animator mStandAnimation;

		protected Texture2D mJumpUpTex;
		protected Texture2D mJumpDownTex;

		protected MonoRandom mRandom;
		int mStartSeed;
		int mFrameNum;

		#endregion





		#region rInitialise

		/// <summary>
		/// AIEntity constructor
		/// </summary>
		/// <param name="pos">Starting position of the entity</param>
		/// <param name="walkSpeed">Walk speed of the entity</param>
		/// <param name="jumpSpeed">Initial upwards velocity when jumping</param>
		/// <param name="widthReduction">Hitbox width reduction compared to sprite size</param>
		/// <param name="heightReduction">Hitbox height reduction compared to sprite size</param>
		public AIEntity(Vector2 pos, float walkSpeed, float jumpSpeed, float widthReduction, float heightReduction) : base(pos, walkSpeed, jumpSpeed)
		{
			mRandom = new MonoRandom();

			mRandom.SetSeed(0);
			mRandom.ChugNumber((int)pos.X);
			mRandom.ChugNumber((int)pos.Y);

			mStartSeed = mRandom.Next();

			mPrevDirection = mRandom.PercentChance(50.0f) ? WalkDirection.Left : WalkDirection.Right;
			mWalkDirection = WalkDirection.None;

			mCWidthReduction = widthReduction;
			mCHeightReduction = heightReduction;

			mFrameNum = 0;
		}

		#endregion rInitialise





		#region rUpdate

		/// <summary>
		/// Update the AI Entity
		/// </summary>
		/// <param name="gameTime">Frame time.</param>
		public override void Update(GameTime gameTime)
		{
			mFrameNum++;
			ChugRandom();

			mRunningAnimation.Update(gameTime);
			mStandAnimation.Update(gameTime);

			DecideActions();

			if (mWalkDirection != WalkDirection.None)
			{
				mPrevDirection = mWalkDirection;
			}

			base.Update(gameTime);
		}



		/// <summary>
		/// Calculate random seed value for this frame based on deterministic variables.
		/// </summary>
		private void ChugRandom()
		{
			//Make random deterministic based on player movement.
			mRandom.SetSeed(mStartSeed);
			mRandom.ChugNumber(mFrameNum / 15);

			int entityNum = EntityManager.I.GetEntityNum();

			for (int i = 0; i < entityNum; i++)
			{
				Entity entity = EntityManager.I.GetEntity(i);

				if (entity is Arnold)
				{
					Vector2 pos = entity.GetPos();

					mRandom.ChugNumber((int)(pos.X / 128.0f));
					mRandom.ChugNumber((int)(pos.Y / 128.0f));
				}
			}
		}



		/// <summary>
		/// React to collision with an entity.
		/// </summary>
		/// <param name="entity"></param>
		public override void OnCollideEntity(Entity entity)
		{
			if (ShouldKill() && entity is Arnold)
			{
				//Kill the player on touching.
				entity.Kill();
			}

			base.OnCollideEntity(entity);
		}



		/// <summary>
		/// Should we kill on touch?
		/// </summary>
		protected virtual bool ShouldKill()
		{
			return true;
		}



		/// <summary>
		/// Decide what to do.
		/// </summary>
		protected abstract void DecideActions();

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get texture we should draw.
		/// </summary>
		/// <returns></returns>
		public override Texture2D GetDrawTexture()
		{
			if (mOnGround)
			{
				if (mWalkDirection == WalkDirection.None)
				{
					return mStandAnimation.GetCurrentTexture();
				}
				else
				{
					return mRunningAnimation.GetCurrentTexture();
				}
			}
			else
			{
				if (mVelocity.Y <= 0.0f)
				{
					return mJumpUpTex;
				}
				else
				{
					return mJumpDownTex;
				}
			}
		}

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Get the collider bounds of the AI entity
		/// </summary>
		/// <returns>Collider bounds of the AI entity</returns>
		public override Rect2f ColliderBounds()
		{
			return GetReducedTextureCollider(mCWidthReduction, mCHeightReduction);
		}

		#endregion rUtility
	}
}
