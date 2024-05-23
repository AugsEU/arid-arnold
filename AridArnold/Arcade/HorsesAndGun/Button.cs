using System;

namespace HorsesAndGun
{
	internal class Button
	{
		private MouseState mCurrentMouse, mPreviousMouse;
		private Texture2D mTexture;
		private SpriteFont mFont;
		private bool mIsMouseOverButton;

		public EventHandler mOnMouseClick;

		public Vector2 mPosition { get; set; }

		public Rectangle mBounds
		{
			get
			{
				return new Rectangle((int)mPosition.X, (int)mPosition.Y,
					mTexture.Width, mTexture.Height);
			}
		}

		public string mText;

		public Button(Texture2D texture, SpriteFont font)
		{
			mTexture = texture;
			mFont = font;
		}

		public void CheckForButtonClick()
		{
			mIsMouseOverButton = false; // Default to false so can then be checked for

			Point mouseScreenPosition = InputManager.I.GetMousePos();
			Rectangle mousePoint = new Rectangle(mouseScreenPosition, new Point(1, 1));

			if (mousePoint.Intersects(mBounds))
			{
				mIsMouseOverButton = true;

				if (InputManager.I.LClick())
				{
					mOnMouseClick?.Invoke(this, new EventArgs());
				}
			}
		}

		public void Update()
		{
			mPreviousMouse = mCurrentMouse;
			mCurrentMouse = Mouse.GetState();
			CheckForButtonClick();
		}

		public void Draw(DrawInfo info)
		{
			Color colour = Color.White;

			if (!mIsMouseOverButton)
			{
				colour = Color.LightGray;
			}

			info.spriteBatch.Draw(mTexture, mPosition, colour);

			if (mText != null)
			{
				float x = (mBounds.X + (mBounds.Width / 2)) - (mFont.MeasureString(mText).X / 2);
				float y = (mBounds.Y + (mBounds.Height / 2)) - (mFont.MeasureString(mText).Y / 2);

				info.spriteBatch.DrawString(mFont, mText, new Vector2(x, y), Color.Black);
			}
		}

	}
}
