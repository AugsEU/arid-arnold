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
			typeof(RedKey),		// 0
			typeof(HotDogPlant),
			typeof(JetPack),
			typeof(RatPoison),
			typeof(MushOil),
			typeof(PocketOrb),	// 5
			typeof(Ivermectin),
			typeof(BorgChip),
			typeof(SlowWatch),
			typeof(ReverseWatch)
		};

		#endregion rConstant





		#region rMembers

		protected Texture2D mTexture;
		protected string mTitle;
		protected string mDescription;
		bool mActive;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create item with base description
		/// </summary>
		public Item(string titleID, string descID)
		{
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
		public abstract int GetPrice();



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

		#endregion rUtil





		#region rFactory

		/// <summary>
		/// Item factory
		/// </summary>
		public static Item CreateItem(int type)
		{
			Type itemType = ITEM_TYPES[type];
			return (Item)Activator.CreateInstance(itemType);
		}

		#endregion rFactory
	}

}
