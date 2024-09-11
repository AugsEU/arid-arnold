namespace AridArnold
{
	/// <summary>
	/// Describing an item you can buy
	/// </summary>
	abstract class Item
	{
		#region rConstant

		public static Type[] ITEM_TYPES =
		{
			typeof(RedKey),			// 0 - All worlds
			typeof(HotDogPlant),
			typeof(Trainers),
			typeof(ChaosDrive),
			typeof(JetPack),
			typeof(ArmyBoots),

			typeof(PocketOrb),		// 6 - Library

			typeof(RatPoison),		// 7 - Cave
			typeof(MushOil),

			typeof(Ivermectin),		// 9 - Lab
			typeof(BorgChip),

			typeof(TreeSeed),		// 11 - Mirror

			typeof(SlowWatch),		// 12 - Kingdom
			typeof(ReverseWatch),

			typeof(ClusterBomb),	// 14 - WW7
			typeof(MoonBoots),
		};

		#endregion rConstant





		#region rMembers

		protected Texture2D mTexture;
		protected string mTitle;
		protected string mDescription;
		public int mPrice;
		bool mActive;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create item with base description
		/// </summary>
		public Item(string titleID, string descID, int price)
		{
			mPrice = price;
			mTitle = LanguageManager.I.GetText(titleID);
			mDescription = LanguageManager.I.GetText(descID);
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Begin item usage.
		/// </summary>
		public virtual void Begin()
		{
			mActive = true;
		}



		/// <summary>
		/// Update item to do effect
		/// </summary>
		public virtual void ActOnArnold(GameTime gameTime, Arnold arnold)
		{
			
		}



		/// <summary>
		/// Update item in your pocket.
		/// </summary>
		public virtual void Update(GameTime gameTime)
		{

		}



		/// <summary>
		/// Call this to stop the item's effect.
		/// </summary>
		public void EndItem()
		{
			mActive = false;
		}



		/// <summary>
		/// Are we still active?
		/// </summary>
		public bool IsActive()
		{
			return mActive;
		}

		#endregion rUpdate



		#region rDraw

		public virtual void DrawOnArnold(DrawInfo info, Arnold arnold)
		{

		}

		#endregion rDraw



		#region rUtil

		/// <summary>
		/// Get texture to draw
		/// </summary>
		public Texture2D GetTexture()
		{
			return mTexture;
		}



		/// <summary>
		/// Get item name string
		/// </summary>
		public virtual string GetTitle()
		{
			return mTitle;
		}



		/// <summary>
		/// Get the item's description
		/// </summary>
		public virtual string GetDescription()
		{
			return mDescription;
		}



		/// <summary>
		/// Get item's price
		/// </summary>
		public int GetPrice()
		{
			int price = mPrice;

			if(price > 0 && CampaignManager.I.IsPlayingCompletedSequence())
			{
				price = 0;
			}

			return price;
		}



		/// <summary>
		/// Can this arnold use the item?
		/// </summary>
		public virtual bool CanUseItem(Arnold arnold)
		{
			bool isShop = CampaignManager.I.GetCurrentLevelType() == AuxData.LevelType.Shop;

			if (isShop && !CanUseInShop())
			{
				return false;
			}

			return arnold.CanUseItem();
		}



		/// <summary>
		/// Should we get this item back after death?
		/// </summary>
		public virtual bool RegenerateAfterDeath()
		{
			return true;
		}



		/// <summary>
		/// Can we use this in the shop?
		/// </summary>
		public virtual bool CanUseInShop()
		{
			return false;
		}



		/// <summary>
		/// Should we refund this at the end of the sequence?
		/// </summary>
		public virtual bool RefundAtSequenceEnd()
		{
			return true;
		}

		#endregion rUtil





		#region rFactory

		/// <summary>
		/// Item factory
		/// </summary>
		public static Item CreateItem(int type, int price)
		{
			Type itemType = ITEM_TYPES[type];
			return (Item)Activator.CreateInstance(itemType, price);
		}

		#endregion rFactory
	}

}
