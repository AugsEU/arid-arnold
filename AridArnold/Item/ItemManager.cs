namespace AridArnold
{
	class ItemManager : Singleton<ItemManager>
	{
		#region rMembers

		Item mItemAtLevelStart;
		Item mActiveItem;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Reset this completely
		/// </summary>
		public void ResetToDefault()
		{
			mItemAtLevelStart = null;
			mActiveItem = null;
		}

		/// <summary>
		/// Called when a level sequence ends.
		/// </summary>
		public void SequenceEnd()
		{
			mActiveItem = null;
		}


		/// <summary>
		/// Called when a level begins
		/// </summary>
		public void LevelBegin()
		{
			mItemAtLevelStart = mActiveItem;
		}



		/// <summary>
		/// Called when a level is over
		/// </summary>
		public void LevelEnd(bool success)
		{
			if (mItemAtLevelStart is not null && mItemAtLevelStart.RegenerateAfterDeath())
			{
				// Regenerate the item
				if (!success)
				{
					mActiveItem = mItemAtLevelStart;
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
			CollectableManager.I.IncPermanentCount(coinID, -amount);
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
			uint currentMoney = CollectableManager.I.GetNumCollected(coinID);

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
		public void PurchaseItem(Item item, Vector2 tickerPos)
		{
			if (CanPurchase(item) == false)
			{
				// Sorry can't buy it.
				return;
			}


			// Already have this item.
			if(MonoAlg.TypeCompare(item, mActiveItem))
			{
				return;
			}

			if (mActiveItem is not null)
			{
				string refundStr = LanguageManager.I.GetText("InGame.Refund") + mActiveItem.GetPrice();
				FXManager.I.AddTextScroller(FontManager.I.GetFont("PixicaMicro-24"), Color.DeepSkyBlue, tickerPos, refundStr, 2.0f, 10.0f, 40.0f);
				tickerPos.Y += 14.0f;

				RefundItem(mActiveItem);
			}

			mActiveItem = item;
			SpendMoney(item.GetPrice());

			string spendStr = LanguageManager.I.GetText("InGame.Spent") + mActiveItem.GetPrice();
			FXManager.I.AddTextScroller(FontManager.I.GetFont("PixicaMicro-24"), Color.Crimson, tickerPos, spendStr, 2.0f, 10.0f, 40.0f);
		}



		/// <summary>
		/// Refund an item
		/// </summary>
		void RefundItem(Item item)
		{
			RefundMoney(item.GetPrice());
		}

		#endregion rPurchase





		#region rUpdate

		/// <summary>
		/// Update item manager
		/// </summary>
		public void Update(GameTime gameTime)
		{
			if (mActiveItem is not null && !mActiveItem.IsActive())
			{
				mActiveItem.InactiveUpdate(gameTime);
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
