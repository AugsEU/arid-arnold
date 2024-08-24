namespace AridArnold
{
	/// <summary>
	/// Describing an item you can buy
	/// </summary>
	abstract class Item
	{
		#region rTypes

		public enum ItemType
		{
			RedKey,
			HotDogPlant
		}

		#endregion rTypes





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
		public virtual void ActiveUpdate(GameTime gameTime, Arnold arnoldUsingItem)
		{
			
		}



		/// <summary>
		/// Update item in your pocket.
		/// </summary>
		public virtual void InactiveUpdate(GameTime gameTime)
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
		public virtual bool CanUseItem(Arnold arnoldUsingItem)
		{
			return arnoldUsingItem.CanUseItem();
		}



		/// <summary>
		/// Should we get this item back after death?
		/// </summary>
		public virtual bool RegenerateAfterDeath()
		{
			return true;
		}

		#endregion rUtil





		#region rFactory

		/// <summary>
		/// Item factory
		/// </summary>
		public static Item CreateItem(ItemType type)
		{
			switch (type)
			{
				case ItemType.RedKey:
					return new RedKey();
				case ItemType.HotDogPlant:
					return new HotDogPlant();
			}

			throw new NotImplementedException();
		}

		#endregion rFactory
	}

}
