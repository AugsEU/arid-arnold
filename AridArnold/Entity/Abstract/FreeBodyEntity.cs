﻿namespace AridArnold
{
	/// <summary>
	/// Dummy entity used for simulating a free body.
	/// </summary>
	internal class FreeBodyEntity : MovingEntity
	{
		#region rMembers

		CardinalDirection mGravityDir;
		float mGravity;
		int mSize;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Init free body at point
		/// </summary>
		public FreeBodyEntity(Vector2 pos, Vector2 vel, float gravity, CardinalDirection gravityDir, int size) : base(pos)
		{
			mVelocity = vel;
			mGravity = gravity;
			mGravityDir = gravityDir;
			mSize = size;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void LoadContent()
		{
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update. Do not call this!
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			throw new NotImplementedException();
		}



		/// <summary>
		/// Move forwards in the simulation.
		/// </summary>
		public void MoveTimeStep(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);
			mPosition += mVelocity * dt;
			mVelocity += mGravity * Util.GetNormal(mGravityDir) * dt;
		}



		/// <summary>
		/// Collider bounds.
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			Vector2 dist = new Vector2(mSize, mSize);

			return new Rect2f(mPosition, mPosition + dist);
		}



		/// <summary>
		/// Set gravity dir
		/// </summary>
		public void SetGravityDir(CardinalDirection gravityDir)
		{
			mGravityDir = gravityDir;
		}

		#endregion rUpdate



		#region rDraw

		/// <summary>
		/// Shouldn't draw.
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			throw new NotImplementedException();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="collisionNormal"></param>
		/// <exception cref="NotImplementedException"></exception>
		public override void ReactToCollision(Vector2 collisionNormal)
		{
		}

		#endregion rDraw
	}
}
