namespace AridArnold
{
	/// <summary>
	/// Describing an item you can buy
	/// </summary>
	abstract class Item
	{
		public enum ItemType
		{
			RedKey,
			HotDogPlant
		}

		protected  Texture2D mTexture;

		public abstract void UseItem();

		public abstract int GetPrice();

		public virtual bool CanUseItem()
		{
			return true;
		}

		public virtual void Update(GameTime gameTime)
		{

		}

		public Texture2D GetTexture()
		{
			return mTexture;
		}

		public virtual bool RegenerateAfterDeath()
		{
			return true;
		}

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
	}

}
