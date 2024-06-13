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
		/// Update item info
		/// </summary>
		public virtual void Update(GameTime gameTime)
		{

		}

		/// <summary>
		/// Trigger item effect
		/// </summary>
		public abstract void UseItem(Arnold arnoldUsingItem);

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
