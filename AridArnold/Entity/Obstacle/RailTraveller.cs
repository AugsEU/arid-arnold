namespace AridArnold
{
	/// <summary>
	/// Represents a point travelling along a rail
	/// </summary>
	abstract class RailTraveller
	{
		#region rMembers

		protected Vector2 mPosition;
		Vector2 mPrevPosition;

		#endregion rMembers





		#region rInit

		public RailTraveller()
		{
			mPosition = new Vector2();
			mPrevPosition = new Vector2();
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Moves rail by timestep
		/// </summary>
		public void Update(GameTime gameTime)
		{
			mPrevPosition = mPosition;
			MoveRail(gameTime);
		}

		/// <summary>
		/// Move the rail.
		/// </summary>
		protected abstract void MoveRail(GameTime gameTime);

		#endregion rUpdate



		#region rDraw

		/// <summary>
		/// Draw rail
		/// </summary>
		public abstract void Draw(DrawInfo info, Vector2 offset);

		#endregion rDraw



		#region rUtil

		/// <summary>
		/// Get current position.
		/// </summary>
		public Vector2 GetPosition()
		{
			return mPosition;
		}



		/// <summary>
		/// Get velocity of this time step to be used in collisions
		/// </summary>
		public Vector2 GetVelocityForCollision(GameTime gameTime)
		{
			// We do collision in quarter steps
			const float COLLISION_STEPS = 4.0f;
			return COLLISION_STEPS * (mPosition - mPrevPosition) / Util.GetDeltaT(gameTime);
		}


		#endregion rUtil

	}
}
