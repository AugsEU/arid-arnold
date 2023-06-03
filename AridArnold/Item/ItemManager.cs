using System.Reflection.Emit;

namespace AridArnold
{
	class ItemManager : Singleton<ItemManager>
	{
		#region rMembers

		Item mRegenItem;
		Item mActiveItem;
		int mCoinsOwed;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Called when a level sequence starts
		/// </summary>
		public void SequenceBegin()
		{
			mCoinsOwed = 0;
		}



		/// <summary>
		/// Called when a level sequence ends.
		/// </summary>
		public void SequenceEnd(bool success)
		{
			if (success == false)
			{
				RefundMoney(mCoinsOwed);
			}
			mActiveItem = null;
		}


		/// <summary>
		/// Called when a level begins
		/// </summary>
		public void LevelBegin()
		{
			mRegenItem = mActiveItem;
		}



		/// <summary>
		/// Called when a level is over
		/// </summary>
		public void LevelEnd(bool success)
		{
			if (mRegenItem is not null && mRegenItem.RegenerateAfterDeath())
			{
				// Regenerate the item
				if (!success)
				{
					mActiveItem = mRegenItem;
				}
			}
		}

		#endregion rInit





		#region rPurchase

		/// <summary>
		/// Spend an amount of money
		/// </summary>
		void SpendMoney(int amount)
		{
			UInt16 coinID = CampaignManager.I.GetCurrCoinID();
			CollectableManager.I.ChangePermanentItem(coinID, -amount);
		}



		/// <summary>
		/// Get an amount of money back
		/// </summary>
		void RefundMoney(int amount)
		{
			SpendMoney(-amount);
		}



		/// <summary>
		/// Can we purchase this item?
		/// </summary>
		bool CanPurchase(Item item)
		{
			UInt16 coinID = CampaignManager.I.GetCurrCoinID();
			uint currentMoney = CollectableManager.I.GetCollected(coinID);

			if (mActiveItem is not null)
			{
				// Account for refund
				currentMoney += (uint)mActiveItem.GetPrice();
			}

			return currentMoney >= item.GetPrice();
		}



		/// <summary>
		/// Purchase an item
		/// </summary>
		public void PurchaseItem(Item item)
		{
			if(CanPurchase(item) == false)
			{
				// Sorry can't buy it.
				return;
			}

			if (mActiveItem is not null)
			{
				RefundItem(mActiveItem);
			}

			mActiveItem = item;
			SpendMoney(item.GetPrice());
			mCoinsOwed += item.GetPrice();
		}



		/// <summary>
		/// Refund an item
		/// </summary>
		void RefundItem(Item item)
		{
			RefundMoney(item.GetPrice());
			mCoinsOwed -= item.GetPrice();
		}

		#endregion rPurchase





		#region rUpdate

		/// <summary>
		/// Update item manager
		/// </summary>
		public void Update(GameTime gameTime)
		{
			if (mActiveItem is not null)
			{
				mActiveItem.Update(gameTime);
			}
		}

		#endregion rUpdate





		#region rUtility

		/// <summary>
		/// Get the active item, may be null.
		/// </summary>
		public Item GetActiveItem()
		{
			return mActiveItem;
		}



		/// <summary>
		/// Take the reference of the active item into your own ownership
		/// </summary>
		public Item PopActiveItem()
		{
			Item item = mActiveItem;
			mActiveItem = null;
			return item;
		}

		#endregion rUtility
	}
}
