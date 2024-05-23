namespace HorsesAndGun
{
	internal class ScrollingImage
	{
		private Texture2D mTexture1, mTexture2;
		private Rectangle mTexture1Bounds, mTexture2Bounds;
		private Vector2 mPosition;
		private int mScrollSpeed;

		public ScrollingImage(Texture2D texture1, Texture2D texture2, Vector2 position, int scrollSpeed)
		{
			mTexture1 = texture1;
			mTexture2 = texture2;
			mPosition = position;
			mScrollSpeed = scrollSpeed;

			mTexture1Bounds = new Rectangle((int)mPosition.X, (int)mPosition.Y, mTexture1.Width, mTexture1.Height);
			mTexture2Bounds = new Rectangle((int)mPosition.X + mTexture1Bounds.Width, (int)mPosition.Y, mTexture2.Width, mTexture2.Height);
		}

		private void ScrollImages()
		{
			if (mTexture1Bounds.X + mTexture1Bounds.Width <= mPosition.X)
			{
				mTexture1Bounds.X = mTexture2Bounds.X + mTexture2Bounds.Width;
			}
			if (mTexture2Bounds.X + mTexture2Bounds.Width <= mPosition.X)
			{
				mTexture2Bounds.X = mTexture1Bounds.X + mTexture1Bounds.Width;
			}
		}

		public void Update(GameTime gameTime)
		{
			mTexture1Bounds.X -= (int)(mScrollSpeed * Util.GetDeltaT(gameTime) / 10.0f);
			mTexture2Bounds.X -= (int)(mScrollSpeed * Util.GetDeltaT(gameTime) / 10.0f);
			ScrollImages();
		}

		public void Draw(DrawInfo info)
		{
			info.spriteBatch.Draw(mTexture1, mTexture1Bounds, Color.White);
			info.spriteBatch.Draw(mTexture2, mTexture2Bounds, Color.White);
		}

	}
}
