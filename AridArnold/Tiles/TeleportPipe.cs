namespace AridArnold
{
	class TeleportPipe : SquareTile
	{
		#region rMembers

		bool mIsEntry;

		// Draw
		Texture2D mBaseTexture;

		#endregion rMembers



		#region rInitialisation

		/// <summary>
		/// Teleportation pipe
		/// </summary>
		public TeleportPipe(Vector2 position) : base(position)
		{
		}



		/// <summary>
		/// Load textures for teleportation pipe
		/// </summary>
		public override void LoadContent(ContentManager content)
		{
			mBaseTexture = content.Load<Texture2D>("Tiles/WW7/TeleportPipe");

			base.LoadContent(content);
		}



		/// <summary>
		/// Finish initialisation.
		/// </summary>
		public override void FinishInit()
		{
			mIsEntry = GetNumNeighbours() == 1;
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update teleport tile
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}


		/// <summary>
		/// When an Entity touches us.
		/// </summary>
		public override void OnTouch(MovingEntity entity)
		{
			if(!mIsEntry)
			{
				return;
			}

			// Spawn traveller
			TeleportTransporter teleportTransporter = new TeleportTransporter(mTileMapIndex, entity);
			EntityManager.I.QueueRegisterEntity(teleportTransporter);

			// To do: Spawn FX

			base.OnTouch(entity);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get teleportation pipe texture
		/// </summary>
		public override Texture2D GetTexture()
		{
			return mBaseTexture;
		}

		#endregion rDraw
	}
}
