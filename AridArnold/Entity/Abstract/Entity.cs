namespace AridArnold
{
	// Layers for interacting with things.
	enum InteractionLayer : byte
	{
		kNone =			0b0000_0000,
		kPlayer =		0b0000_0001, // For Player entity
		kAltPlayer =	0b0000_0010, // For other things Players control
		kGravityOrb =   0b0000_0100  // For things affected by gravity orb
	}

	/// <summary>
	/// Represents a moving entity in the game world.
	/// </summary>
	abstract class Entity
	{
		#region rMembers

		// Keep track for IDs. Might be problems if you spawn 2^64 entities in a single play session.
		public static UInt64 sHandleHead = 0;

		protected UInt64 mHandle;
		protected Vector2 mPosition;
		protected Vector2 mCentreOfMass;
		protected Texture2D mTexture;
		protected float mUpdateOrder;
		protected bool mPlayerNear;
		private bool mEnabled;
		private InteractionLayer mLayer;

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
			mPlayerNear = false;

			// To do: Make this atomic if we end up needing to spawn multiple entities on different threads.
			sHandleHead++;

			mLayer = InteractionLayer.kNone;
		}



		/// <summary>
		/// LoadContent for entity such as textures
		/// </summary>
		public abstract void LoadContent();

		#endregion rInitialisation





		#region rProperties

		/// <summary>
		/// Should this entity persist after we come back from a door
		/// </summary>
		public virtual bool PersistLevelEntry()
		{
			return false;
		}



		/// <summary>
		/// What should this interact with?
		/// </summary>
		public virtual InteractionLayer GetInteractionLayer()
		{
			return mLayer;
		}



		/// <summary>
		/// Are we on this interaction layer?
		/// </summary>
		public bool OnInteractLayer(InteractionLayer layer)
		{
			return MonoAlg.TestFlag(GetInteractionLayer(), layer);
		}



		/// <summary>
		/// Start interacting with layer
		/// </summary>
		protected void LayerOptIn(params InteractionLayer[] flags)
		{
			mLayer = (InteractionLayer)MonoAlg.AddFlags(mLayer, flags);
		}



		/// <summary>
		/// Start interacting with layer
		/// </summary>
		protected void LayerOptOut(params InteractionLayer[] flags)
		{
			mLayer = (InteractionLayer)MonoAlg.SubFlags(mLayer, flags);
		}

		#endregion rProperties





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

			HandleInput();
			mPlayerNear = false;

			if (EventManager.I.IsSignaled(EventType.TimeChanged))
			{
				OnTimeChange(gameTime);
			}
		}



		/// <summary>
		/// Handle any inputs
		/// </summary>
		void HandleInput()
		{
			if (mPlayerNear == false)
			{
				// Player can't interact
				return;
			}

			bool activate = InputManager.I.KeyHeld(InputAction.Confirm);

			if (activate)
			{
				OnPlayerInteract();
			}
		}



		/// <summary>
		/// Trigger event when interacted with
		/// </summary>
		protected virtual void OnPlayerInteract()
		{
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
		public virtual void OnCollideEntity(Entity entity)
		{
			if (entity.OnInteractLayer(InteractionLayer.kPlayer))
			{
				mPlayerNear = true;
			}
		}



		/// <summary>
		/// Calculate update order. This is supposed to be storred in mUpdateOrder
		/// </summary>
		protected virtual void CalculateUpdateOrder()
		{
		}



		/// <summary>
		/// Kill this entity. By default most are immortal in this sense
		/// </summary>
		public virtual void Kill()
		{

		}



		/// <summary>
		/// Called when time changes
		/// </summary>
		protected virtual void OnTimeChange(GameTime gameTime)
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
		public virtual Rect2f ColliderBounds()
		{
			return new Rect2f(mPosition, mTexture);
		}



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
		/// Set position relative to centre.
		/// </summary>
		public void SetMiddleFootPos(Vector2 footMiddle)
		{
			Rect2f collider = ColliderBounds();

			footMiddle.X -= collider.Width / 2.0f;
			footMiddle.Y -= collider.Height;

			mPosition = footMiddle;
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



		/// <summary>
		/// Is the player intersecting our collider?
		/// </summary>
		protected bool IsPlayerNear()
		{
			return mPlayerNear;
		}

		#endregion rUtility





		#region rFactory

		public static Entity CreateEntityFromData(EntityData data)
		{
			Vector2 worldPosition = TileManager.I.GetTileTopLeft(data.mPosition);
			worldPosition.Y -= 0.000123456f; // Collision offset bodge

			Entity entity;

			switch (data.mEntityClass)
			{
				// Player
				case EntityData.EntityClass.kArnold:
					entity = new Arnold(worldPosition, data.mIntParams[0] != 0);
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
				case EntityData.EntityClass.kFarry:
					entity = new FallingFarry(worldPosition);
					break;
				case EntityData.EntityClass.kMamal:
					entity = new Mamal(worldPosition);
					break;
				case EntityData.EntityClass.kPapyras:
					entity = new Papyras(worldPosition, data.mFloatParams[0], data.mFloatParams[1]);
					break;
				case EntityData.EntityClass.kRanger:
					entity = new Ranger(worldPosition, data.mFloatParams[0], data.mFloatParams[1]);
					break;

				// NPC
				case EntityData.EntityClass.kSimpleNPC:
					entity = new SimpleTalkNPC(worldPosition, data.mNPCDataPath, data.mTalkText, data.mHeckleText, data.mIntParams[0] == 1, data.mIntParams[1] == 1);
					break;
				case EntityData.EntityClass.kBickDogel: // Special NPC
					entity = new GrillVogel(worldPosition);
					break;
				case EntityData.EntityClass.kFireBarrel:
					entity = new FireBarrel(worldPosition);
					break;
				case EntityData.EntityClass.kBouncer:
					entity = new Bouncer(worldPosition);
					break;
				case EntityData.EntityClass.kGreatGate:
					entity = new GreatGate(worldPosition);
					break;

				// Utility
				case EntityData.EntityClass.kArnoldSpawner:
					entity = new ArnoldRespawn(worldPosition, data.mGravityDirection, data.mStartDirection);
					break;
				case EntityData.EntityClass.kSequenceDoor:
					entity = new SequenceDoor(worldPosition, data.mIntParams, (int)data.mFloatParams[0]);
					break;
				case EntityData.EntityClass.kLevelLock:
					entity = new KeyDoor(worldPosition, data.mIntParams[0]);
					break;
				case EntityData.EntityClass.kShopDoor:
					entity = new ShopDoor(worldPosition, data.mIntParams[0], data.mFloatParams[0], data.mFloatParams[1]);
					break;
				case EntityData.EntityClass.kItemStand:
					entity = new ItemStand(worldPosition, data.mIntParams[0]);
					break;
				case EntityData.EntityClass.kGravityOrb:
					entity = new GravityOrb(worldPosition, (CardinalDirection)data.mIntParams[0]);
					break;
				case EntityData.EntityClass.kGravityTile:
					entity = new GravityTile(worldPosition);
					break;
				case EntityData.EntityClass.kTimeMachine:
					entity = new TimeMachine(worldPosition, data.mIntParams[0], data.mIntParams[1]);
					break;
				case EntityData.EntityClass.kPlantPot:
					entity = new PlantPot(worldPosition, data.mIntParams[0]);
					break;
				case EntityData.EntityClass.kPillarPot:
					entity = new PillarPot(worldPosition, data.mIntParams[0]);
					break;
				case EntityData.EntityClass.kWorldCabinet:
					entity = new WorldArcadeCabinet(worldPosition, (ArcadeGameType)data.mIntParams[0]);
					break;
				case EntityData.EntityClass.kArcadeBuilding:
					entity = new ArcadeBuilding(worldPosition, data.mIntParams[0], data.mFloatParams[0], data.mFloatParams[1]);
					break;
				case EntityData.EntityClass.kManualReader:
					entity = new ManualReader(worldPosition);
					break;
				case EntityData.EntityClass.kFountain:
					entity = new Fountain(worldPosition);
					break;
				default:
					throw new NotImplementedException();
			}

			if (entity is PlatformingEntity)
			{
				PlatformingEntity platformingEntity = (PlatformingEntity)entity;
				platformingEntity.SetGravity(data.mGravityDirection);
				platformingEntity.SetPrevWalkDirection(data.mStartDirection);
			}

			return entity;
		}

		#endregion rFactory
	}
}
