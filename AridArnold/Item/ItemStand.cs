namespace AridArnold
{
	/// <summary>
	/// Entity holding an item
	/// </summary>
	internal class ItemStand : Entity
	{
		#region rConstants

		const float ANGULAR_SPEED = 0.5f;
		const float AMPLITUDE = 1.5f;

		#endregion rConstants





		#region rMembers

		Item mItem;
		Animator mItemAnimator;
		float mAngle;
		bool mPlayerNear;
		SpriteFont mFont;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create item stand at point
		/// </summary>
		/// <param name="pos"></param>
		public ItemStand(Vector2 pos, int itemType) : base(pos)
		{
			mItem = Item.CreateItem((Item.ItemType)itemType);
		}

		/// <summary>
		/// Load content for item stand
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/ItemStand");
			mItemAnimator = mItem.GenerateAnimator();
			mFont = FontManager.I.GetFont("Pixica Micro-24");
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update item stand
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);
			mItemAnimator.Update(gameTime);
			mAngle += ANGULAR_SPEED * dt;

			HandleInput();

			base.Update(gameTime);
			mPlayerNear = false;
		}



		/// <summary>
		/// Handle any inputs
		/// </summary>
		void HandleInput()
		{
			bool activate = InputManager.I.KeyHeld(AridArnoldKeys.Confirm);

			if (activate)
			{
				ItemManager.I.PurchaseItem(mItem);
			}
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
			Texture2D itemTex = mItemAnimator.GetCurrentTexture();
			itemPos.Y -= itemTex.Height + AMPLITUDE * MathF.Sin(mAngle) - 7.0f;
			itemPos.X += (mTexture.Width - itemTex.Width) / 2.0f;
			MonoDraw.DrawTexture(info, itemTex, itemPos);

			Vector2 pricePos = itemPos;
			pricePos.Y -= 10.0f;
			pricePos.X = mPosition.X + mTexture.Width / 2.0f;
			MonoDraw.DrawStringCentred(info, mFont, pricePos, Color.White, mItem.GetPrice().ToString(), DrawLayer.Default);
		}

		#endregion rDraw
	}
}
