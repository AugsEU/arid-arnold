namespace AridArnold
{
	/// <summary>
	/// Ghost of Arnold's fastest attempt.
	/// </summary>
	class GhostArnold : Arnold
	{
		#region rInitialisation

		/// <summary>
		/// Constructor 
		/// </summary>
		/// <param name="startPos">Starting position</param>
		public GhostArnold(Vector2 startPos) : base(startPos)
		{
		}



		/// <summary>
		/// Start level with default parameters
		/// </summary>
		public void StartLevel()
		{
			mPrevDirection = WalkDirection.Right;
			mVelocity = Vector2.Zero;
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
		}



		/// <summary>
		/// Get colour to draw this ghost as.
		/// </summary>
		/// <returns></returns>
		protected override Color GetDrawColor()
		{
			//Slight green.
			return new Color(0.0f, 0.4f, 0.0f, 0.9f);
		}

		#endregion
	}
}
