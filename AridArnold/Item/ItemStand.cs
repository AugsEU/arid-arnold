using static AridArnold.Item;

namespace AridArnold
{
	/// <summary>
	/// Entity holding an item
	/// </summary>
	internal class ItemStand : Entity
	{
		#region rConstants

		static Vector2 INFO_BUBBLE_OFFSET = new Vector2(8.0f, -28.0f);
		static Vector2 SPENDING_TICKER_OFFSET = new Vector2(54.0f, 0.0f);
		const float ANGULAR_SPEED = 0.5f;
		const float AMPLITUDE = 1.5f;

		#endregion rConstants





		#region rMembers

		int mItemType;
		Item mDisplayItem;
		float mAngle;
		SpriteFont mFont;
		ItemStandInfoBubble mInfoBubble;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create item stand at point
		/// </summary>
		/// <param name="pos"></param>
		public ItemStand(Vector2 pos, int itemType, int price) : base(pos)
		{
			mItemType = itemType;
			mDisplayItem = Item.CreateItem(mItemType, price);

			mInfoBubble = new ItemStandInfoBubble(pos + INFO_BUBBLE_OFFSET, BubbleStyle.DefaultPrompt, mDisplayItem.GetTitle(), mDisplayItem.GetDescription());
		}



		/// <summary>
		/// Load content for item stand
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/ItemStand");
			mFont = FontManager.I.GetFont("PixicaMicro-24");
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update item stand
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update(GameTime gameTime)
		{
			mDisplayItem.Update(gameTime);
			float dt = Util.GetDeltaT(gameTime);
			mAngle += ANGULAR_SPEED * dt;

			mInfoBubble.Update(gameTime, mPlayerNear);

			base.Update(gameTime);
		}



		/// <summary>
		/// Handle any inputs
		/// </summary>
		protected override void OnPlayerInteract()
		{
			if (InputManager.I.KeyPressed(InputAction.Confirm))
			{
				Item newItem = Item.CreateItem(mItemType, mDisplayItem.GetPrice());
				ItemManager.I.PurchaseItem(newItem, mPosition + SPENDING_TICKER_OFFSET);
				SFXManager.I.PlaySFX(AridArnoldSFX.BuyItem, 0.1f);
			}
		}



		/// <summary>
		/// Deal with entity collisions.
		/// </summary>
		public override void OnCollideEntity(Entity entity)
		{
			if (entity is Arnold)
			{
				Arnold arnold = (Arnold)entity;
				mPlayerNear = arnold.CanBuyItem();
			}

			base.OnCollideEntity(entity);
		}



		/// <summary>
		/// Collider bounds
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			return new Rect2f(mPosition, Tile.sTILE_SIZE, Tile.sTILE_SIZE);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the item stand
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			MonoDraw.DrawTexture(info, mTexture, mPosition);

			Vector2 itemPos = mPosition;
			Texture2D itemTex = mDisplayItem.GetTexture();
			itemPos.Y -= itemTex.Height + AMPLITUDE * MathF.Sin(mAngle) - 7.0f;
			itemPos.X += (mTexture.Width - itemTex.Width) / 2.0f;
			MonoDraw.DrawTexture(info, itemTex, itemPos);

			Vector2 pricePos = itemPos;
			Color priceCol = ItemManager.I.CanPurchase(mDisplayItem) ? Color.White : Color.Red;
			pricePos.Y -= 10.0f;
			pricePos.X = mPosition.X + mTexture.Width / 2.0f;
			MonoDraw.DrawStringCentred(info, mFont, pricePos, priceCol, mDisplayItem.GetPrice().ToString(), DrawLayer.Default);

			mInfoBubble.Draw(info);
		}

		#endregion rDraw
	}
}
