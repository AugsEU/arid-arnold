namespace AridArnold
{
	class EPowerItems : UIPanelBase
	{
		static Vector2 USE_FRAME_OFFSET = new Vector2(52.0f, 26.0f);
		static Vector2 ITEM_OFFSET = new Vector2(62.0f, 36.0f);

		Texture2D mItemUsableFrame;
		Item mCurrItem;

		public EPowerItems(XmlNode rootNode) : base(rootNode, "UI/InGame/PowerItemBG")
		{
			mItemUsableFrame = MonoData.I.MonoGameLoad<Texture2D>("UI/InGame/ItemUsableFrame");
			mCurrItem = null;
		}

		public override void Update(GameTime gameTime)
		{
			// Check for new item
			Item newItem = ItemManager.I.GetActiveItem();
			if (!object.ReferenceEquals(newItem, mCurrItem))
			{
				mCurrItem = newItem;
			}

			base.Update(gameTime);
		}

		bool CanAnyArnoldUseItem()
		{
			if(mCurrItem is null)
			{
				return false;
			}

			for(int e = 0; e < EntityManager.I.GetEntityNum(); e++)
			{
				Entity entity = EntityManager.I.GetEntity(e);
				if(entity is Arnold arnold)
				{
					if(mCurrItem.CanUseItem(arnold))
					{
						return true;
					}
				}
			}

			return false;
		}

		public override void Draw(DrawInfo info)
		{
			const float SCALE = 4.0f;

			base.Draw(info);

			if (mCurrItem is not null)
			{
				Texture2D itemTex = mCurrItem.GetTexture();
				if (CanAnyArnoldUseItem())
				{
					MonoDraw.DrawTextureDepth(info, mItemUsableFrame, GetPosition() + USE_FRAME_OFFSET, GetDepth());
				}

				MonoDraw.DrawTextureDepthScale(info, itemTex, GetPosition() + ITEM_OFFSET, 4.0f, GetDepth());
			}
		}
	}
}
