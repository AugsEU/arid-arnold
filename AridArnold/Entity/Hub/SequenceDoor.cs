namespace AridArnold
{
	class SequenceDoor : Entity
	{
		#region rConstant

		const float HITBOX_WIDTH = 16.0f;
		const float HITBOX_HEIGHT = 16.0f;
		static Vector2 BUBBLE_OFFSET = new Vector2(8.0f, -10.0f);

		#endregion rConstant





		#region rMembers

		Texture2D mOpenTexture;
		Texture2D mClosedTexture;
		Texture2D[] mNumberTextures;

		LevelSequenceInfoBubble mHelpBubble;
		bool mPlayerNear;
		bool mDoorOpen;

		List<Level> mLevelSequence;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Create sequence door
		/// </summary>
		/// <param name="levelSequence">List of level IDs</param>
		public SequenceDoor(Vector2 pos, int[] levelSequence) : base(pos)
		{
			mLevelSequence = new List<Level>();
			for (int i = 0; i < levelSequence.Length; i++)
			{
				int lvlID = levelSequence[i];
				if(lvlID != 0)
				{
					string path = CampaignManager.I.GetLevelPath(lvlID);
					mLevelSequence.Add(Level.LoadFromFile(path));
				}
			}

			MonoDebug.Assert(mLevelSequence.Count > 0 && mLevelSequence.Count < 10);

			InfoBubble.BubbleStyle bubbleStyle = new InfoBubble.BubbleStyle();
			bubbleStyle.mInnerColor = new Color(20, 20, 20, 150);
			bubbleStyle.mBorderColor = new Color(150, 150, 150, 200);
			mHelpBubble = new LevelSequenceInfoBubble(mLevelSequence, mPosition + BUBBLE_OFFSET, bubbleStyle);

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

			mNumberTextures = new Texture2D[10];
			mNumberTextures[0] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorZero");
			mNumberTextures[1] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorOne");
			mNumberTextures[2] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorTwo");
			mNumberTextures[3] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorThree");
			mNumberTextures[4] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorFour");
			mNumberTextures[5] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorFive");
			mNumberTextures[6] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorSix");
			mNumberTextures[7] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorSeven");
			mNumberTextures[8] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorEight");
			mNumberTextures[9] = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorNine");

			mTexture = mClosedTexture;
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			if(mPlayerNear)
			{
				mHelpBubble.Open();
			}
			else
			{
				mHelpBubble.Close();
			}
			mHelpBubble.Update(gameTime);

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

			CampaignManager.I.PushLevelSequence(mLevelSequence);
			CampaignManager.I.QueueLoadSequence(new LevelSequenceLoader());
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
			MonoDraw.DrawTextureDepth(info, mNumberTextures[mLevelSequence.Count], mPosition, DrawLayer.Default);

			mHelpBubble.Draw(info);
		}

		#endregion rDraw
	}
}
