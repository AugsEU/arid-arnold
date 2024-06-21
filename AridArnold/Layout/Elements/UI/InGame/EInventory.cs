namespace AridArnold
{
	class EInventory : UIPanelBase
	{
		static Vector2 WATER_BOTTLE_OFFSET = new Vector2(20.0f, 33.0f);
		static Vector2 KEY_OFFSET = new Vector2(80.0f, 33.0f);
		static Vector2 COIN_OFFSET = new Vector2(138.0f, 33.0f);
		static Vector2 COLLECTIBLE_TEXT_ORIGIN = new Vector2(37.0f, 79.0f);

		// Data
		Level mPrevLevel;

		// Fonts
		SpriteFont mLargeFont;
		SpriteFont mSmallFont;

		// Counted items
		Texture2D mWaterBottleTexture;
		Texture2D mKeyTexture;
		Texture2D mCoinTexture;

		// Special items

		public EInventory(XmlNode rootNode) : base(rootNode, "UI/InGame/InventoryBG")
		{
			mLargeFont = FontManager.I.GetFont("Pixica", 24, true);
			mSmallFont = FontManager.I.GetFont("Pixica", 12, true);

			mWaterBottleTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Bottle");
			mKeyTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/KeyFull");
			mCoinTexture = null;

			mPrevLevel = null;
		}

		public override void Update(GameTime gameTime)
		{
			// Check for new coin texture
			Level currLevel = CampaignManager.I.GetCurrentLevel();
			if (object.ReferenceEquals(currLevel, mPrevLevel) == false)
			{
				if (currLevel is null)
				{
					// Unload coin anim
					mCoinTexture = null;
				}
				else
				{
					// Level changed, reload coin animation
					mCoinTexture = MonoData.I.MonoGameLoad<Texture2D>("CoinFull");
				}
				mPrevLevel = currLevel;
			}
			base.Update(gameTime);
		}

		public override void Draw(DrawInfo info)
		{
			base.Draw(info);

			Vector2 textPos = COLLECTIBLE_TEXT_ORIGIN + GetPosition();

			// Water
			int numWater = (int)CollectableManager.I.GetNumCollected(CollectableCategory.WaterBottle);
			MonoDraw.DrawTextureDepthScale(info, mWaterBottleTexture, WATER_BOTTLE_OFFSET + GetPosition(), 2.0f, GetDepth());
			MonoDraw.DrawStringCentred(info, mLargeFont, textPos, PANEL_WHITE, numWater.ToString(), GetDepth());
			textPos.X += 59.0f;

			// Keys
			int numKeys = (int)CollectableManager.I.GetNumCollected((UInt16)CollectableCategory.Key);
			MonoDraw.DrawTextureDepthScale(info, mKeyTexture, KEY_OFFSET + GetPosition(), 2.0f, GetDepth());
			MonoDraw.DrawStringCentred(info, mLargeFont, textPos, PANEL_WHITE, numKeys.ToString(), GetDepth());
			textPos.X += 59.0f;

			// Coins
			if (mCoinTexture is not null)
			{
				int numCoins = (int)CollectableManager.I.GetNumCollected(CampaignManager.I.GetCurrCoinID());
				string coinString = string.Format("{0}$", numCoins);

				SpriteFont waterFont = mLargeFont;
				if(coinString.Length >= 5)
				{
					waterFont = mSmallFont;
				}

				MonoDraw.DrawTextureDepthScale(info, mCoinTexture, COIN_OFFSET + GetPosition(), 2.0f, GetDepth());
				MonoDraw.DrawStringCentred(info, waterFont, textPos, PANEL_WHITE, coinString, GetDepth());
			}
			
		}
	}
}
