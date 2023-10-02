namespace AridArnold
{
	internal class PlantPot : PlatformingEntity
	{
		#region rMembers

		int mLength;
		bool mIsWinter;

		Texture2D mTreeTrunk;
		Texture2D mTreeTop;
		Texture2D mTreeGhost;
		Texture2D mPotSummer;
		Texture2D mPotWinter;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Create plant pot at point
		/// </summary>
		public PlantPot(Vector2 pos, int length) : base(pos)
		{
			mIsWinter = TimeZoneManager.I.GetCurrentTimeZone() == 1;
			mLength = length;
			EventManager.I.AddListener(EventType.TimeChanged, OnTimeChange);
		}

		/// <summary>
		/// Load content
		/// </summary>
		/// <exception cref="NotImplementedException"></exception>
		public override void LoadContent()
		{
			mTreeTrunk = MonoData.I.MonoGameLoad<Texture2D>("PlantPot/TreeTrunk");
			mTreeTop = MonoData.I.MonoGameLoad<Texture2D>("PlantPot/TreeTop");
			mTreeGhost = MonoData.I.MonoGameLoad<Texture2D>("PlantPot/TreeGhost");
			mPotSummer = MonoData.I.MonoGameLoad<Texture2D>("PlantPot/PotSummer");
			mPotWinter = MonoData.I.MonoGameLoad<Texture2D>("PlantPot/PotWinter");

			mTexture = mPotWinter;
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
		/// Update pushing from entities
		/// </summary>
		private void UpdatePush(GameTime gameTime)
		{
			if (!mIsWinter)
			{
				SetWalkDirection(WalkDirection.None);
				return;
			}

			if(IceTile.IsOnIce(this))
			{
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
			
			if(direction > 0)
			{
				SetWalkDirection(WalkDirection.Left);
			}
			else if(direction < 0)
			{
				SetWalkDirection(WalkDirection.Right);
			}
			else
			{
				SetWalkDirection(WalkDirection.None);
			}
		}



		/// <summary>
		/// Called when the time changes
		/// </summary>
		public void OnTimeChange(EArgs eArgs)
		{
			mIsWinter = TimeZoneManager.I.GetCurrentTimeZone() == 1;
		}



		/// <summary>
		/// Collider bounds
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			Vector2 min;
			Vector2 max;
			int length = mIsWinter ? 0 : mLength;

			switch (GetGravityDir())
			{
				case CardinalDirection.Down:
					min = mPosition + new Vector2(0.0f, -length* Tile.sTILE_SIZE);
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

			Texture2D potTexture = mIsWinter ? mPotWinter : mPotSummer;
			MonoDraw.DrawPlatformer(info, originalTextureRect, potTexture, Color.White, gravityDir, mPrevDirection, drawLayer);

			Vector2 negGravity = -Tile.sTILE_SIZE * Util.GetNormal(gravityDir);
			for (int i = 0; i < mLength; ++i)
			{
				Texture2D treeTexture = mTreeGhost;
				if(!mIsWinter)
				{
					treeTexture = i == mLength - 1 ? mTreeTop : mTreeTrunk;
				}
				originalTextureRect.min += negGravity;
				originalTextureRect.max += negGravity;

				MonoDraw.DrawPlatformer(info, originalTextureRect, treeTexture, Color.White, gravityDir, mPrevDirection, drawLayer);
			}
		}

		#endregion rDraw
	}
}
