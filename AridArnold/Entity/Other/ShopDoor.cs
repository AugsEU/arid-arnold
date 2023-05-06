namespace AridArnold
{
	class ShopDoor : Entity
	{
		#region rConstant

		const float HITBOX_WIDTH = 16.0f;
		const float HITBOX_HEIGHT = 16.0f;
		static Vector2 BUBBLE_OFFSET = new Vector2(8.0f, -10.0f);

		#endregion rConstant





		#region rMembers

		Texture2D mOpenTexture;
		Texture2D mClosedTexture;
		Texture2D mArrowTexture;
		bool mPlayerNear;
		bool mDoorOpen;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Create shop door
		/// </summary>
		public ShopDoor(Vector2 pos) : base(pos)
		{
			mPlayerNear = false;
			mDoorOpen = false;
		}



		/// <summary>
		/// Load door content
		/// </summary>
		public override void LoadContent()
		{
			mOpenTexture = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorOpen");
			mClosedTexture = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorClosed");
			mArrowTexture = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorArrow");
			mTexture = mClosedTexture;
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			HandleInput();

			base.Update(gameTime);
			mPlayerNear = false;
		}



		/// <summary>
		/// Handle any inputs
		/// </summary>
		void HandleInput()
		{
			if(mPlayerNear == false || mDoorOpen == true)
			{
				// Player can't interact
				return;
			}

			bool activate = InputManager.I.KeyHeld(AridArnoldKeys.Confirm);

			if(activate)
			{
				OpenDoor();
			}
		}



		/// <summary>
		/// Open the door
		/// </summary>
		void OpenDoor()
		{
			mDoorOpen = true;
			mTexture = mOpenTexture;

			EventManager.I.SendEvent(EventType.ShopDoorOpen, new EArgs(this));
		}



		/// <summary>
		/// Handle collision.
		/// </summary>
		public override void OnCollideEntity(Entity entity)
		{
			if(entity is Arnold)
			{
				mPlayerNear = true;
			}

			base.OnCollideEntity(entity);
		}



		/// <summary>
		/// Activation area.
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			return new Rect2f(mPosition, HITBOX_WIDTH, HITBOX_HEIGHT);
		}

		#endregion rUpdate




		#region rDraw

		/// <summary>
		/// Draw door
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			MonoDraw.DrawTextureDepth(info, mTexture, mPosition, DrawLayer.Default);

			if (mDoorOpen == false)
			{
				MonoDraw.DrawTextureDepth(info, mArrowTexture, mPosition, DrawLayer.Default);
			}
		}

		#endregion rDraw
	}
}
