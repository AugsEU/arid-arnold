namespace AridArnold
{
	/// <summary>
	/// Entity that can move and collide.
	/// </summary>
	abstract class MovingEntity : Entity
	{
		#region rMembers

		protected Vector2 mVelocity;
		protected bool mFallthrough;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Create moving entity
		/// </summary>
		/// <param name="pos">Starting position</param>
		public MovingEntity(Vector2 pos) : base(pos)
		{
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update Moving Entity. No guarantees on order.
		/// </summary>
		/// <param name="gameTime">Frame time.</param>
		public override void OrderedUpdate(GameTime gameTime)
		{
			EntityManager.I.UpdateCollisionEntity(gameTime, this);

			ApplyVelocity(gameTime);

			mFallthrough = false;

			base.OrderedUpdate(gameTime);
		}



		/// <summary>
		/// Move position by velocity
		/// </summary>
		/// <param name="gameTime">Frame time.</param>
		protected void ApplyVelocity(GameTime gameTime)
		{
			mPosition += VelocityToDisplacement(gameTime);
		}



		/// <summary>
		/// Resolve collision with specific normal.
		/// </summary>
		/// <param name="collisionNormal"></param>
		public abstract void ReactToCollision(Vector2 collisionNormal);

		#endregion rUpdate





		#region rUtility

		/// <summary>
		/// Should we collide with platform tiles?
		/// </summary>
		/// <returns></returns>
		public bool CollideWithPlatforms()
		{
			return !mFallthrough;
		}



		/// <summary>
		/// Convert velocity into actual position displacement.
		/// </summary>
		/// <param name="gameTime">Frame time.</param>
		/// <returns></returns>
		public Vector2 VelocityToDisplacement(GameTime gameTime)
		{
			return mVelocity * Util.GetDeltaT(gameTime);
		}



		/// <summary>
		/// Make entity fall through platforms.
		/// </summary>
		protected void FallThroughPlatforms()
		{
			mFallthrough = true;
		}



		/// <summary>
		/// Velocity Property.
		/// </summary>
		public Vector2 pVelocity
		{
			get { return mVelocity; }
			set { mVelocity = value; }
		}

		#endregion rUtility

	}
}
