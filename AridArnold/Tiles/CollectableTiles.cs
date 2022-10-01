namespace AridArnold
{
	/// <summary>
	/// A tile you can collect
	/// </summary>
	abstract class CollectableTile : InteractableTile
	{
		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public CollectableTile(Vector2 position) : base(position)
		{
		}

		protected abstract CollectableType GetCollectableType();

		public override void OnEntityIntersect(Entity entity)
		{
			if (entity is Arnold)
			{
				CollectableManager.I.CollectItem(GetCollectableType());
				mEnabled = false;
			}
		}
	}





	/// <summary>
	/// A flag, represents the end of a world and a checkpoint
	/// </summary>
	class FlagTile : CollectableTile
	{
		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public FlagTile(Vector2 position) : base(position)
		{
		}

		/// <summary>
		/// Load all textures
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public override void LoadContent(ContentManager content)
		{
			mTexture = content.Load<Texture2D>("Tiles/flag");
		}

		public override void OnEntityIntersect(Entity entity)
		{
			if (entity is Arnold)
			{
				ProgressManager.I.ReportCheckpoint();
			}

			base.OnEntityIntersect(entity);
		}

		protected override CollectableType GetCollectableType()
		{
			return CollectableType.Flag;
		}
	}





	/// <summary>
	/// Hot dog tile. Gives the player 1 life
	/// </summary>
	class HotDogTile : InteractableTile
	{
		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public HotDogTile(Vector2 position) : base(position)
		{
		}



		/// <summary>
		/// Load all textures and assets
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public override void LoadContent(ContentManager content)
		{
			mTexture = content.Load<Texture2D>("Tiles/hotdog");
		}



		/// <summary>
		/// Give the player a life when intersecting this tile.
		/// </summary>
		public override void OnEntityIntersect(Entity entity)
		{
			if (entity is Arnold)
			{
				int livesBefore = ProgressManager.I.pLives;
				ProgressManager.I.GiveLife();
				mEnabled = false;

				//If there is actually a life increase
				if (livesBefore < ProgressManager.I.pLives)
				{
					FXManager.I.AddTextScroller(FontManager.I.GetFont("Pixica Micro-24"), Color.OliveDrab, entity.pPosition, "+1 Life");
				}
				else
				{
					FXManager.I.AddTextScroller(FontManager.I.GetFont("Pixica Micro-24"), Color.White, entity.pPosition, "+0 Lives");
				}
			}

			base.OnEntityIntersect(entity);
		}
	}





	/// <summary>
	/// Water bottle tile. Needed to complete most levels.
	/// </summary>
	class WaterBottleTile : CollectableTile
	{
		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public WaterBottleTile(Vector2 position) : base(position)
		{
		}



		/// <summary>
		/// Load all textures and assets
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public override void LoadContent(ContentManager content)
		{
			mTexture = content.Load<Texture2D>("Tiles/bottle");
		}

		protected override CollectableType GetCollectableType()
		{
			return CollectableType.WaterBottle;
		}
	}
}
