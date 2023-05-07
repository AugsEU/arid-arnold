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
		}

		public abstract void UseItem();

		public abstract int GetPrice();

		public virtual bool CanUseItem()
		{
			return true;
		}

		public virtual void Update(GameTime gameTime)
		{

		}

		public abstract Animator GenerateAnimator();

		public static Item CreateItem(ItemType type)
		{
			switch (type)
			{
				case ItemType.RedKey:
					return new RedKey();
			}

			throw new NotImplementedException();
		}
	}

}
