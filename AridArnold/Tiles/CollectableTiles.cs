namespace AridArnold
{
	/// <summary>
	/// A tile you can collect
	/// </summary>
	abstract class CollectableTile : InteractableTile
	{
		protected abstract CollectibleType GetCollectibleType();

		public override void OnEntityIntersect(Entity entity, Rect2f bounds)
		{
			if (entity is Arnold)
			{
				CollectibleManager.I.CollectItem(GetCollectibleType());
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
		/// Load all textures
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public override void LoadContent(ContentManager content)
		{
			mTexture = content.Load<Texture2D>("Tiles/flag");
		}

		public override void OnEntityIntersect(Entity entity, Rect2f bounds)
		{
			if (entity is Arnold)
			{
				ProgressManager.I.ReportCheckpoint();
			}
			base.OnEntityIntersect(entity, bounds);
		}

		protected override CollectibleType GetCollectibleType()
		{
			return CollectibleType.Flag;
		}
	}





	/// <summary>
	/// Hot dog tile. Gives the player 1 life
	/// </summary>
	class HotDogTile : InteractableTile
	{
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
		public override void OnEntityIntersect(Entity entity, Rect2f bounds)
		{
			if (entity is Arnold)
			{
				int livesBefore = ProgressManager.I.Lives;
				ProgressManager.I.GiveLife();
				mEnabled = false;

				//If there is actually a life increase
				if (livesBefore < ProgressManager.I.Lives)
				{
					FXManager.I.AddTextScroller(FontManager.I.GetFont("Pixica Micro-24"), Color.OliveDrab, entity.pPosition, "+1 Life");
				}
				else
				{
					FXManager.I.AddTextScroller(FontManager.I.GetFont("Pixica Micro-24"), Color.White, entity.pPosition, "+0 Lives");
				}
			}
			base.OnEntityIntersect(entity, bounds);
		}
	}





	/// <summary>
	/// Water bottle tile. Needed to complete most levels.
	/// </summary>
	class WaterBottleTile : CollectableTile
	{
		/// <summary>
		/// Load all textures and assets
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public override void LoadContent(ContentManager content)
		{
			mTexture = content.Load<Texture2D>("Tiles/bottle");
		}

		protected override CollectibleType GetCollectibleType()
		{
			return CollectibleType.WaterBottle;
		}
	}
}
