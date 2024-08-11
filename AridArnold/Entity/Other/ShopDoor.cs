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
		bool mDoorOpen;
		Vector2 mArnoldWarpPoint;
		int mLevelLoadID;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Create shop door
		/// </summary>
		public ShopDoor(Vector2 pos, int toID, float arnoldX, float arnoldY) : base(pos)
		{
			mDoorOpen = false;
			mLevelLoadID = toID;
			mArnoldWarpPoint = new Vector2(arnoldX, arnoldY);
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
		/// Open the door when interacted with
		/// </summary>
		protected override void OnPlayerInteract()
		{
			OpenDoor();
			base.OnPlayerInteract();
		}



		/// <summary>
		/// Open the door
		/// </summary>
		void OpenDoor()
		{
			if(mDoorOpen)
			{
				return;
			}

			SFXManager.I.PlaySFX(AridArnoldSFX.DoorOpen, 0.1f);

			mDoorOpen = true;
			mTexture = mOpenTexture;

			EventManager.I.TriggerEvent(EventType.ShopDoorOpen);

			if(CampaignManager.I.GetCurrentLevelType() == AuxData.LevelType.Hub)
			{
				HubDirectLoader toHub = new HubDirectLoader(mLevelLoadID);
				Arnold arnold = EntityManager.I.FindArnold(); // OK because in hub.

				if(arnold is not null)
				{
					toHub.AddPersistentEntities(arnold);
					arnold.SetPos(mArnoldWarpPoint);
					arnold.SetVelocity(Vector2.Zero);
				}

				CampaignManager.I.QueueLoadSequence(toHub);
			}
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
