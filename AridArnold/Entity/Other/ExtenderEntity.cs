using Microsoft.Xna.Framework.Graphics;

namespace AridArnold
{
	abstract class ExtenderEntity : PlatformingEntity
	{
		#region rMembers

		int mLength;
		bool mIsExtended;
		WalkDirection mPrevPushDir;

		protected Texture2D mMiddleTexture;
		protected Texture2D mTopTexture;
		protected Texture2D mGhostTexture;
		protected Texture2D mOffSeasonTexture;
		protected Texture2D mOnSeasonTexture;
		

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Create extender at point
		/// </summary>
		public ExtenderEntity(Vector2 pos, int length) : base(pos, walkSpeed: 9.5f, iceGrip: 20.2f)
		{
			mIsExtended = TimeZoneManager.I.GetCurrentTimeZone() == GetOnSeason();
			mLength = length;
			EventManager.I.AddListener(EventType.TimeChanged, OnTimeChange);
			mPrevDirection = WalkDirection.Right;
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update plantpot
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			EntityManager.I.AddColliderSubmission(new EntityColliderSubmission(this));

			mUpdateOrder = +10000.0f;

			base.Update(gameTime);
		}



		/// <summary>
		/// Physics
		/// </summary>
		public override void OrderedUpdate(GameTime gameTime)
		{
			UpdatePush(gameTime);

			base.OrderedUpdate(gameTime);
		}



		/// <summary>
		/// React to a collision
		/// </summary>
		protected override void ReactToCollision(CollisionType collisionType)
		{
			switch (collisionType)
			{
				case CollisionType.Wall:
					BeginPush(WalkDirection.None);
					break;
			}

			base.ReactToCollision(collisionType);
		}



		/// <summary>
		/// Update pushing from entities
		/// </summary>
		private void UpdatePush(GameTime gameTime)
		{
			if (!mIsExtended)
			{
				BeginPush(WalkDirection.None);
				return;
			}

			List<Entity> nearbyEntities = EntityManager.I.GetNearPos(Tile.sTILE_SIZE * 0.9f, GetCentrePos(), typeof(Arnold));

			int direction = 0;

			foreach (Entity entity in nearbyEntities)
			{
				PlatformingEntity platformingEntity = entity as PlatformingEntity;

				if (platformingEntity.OnGround() && platformingEntity.GetGravityDir() == GetGravityDir())
				{
					WalkDirection toMe = DirectionNeededToWalkToMe(platformingEntity.GetCentrePos());
					if (platformingEntity.GetWalkDirection() == toMe)
					{
						if (toMe == WalkDirection.Left)
						{
							direction += 1;
						}
						else if (toMe == WalkDirection.Right)
						{
							direction -= 1;
						}
					}
				}
			}

			bool shouldGrip = mVelocity.LengthSquared() < 0.1f || !IceTile.IsOnIce(this);

			if (direction > 0)
			{
				BeginPush(WalkDirection.Left);
			}
			else if (direction < 0)
			{
				BeginPush(WalkDirection.Right);
			}
			else if (shouldGrip)
			{
				BeginPush(WalkDirection.None);
			}
		}



		/// <summary>
		/// Begin push if possible
		/// </summary>
		/// <param name="direction"></param>
		private void BeginPush(WalkDirection direction)
		{
			if (direction != WalkDirection.None)
			{
				//Check for platforms
				Vector2 checkDir = Util.GetNormal(Util.WalkDirectionToCardinal(direction, GetGravityDir()));
				checkDir *= Tile.sTILE_SIZE / 1.95f;

				Vector2 checkPos = GetCentrePos() + checkDir;

				if (TileManager.I.GetTile(checkPos) is PlatformTile)
				{
					direction = WalkDirection.None;
				}
			}

			SetWalkDirection(direction);
			if(direction != WalkDirection.None)
			{
				mPrevPushDir = direction;
			}
		}



		/// <summary>
		/// Called when the time changes
		/// </summary>
		public void OnTimeChange(EArgs eArgs)
		{
			mIsExtended = TimeZoneManager.I.GetCurrentTimeZone() == GetOnSeason();

			int maxCounter = 0;

			MonoDebug.Assert(mPrevPushDir != WalkDirection.None);
			Vector2 walkDir = Util.GetNormal(Util.WalkDirectionToCardinal(mPrevPushDir, GetGravityDir()));

			while (IceTile.IsOnIce(this, 2.0f) && maxCounter < 5000)
			{
				maxCounter++;
				mPosition += walkDir;
			}

			MonoDebug.Assert(maxCounter != 5000);
		}



		/// <summary>
		/// Collider bounds
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			Vector2 min;
			Vector2 max;
			int length = mIsExtended ? 0 : mLength;

			switch (GetGravityDir())
			{
				case CardinalDirection.Down:
					min = mPosition + new Vector2(0.0f, 0.0001f - length * Tile.sTILE_SIZE);
					max = mPosition + new Vector2(Tile.sTILE_SIZE, Tile.sTILE_SIZE);
					break;
				default:
					throw new NotImplementedException();
			}

			return new Rect2f(min, max);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw tree texture;
		/// </summary>
		/// <param name="info"></param>
		public override void Draw(DrawInfo info)
		{
			Rect2f originalTextureRect = new Rect2f(mPosition, mTexture);
			CardinalDirection gravityDir = GetGravityDir();
			DrawLayer drawLayer = GetDrawLayer();

			Texture2D potTexture = mIsExtended ? mOnSeasonTexture : mOffSeasonTexture;
			MonoDraw.DrawPlatformer(info, originalTextureRect, potTexture, Color.White, gravityDir, mPrevDirection, drawLayer);

			Vector2 negGravity = -Tile.sTILE_SIZE * Util.GetNormal(gravityDir);
			for (int i = 0; i < mLength; ++i)
			{
				Texture2D treeTexture = mGhostTexture;
				if (!mIsExtended)
				{
					treeTexture = i == mLength - 1 ? mTopTexture : mMiddleTexture;
				}
				originalTextureRect.min += negGravity;
				originalTextureRect.max += negGravity;

				MonoDraw.DrawPlatformer(info, originalTextureRect, treeTexture, Color.White, gravityDir, mPrevDirection, drawLayer);
			}
		}



		/// <summary>
		/// Get layer to draw on
		/// </summary>
		protected override DrawLayer GetDrawLayer()
		{
			return DrawLayer.TileEffects;
		}

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Get the season where we are extended
		/// </summary>
		protected abstract int GetOnSeason();

		#endregion rUtility
	}
}
