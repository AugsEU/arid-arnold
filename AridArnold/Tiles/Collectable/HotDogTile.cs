using AridArnold.Tiles.Basic;

namespace AridArnold
{
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
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/HotDog");
		}



		/// <summary>
		/// Give the player a life when intersecting this tile.
		/// </summary>
		public override void OnEntityIntersect(Entity entity)
		{
			if (MonoAlg.TestFlag(entity.GetInteractionLayer(), InteractionLayer.kPlayer))
			{
				int livesBefore = CampaignManager.I.GetLives();
				CampaignManager.I.GainLife();
				mEnabled = false;

				//If there is actually a life increase
				if (livesBefore < CampaignManager.I.GetLives())
				{
					FXManager.I.AddTextScroller(FontManager.I.GetFont("Pixica Micro-24"), Color.OliveDrab, entity.GetPos(), "+1 Life");
				}
				else
				{
					FXManager.I.AddTextScroller(FontManager.I.GetFont("Pixica Micro-24"), Color.White, entity.GetPos(), "+0 Lives");
				}
			}

			base.OnEntityIntersect(entity);
		}
	}
}
