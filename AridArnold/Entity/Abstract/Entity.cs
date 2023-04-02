namespace AridArnold
{
	/// <summary>
	/// Represents a moving entity in the game world.
	/// </summary>
	abstract class Entity
	{
		#region rMembers

		// Keep track for IDs. Might be problems if you spawn 2^64 entities in a single play session.
		static UInt64 sHandleHead = 0;

		protected UInt64 mHandle;
		protected Vector2 mPosition;
		protected Vector2 mCentreOfMass;
		protected Texture2D mTexture;
		protected float mUpdateOrder;
		private bool mEnabled;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Entity constructor
		/// </summary>
		/// <param name="pos">Starting position.</param>
		public Entity(Vector2 pos)
		{
			mPosition = pos;
			mCentreOfMass = pos;
			mUpdateOrder = 0.0f;
			mHandle = sHandleHead;
			mEnabled = true;

			// To do: Make this atomic if we end up needing to spawn multiple entities on different threads.
			sHandleHead++;
		}



		/// <summary>
		/// LoadContent for entity such as textures
		/// </summary>
		public abstract void LoadContent();

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update entity. No guarantees on order.
		/// </summary>
		/// <param name="gameTime">Frame time.</param>
		public virtual void Update(GameTime gameTime)
		{
			TileManager.I.EntityTouchTiles(this);

			mCentreOfMass = ColliderBounds().Centre;

			//Calculate order for OrderedUpdate
			CalculateUpdateOrder();
		}



		/// <summary>
		/// Update entity with entity update order done by mUpdateOrder.
		/// </summary>
		public virtual void OrderedUpdate(GameTime gameTime)
		{

		}



		/// <summary>
		/// React to a collision with this entity.
		/// </summary>
		/// <param name="entity"></param>
		public virtual void CollideWithEntity(Entity entity)
		{
			//Default: Do nothing.
		}



		/// <summary>
		/// Calculate update order. This is supposed to be storred in mUpdateOrder
		/// </summary>
		protected virtual void CalculateUpdateOrder()
		{
		}


		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw entity
		/// </summary>
		/// <param name="info">Info needed for drawing.</param>
		public abstract void Draw(DrawInfo info);

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Get the collider for this entity
		/// </summary>
		/// <returns></returns>
		public abstract Rect2f ColliderBounds();



		/// <summary>
		/// Move position by relative amount.
		/// </summary>
		/// <param name="shift">Relative change in position.</param>
		public void ShiftPosition(Vector2 shift)
		{
			mPosition += shift;
		}


		/// <summary>
		/// Get position of 
		/// </summary>
		public Vector2 GetPos()
		{
			return mPosition;
		}


		/// <summary>
		/// Set position.
		/// </summary>
		public void SetPos(Vector2 pos)
		{
			mPosition = pos;
		}



		/// <summary>
		/// Get the centre of this entity
		/// </summary>
		public Vector2 GetCentrePos()
		{
			Rect2f collider = ColliderBounds();

			return (collider.min + collider.max) / 2.0f;
		}



		/// <summary>
		/// Set position relative to centre.
		/// </summary>
		public void SetCentrePos(Vector2 centrePos)
		{
			Rect2f collider = ColliderBounds();
			centrePos.X -= collider.Width / 2.0f;
			centrePos.Y -= collider.Height / 2.0f;

			mPosition = centrePos;
		}



		/// <summary>
		/// Get the update ordering. Higher value means update will happen first.
		/// </summary>
		public float GetUpdateOrder()
		{
			return mUpdateOrder;
		}


		/// <summary>
		/// Get unique handle for this entity.
		/// </summary>
		public UInt64 GetHandle()
		{
			return mHandle;
		}



		/// <summary>
		/// Enable/Disable this entity. Disabled entities will not be drawn or updated.
		/// </summary>
		public virtual void SetEnabled(bool enabled)
		{
			mEnabled = enabled;
		}



		/// <summary>
		/// Is this enabled?
		/// </summary>
		public bool IsEnabled()
		{
			return mEnabled;
		}

		#endregion rUtility





		#region rFactory

		public static Entity CreateEntityFromData(EntityData data)
		{
			Vector2 worldPosition = TileManager.I.GetTileTopLeft(data.mPosition);

			Entity entity;

			switch (data.mEntityClass)
			{
				// Player
				case EntityData.EntityClass.kArnold:
					entity = new Arnold(worldPosition);
					break;
				case EntityData.EntityClass.kAndrold:
					entity = new Androld(worldPosition);
					break;

				// Enemy
				case EntityData.EntityClass.kTrundle:
					entity = new Trundle(worldPosition);
					break;
				case EntityData.EntityClass.kRoboto:
					entity = new Roboto(worldPosition);
					break;
				case EntityData.EntityClass.kFutronGun:
					entity = new FutronGun(worldPosition, data.mFloatParams[0], data.mFloatParams[1]);
					break;
				case EntityData.EntityClass.kFutronRocket:
					entity = new FutronRocket(worldPosition, data.mFloatParams[0], data.mFloatParams[1], data.mFloatParams[2], data.mFloatParams[3]);
					break;

				// NPC
				case EntityData.EntityClass.kBarbara:
					entity = new Barbara(worldPosition);
					break;
				case EntityData.EntityClass.kZippy:
					entity = new Zippy(worldPosition);
					break;
				case EntityData.EntityClass.kDok:
					entity = new Dok(worldPosition);
					break;
				case EntityData.EntityClass.kBickDogel: // Special NPC
					entity = new GrillVogel(worldPosition);
					break;
				case EntityData.EntityClass.kElectrent:
					entity = new Electrent(worldPosition);
					break;
				default:
					throw new NotImplementedException();
			}

			if(entity is PlatformingEntity)
			{
				PlatformingEntity platformingEntity = (PlatformingEntity)entity;
				platformingEntity.SetGravity(data.mGravityDirection);
				platformingEntity.SetPrevWalkDirection(data.mStartDirection);
			}

			if(entity is SimpleTalkNPC)
			{
				SimpleTalkNPC npc = (SimpleTalkNPC)entity;
				npc.SetTalkText(data.mTalkText);
				npc.SetHeckleText(data.mHeckleText);
			}

			return entity;
		}

		#endregion rFactory
	}
}
