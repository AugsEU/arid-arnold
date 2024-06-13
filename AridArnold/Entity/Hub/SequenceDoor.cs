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
		Texture2D mOutOfTimeTexture;
		Texture2D[] mNumberTextures;

		LevelSequenceInfoBubble mHelpBubble;
		bool mDoorOpen;
		bool mAlreadyCompleted;
		Point mTileCoord;
		int mActiveTimeZone;

		List<Level> mLevelSequence;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Create sequence door
		/// </summary>
		/// <param name="levelSequence">List of level IDs</param>
		public SequenceDoor(Vector2 pos, int[] levelSequence, int timeZone) : base(pos)
		{
			mLevelSequence = new List<Level>();
			for (int i = 0; i < levelSequence.Length; i++)
			{
				int lvlID = levelSequence[i];
				if (lvlID != 0)
				{
					string path = CampaignManager.I.GetLevelPath(lvlID);
					mLevelSequence.Add(Level.LoadFromFile(path, lvlID));
				}
			}

			MonoDebug.Assert(mLevelSequence.Count > 0 && mLevelSequence.Count < 10);

			BubbleStyle bubbleStyle = new BubbleStyle();
			bubbleStyle.mInnerColor = new Color(20, 20, 20, 150);
			bubbleStyle.mBorderColor = new Color(150, 150, 150, 200);
			mHelpBubble = new LevelSequenceInfoBubble(mLevelSequence, mPosition + BUBBLE_OFFSET, bubbleStyle);

			mDoorOpen = false;
			mTileCoord = TileManager.I.GetTileMapCoord(mPosition);

			mActiveTimeZone = timeZone;
		}



		/// <summary>
		/// Load door content
		/// </summary>
		public override void LoadContent()
		{
			mOpenTexture = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorOpen");
			mClosedTexture = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorClosed");
			mOutOfTimeTexture = MonoData.I.MonoGameLoad<Texture2D>("Shared/Door/DoorOutOfTime");

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
			CheckCompletion();
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			if (!IsInCorrectTimeZone())
			{
				return;
			}

			CheckCompletion();
			mHelpBubble.Update(gameTime, IsPlayerNear());

			base.Update(gameTime);
		}



		/// <summary>
		/// Check if door has been completed yet.
		/// </summary>
		private void CheckCompletion()
		{
			mAlreadyCompleted = CollectableManager.I.HasSpecific(mTileCoord, (UInt16)PermanentCollectable.Door);
		}



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
			mDoorOpen = true;
			mTexture = mOpenTexture;

			CampaignManager.I.PushLevelSequence(mLevelSequence, mTileCoord);
			CampaignManager.I.QueueLoadSequence(new LevelSequenceLoader());
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
			Texture2D texture = IsInCorrectTimeZone() ? mTexture : mOutOfTimeTexture;
			MonoDraw.DrawTextureDepth(info, texture, mPosition, DrawLayer.Default);

			if (mDoorOpen == false && IsInCorrectTimeZone())
			{
				Color numberCol = mAlreadyCompleted ? Color.Gray : Color.White;
				MonoDraw.DrawTextureDepthColor(info, mNumberTextures[mLevelSequence.Count], mPosition, numberCol, DrawLayer.Default);
			}

			mHelpBubble.Draw(info);
		}

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Check if timezone is correct
		/// </summary>
		/// <returns></returns>
		bool IsInCorrectTimeZone()
		{
			return TimeZoneManager.I.GetCurrentTimeZone() == mActiveTimeZone;
		}

		#endregion rUtility
	}
}
