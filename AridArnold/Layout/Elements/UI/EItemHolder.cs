using static System.Formats.Asn1.AsnWriter;

namespace AridArnold
{
	internal class EItemHolder : LayElement
	{
		Texture2D mFrame;
		Animator mItemAnim;
		Item mCurrItem;

		public EItemHolder(XmlNode rootNode) : base(rootNode)
		{
			mFrame = MonoData.I.MonoGameLoad<Texture2D>("UI/ItemFrame");
			mItemAnim = null;
			mCurrItem = null;
		}

		public override void Update(GameTime gameTime)
		{
			// Check for new item
			Item newItem = ItemManager.I.GetActiveItem();
			if(!object.ReferenceEquals(newItem, mCurrItem))
			{
				if(newItem is not null)
				{
					mItemAnim = newItem.GenerateAnimator();
				}
				else
				{
					mItemAnim = null;
				}
				mCurrItem = newItem;
			}

			if(mItemAnim is not null)
			{
				mItemAnim.Update(gameTime);
			}
			
			base.Update(gameTime);
		}

		public override void Draw(DrawInfo info)
		{
			const float SCALE = 5.0f;
			MonoDraw.DrawTextureDepth(info, mFrame, mPos, mDepth);

			if(mItemAnim is not null)
			{
				Vector2 itemPos = mPos;
				Texture2D itemTex = mItemAnim.GetCurrentTexture();
				itemPos.X += (mFrame.Width - SCALE * itemTex.Width) / 2.0f;
				itemPos.Y += (mFrame.Height - SCALE * itemTex.Height) / 2.0f;
				MonoDraw.DrawTexture(info, mItemAnim.GetCurrentTexture(), itemPos, null, Color.White, 0.0f, Vector2.Zero, SCALE, SpriteEffects.None, mDepth);
			}

			base.Draw(info);
		}
	}
}
