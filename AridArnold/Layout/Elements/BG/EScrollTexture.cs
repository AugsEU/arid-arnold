
namespace AridArnold
{
	class EScrollTexture : LayElement
	{
		Texture2D mTexture;
		Vector2 mScrollSpeed;
		Vector2 mBasePosition;

		public EScrollTexture(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			mTexture = MonoParse.GetTexture(rootNode["texture"]);
			mScrollSpeed = MonoParse.GetVector(rootNode["speed"]);
			mBasePosition = Vector2.Zero;

			MonoRandom drawRandom = RandomManager.I.GetDraw();
			if (mScrollSpeed.X != 0.0f)
			{
				mBasePosition.X = drawRandom.GetFloatRange(-mTexture.Width, -0.1f);
			}

			if (mScrollSpeed.Y != 0.0f)
			{
				mBasePosition.Y = drawRandom.GetFloatRange(-mTexture.Height, -0.1f);
			}
		}

		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);

			mBasePosition += dt * mScrollSpeed;

			if (mBasePosition.X > 0.0f)
			{
				mBasePosition.X -= mTexture.Width;
			}
			else if (mBasePosition.Y > 0.0f)
			{
				mBasePosition.Y -= mTexture.Height;
			}
		}

		public override void Draw(DrawInfo info)
		{
			int xDrawNum = 1;
			int yDrawNum = 1;

			if (mScrollSpeed.X != 0.0f)
			{
				xDrawNum = 1 + (Screen.SCREEN_WIDTH / mTexture.Width);
			}

			if (mScrollSpeed.Y != 0.0f)
			{
				yDrawNum = 1 + (Screen.SCREEN_HEIGHT / mTexture.Height);
			}

			for (int x = 0; x < xDrawNum; x++)
			{
				for (int y = 0; y < yDrawNum; y++)
				{
					Vector2 drawPos = GetPosition() + mBasePosition;
					drawPos.X += x * mTexture.Width;
					drawPos.Y += y * mTexture.Height;
					MonoDraw.DrawTextureDepth(info, mTexture, drawPos, GetDepth());
				}
			}
		}
	}
}
