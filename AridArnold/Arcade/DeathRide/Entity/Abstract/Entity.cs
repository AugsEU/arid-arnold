namespace GMTK2023
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
		protected bool mPlayerNear;
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
			mPlayerNear = false;

			// To do: Make this atomic if we end up needing to spawn multiple entities on different threads.
			sHandleHead++;
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

		#endregion rProperties





		#region rUpdate

		/// <summary>
		/// Update entity. No guarantees on order.
		/// </summary>
		/// <param name="gameTime">Frame time.</param>
		public virtual void Update(GameTime gameTime)
		{
			mCentreOfMass = ColliderBounds().Centre;

			//Calculate order for OrderedUpdate
			CalculateUpdateOrder();

			HandleInput();
			mPlayerNear = false;
		}



		/// <summary>
		/// Handle any inputs
		/// </summary>
		void HandleInput()
		{
		}



		/// <summary>
		/// Trigger event when interacted with
		/// </summary>
		protected virtual void OnPlayerInteract()
		{
		}



		/// <summary>
		/// React to a collision with this entity.
		/// </summary>
		/// <param name="entity"></param>
		public virtual void OnCollideEntity(Entity entity)
		{

		}



		/// <summary>
		/// Calculate update order. This is supposed to be storred in mUpdateOrder
		/// </summary>
		protected virtual void CalculateUpdateOrder()
		{
		}

		protected void ForceInBounds(Rectangle area)
		{
			Rect2f collider = ColliderBounds();

			if (mPosition.X < area.X)
			{
				mPosition.X = area.X;
			}

			if (mPosition.X + collider.Width > area.X + area.Width)
			{
				mPosition.X = area.X + area.Width - collider.Width;
			}

			if (mPosition.Y < area.Y)
			{
				mPosition.Y = area.Y;
			}

			if (mPosition.Y + collider.Height > area.Y + area.Height)
			{
				mPosition.Y = area.Y + area.Height - collider.Height;
			}
		}


		public virtual void Kill()
		{
			EntityManager.I.QueueDeleteEntity(this);
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
	}
}
