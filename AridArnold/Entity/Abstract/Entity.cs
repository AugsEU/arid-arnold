namespace AridArnold
{
	/// <summary>
	/// Represents a moving entity in the game world.
	/// </summary>
	abstract class Entity
	{
		#region rMembers

		protected Vector2 mPosition;
		protected Vector2 mCentreOfMass;
		protected Texture2D mTexture;
		protected float mUpdateOrder;

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
		}



		/// <summary>
		/// LoadContent for entity such as textures
		/// </summary>
		/// <param name="content">Monogame Content Manager</param>
		public abstract void LoadContent(ContentManager content);

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
		/// Get collider submission for this frame. Entities that want to collide will have to submit one.
		/// </summary>
		public virtual ColliderSubmission GetColliderSubmission()
		{
			return null;
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
		/// Kill this entity.
		/// </summary>
		public virtual void Kill()
		{

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
		/// Position property.
		/// </summary>
		public Vector2 pPosition
		{
			get { return mPosition; }
			set { mPosition = value; }
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
		/// Get the update ordering. Higher value means update will happen first.
		/// </summary>
		public float GetUpdateOrder()
		{
			return mUpdateOrder;
		}

		#endregion rUtility
	}
}
