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





		#region rUtil

		/// <summary>
		/// Get current position.
		/// </summary>
		public Vector2 GetPosition()
		{
			return mPosition;
		}



		/// <summary>
		/// Get velocity of this time step.
		/// </summary>
		public Vector2 GetVelocity(GameTime gameTime)
		{
			return (mPosition - mPrevPosition) / Util.GetDeltaT(gameTime);
		}


		#endregion rUtil

	}
}
