namespace AridArnold
{
	internal class LaserBomb : ProjectileEntity
	{
		#region rConstants

		const float LASER_BOMB_GRAVITY = 2.0f;

		#endregion rConstants





		#region rMembers

		Vector2 mVelocity;

		Animator mBombAnim;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Load laser bomb
		/// </summary>
		public LaserBomb(Vector2 position, Vector2 velocity) : base(position)
		{
			mVelocity = velocity;
		}



		/// <summary>
		/// Load laser texture
		/// </summary>
		public override void LoadContent(ContentManager content)
		{
			mBombAnim = new Animator(content, Animator.PlayType.Repeat, ("Enemies/Futron-Rocket/Bomb1", 0.1f)
																	  , ("Enemies/Futron-Rocket/Bomb2", 0.1f)
																	  , ("Enemies/Futron-Rocket/Bomb3", 0.1f)
																	  , ("Enemies/Futron-Rocket/Bomb4", 0.1f));
			mBombAnim.Play();

			mTexture = mBombAnim.GetTexture(0);
		}

		#endregion rInitialiation





		#region rUpdate

		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);

			// Free-body motion
			mVelocity.Y += dt * LASER_BOMB_GRAVITY;
			mPosition += dt * mVelocity;

			mBombAnim.Update(gameTime);
			base.Update(gameTime);
		}


		/// <summary>
		/// Collider bounds.
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			return new Rect2f(mPosition, mTexture);
		}

		#endregion rUpdate





		#region rDraw

		public override void Draw(DrawInfo info)
		{
			Vector2 roundedPos = MonoMath.Round(mPosition);
			MonoDraw.DrawTexture(info, mBombAnim.GetCurrentTexture(), roundedPos);
		}

		#endregion rDraw
	}
}
