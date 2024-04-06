namespace AridArnold
{
	class RailPlatform : Entity
	{
		#region rMembers

		Animator mPlatformAnimation;
		RailTraveller mRail;
		int mSize;
		CardinalDirection mRotation;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Create rail platform that travels along a rail.
		/// </summary>
		public RailPlatform(RailTraveller rail, CardinalDirection direction, int size) : base(rail.GetPosition())
		{
			mRail = rail;
			mSize = size;
			mRotation = direction;
		}


		/// <summary>
		/// Load content texture.
		/// </summary>
		public override void LoadContent()
		{
			mPlatformAnimation = MonoData.I.LoadAnimator("Platform");
		}

		#endregion rInitialisation





		#region rUpdate

		public override void Update(GameTime gameTime)
		{
			mRail.Update(gameTime);

			mPlatformAnimation.Update(gameTime);

			EntityManager.I.AddColliderSubmission(new PlatformColliderSubmission(mRail.GetVelocity(gameTime), mPosition, mSize * Tile.sTILE_SIZE, mRotation));

			mPosition = mRail.GetPosition();

			base.Update(gameTime);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the platform.
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			Vector2 directionVec = MonoMath.Perpendicular(Util.GetNormal(mRotation)) * Tile.sTILE_SIZE;
			directionVec.X = MathF.Abs(directionVec.X);
			directionVec.Y = MathF.Abs(directionVec.Y);

			float rotation = Util.GetRotation(mRotation);

			for (int i = 0; i < mSize; i++)
			{
				Vector2 pos = mPosition + i * directionVec;

				MonoDraw.DrawTextureRotCent(info, mPlatformAnimation.GetCurrentTexture(), pos, rotation);
			}

			Vector2 offset = directionVec * mSize * 0.5f;
			switch (mRotation)
			{
				case CardinalDirection.Up:
					break;
				case CardinalDirection.Right:
					offset.X += 14.0f;
					break;
				case CardinalDirection.Down:
					offset.Y += 14.0f;
					break;
				case CardinalDirection.Left:

					break;
			}
			mRail.Draw(info, offset);
		}



		/// <summary>
		/// Size of platform
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			return new Rect2f(mPosition, mPosition + new Vector2(mSize * Tile.sTILE_SIZE, 3.0f));
		}

		#endregion rDraw
	}
}


