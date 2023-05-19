using AridArnold.Tiles.Basic;

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
		public override void LoadContent()
		{
			mBaseTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/WW7/TeleportPipe");

			base.LoadContent();
		}



		/// <summary>
		/// Finish initialisation.
		/// </summary>
		public override void FinishInit()
		{
			mIsEntry = GetNumDirectlyAdjacenct() == 1;
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
		public override void OnTouch(MovingEntity entity, CollisionResults collisionResults)
		{
			if(!mIsEntry)
			{
				return;
			}

			Vector2 normal = GetEntryDirection();
			if(normal != collisionResults.normal)
			{
				return;
			}

			// Spawn traveller
			TeleportTransporter teleportTransporter = new TeleportTransporter(mTileMapIndex, entity);
			EntityManager.I.QueueRegisterEntity(teleportTransporter);

			// To do: Spawn FX

			base.OnTouch(entity, collisionResults);
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





		#region rUtility

		/// <summary>
		/// Get normal of entry direction.
		/// E.g. if we are entering from the top, this returns up.
		/// </summary>
		Vector2 GetEntryDirection()
		{
			if(!mIsEntry)
			{
				return Vector2.Zero;
			}

			switch (mAdjacency)
			{
				case AdjacencyType.Ad8:
					return new Vector2(0.0f, 1.0f);
				case AdjacencyType.Ad2:
					return new Vector2(0.0f, -1.0f);
				case AdjacencyType.Ad4:
					return new Vector2(1.0f, 0.0f);
				case AdjacencyType.Ad6:
					return new Vector2(-1.0f, 0.0f);
				default:
					break;
			}

			throw new NotImplementedException();
		}

		#endregion rUtility
	}
}
