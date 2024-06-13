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
			if (entity.OnInteractLayer(InteractionLayer.kPlayer))
			{
				int livesBefore = CampaignManager.I.GetLives();
				CampaignManager.I.GainLife();
				mEnabled = false;

				string lifeStr;
				Color tickerColor;

				//If there is actually a life increase
				if (livesBefore < CampaignManager.I.GetLives())
				{
					lifeStr = LanguageManager.I.GetText("InGame.LifeGain");
					tickerColor = Color.OliveDrab;
				}
				else
				{
					lifeStr = LanguageManager.I.GetText("InGame.NoLifeGain");
					tickerColor = Color.Wheat;
				}

				FXManager.I.AddTextScroller(FontManager.I.GetFont("PixicaMicro-24"), tickerColor, entity.GetPos(), lifeStr);
			}

			base.OnEntityIntersect(entity);
		}
	}
}
