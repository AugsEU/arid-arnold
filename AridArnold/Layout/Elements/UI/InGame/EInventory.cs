namespace AridArnold
{
	class EInventory : UIPanelBase
	{
		private struct KeyItemUI
		{
			public KeyItemUI(Texture2D tex, string locString)
			{
				mTexture = tex;
				mLocString = locString;
			}

			public Texture2D mTexture;
			public string mLocString;
		}

		static Vector2 WATER_BOTTLE_OFFSET = new Vector2(20.0f, 33.0f);
		static Vector2 KEY_OFFSET = new Vector2(80.0f, 33.0f);
		static Vector2 COIN_OFFSET = new Vector2(138.0f, 33.0f);
		static Vector2 COLLECTIBLE_TEXT_ORIGIN = new Vector2(37.0f, 79.0f);

		static Vector2 KEY_ITEM_OFFSET = new Vector2(8.0f, 93.0f);
		static float KEY_ITEM_DISPLACEMENT = 13.0f;

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
		KeyItemUI[] mKeyItemInfo;

		public EInventory(XmlNode rootNode) : base(rootNode, "UI/InGame/InventoryBG")
		{
			mLargeFont = FontManager.I.GetFont("Pixica", 24, true);
			mSmallFont = FontManager.I.GetFont("Pixica", 12, true);

			mWaterBottleTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Bottle");
			mKeyTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/KeyFull");
			mCoinTexture = null;

			mPrevLevel = null;

			// Fill out key item info
			UInt32 maxFlags = (UInt32)KeyItemFlagType.kMaxKeyItems;
			mKeyItemInfo = new KeyItemUI[maxFlags];
			for(UInt32 i = 0; i < maxFlags; i++)
			{
				KeyItemFlagType itemEnum = (KeyItemFlagType)i;
				Texture2D itemTex = FlagTypeHelpers.LoadKeyItemTexture(itemEnum);
				string locStringID = FlagTypeHelpers.GetKeyItemNameID(itemEnum);
				string locString = LanguageManager.I.GetText(locStringID);

				mKeyItemInfo[i] = new KeyItemUI(itemTex, locString);
			}
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

			DrawKeyItems(info);
		}

		/// <summary>
		/// Draw the key items list
		/// </summary>
		void DrawKeyItems(DrawInfo info)
		{
			Vector2 pos = GetPosition() + KEY_ITEM_OFFSET;
			for (int i = 0; i < (int)KeyItemFlagType.kMaxKeyItems; i++)
			{
				KeyItemFlagType itemEnum = (KeyItemFlagType)i;

				bool hasItem = FlagsManager.I.CheckFlag(FlagCategory.kKeyItems, (UInt32)itemEnum);

				if (hasItem)
				{
					ref KeyItemUI keyItemUI = ref mKeyItemInfo[i];
					MonoDraw.DrawTextureDepth(info, keyItemUI.mTexture, pos, GetDepth());
					MonoDraw.DrawString(info, mSmallFont, keyItemUI.mLocString, pos + new Vector2(19.0f, 0.0f), PANEL_WHITE, GetDepth());
				}

				pos.Y += KEY_ITEM_DISPLACEMENT;
			}
		}
	}
}
