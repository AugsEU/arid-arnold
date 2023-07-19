namespace AridArnold
{
	internal class EItemHolder : LayElement
	{
		Texture2D mFrame;
		Item mCurrItem;

		public EItemHolder(XmlNode rootNode) : base(rootNode)
		{
			mFrame = MonoData.I.MonoGameLoad<Texture2D>("UI/ItemFrame");
			mCurrItem = null;
		}

		public override void Update(GameTime gameTime)
		{
			// Check for new item
			Item newItem = ItemManager.I.GetActiveItem();
			if(!object.ReferenceEquals(newItem, mCurrItem))
			{
				mCurrItem = newItem;
			}
			
			base.Update(gameTime);
		}

		public override void Draw(DrawInfo info)
		{
			const float SCALE = 5.0f;
			MonoDraw.DrawTextureDepth(info, mFrame, GetPosition(), GetDepth());

			if(mCurrItem is not null)
			{
				Vector2 itemPos = GetPosition();
				Texture2D itemTex = mCurrItem.GetTexture();
				itemPos.X += (mFrame.Width - SCALE * itemTex.Width) / 2.0f;
				itemPos.Y += (mFrame.Height - SCALE * itemTex.Height) / 2.0f;
				MonoDraw.DrawTexture(info, itemTex, itemPos, null, Color.White, 0.0f, Vector2.Zero, SCALE, SpriteEffects.None, GetDepth());
			}

			base.Draw(info);
		}
	}
}
