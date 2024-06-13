namespace AridArnold
{
	class ECollectableCounter : LayElement
	{
		const float SPACING = 50.0f;
		SpriteFont mFont;
		Texture2D mKeyTexture;
		Level mCurrLevel;
		Animator mCoinAnim;

		public ECollectableCounter(XmlNode rootNode) : base(rootNode)
		{
			mFont = FontManager.I.GetFont("Pixica-24");
			mKeyTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/KeyFull");
			mCurrLevel = null;
		}

		public override void Update(GameTime gameTime)
		{
			// Check for new coin texture
			Level currLevel = CampaignManager.I.GetCurrentLevel();
			if (object.ReferenceEquals(currLevel, mCurrLevel) == false)
			{
				if (currLevel is null)
				{
					// Unload coin anim
					mCoinAnim = null;
				}
				else
				{
					// Level changed, reload coin animation
					mCoinAnim = MonoData.I.LoadAnimator("CoinFull");
				}
				mCurrLevel = currLevel;
			}
			base.Update(gameTime);
		}

		public override void Draw(DrawInfo info)
		{
			int numKeys = (int)CollectableManager.I.GetNumCollected((UInt16)PermanentCollectable.Key);
			Vector2 position = GetPosition();

			DrawCollectable(info, position, mKeyTexture, numKeys);
			position.Y += SPACING;
			if (mCoinAnim != null)
			{
				int numCoins = (int)CollectableManager.I.GetNumCollected(CampaignManager.I.GetCurrCoinID());
				DrawCollectable(info, position, mCoinAnim.GetCurrentTexture(), numCoins);
				position.Y += SPACING;
			}


			base.Draw(info);
		}

		void DrawCollectable(DrawInfo info, Vector2 position, Texture2D icon, int number)
		{
			const float SCALE = 4.0f;
			string displayText = " x " + number.ToString();
			Vector2 textureSize = mFont.MeasureString(displayText);
			Vector2 offset = new Vector2(5.0f, 0.0f);
			offset.X = icon.Width * SCALE;
			offset.Y = (icon.Height * SCALE - textureSize.Y) / 2.0f;


			MonoDraw.DrawTexture(info, icon, position, null, Color.White, 0.0f, Vector2.Zero, SCALE, SpriteEffects.None, GetDepth());
			MonoDraw.DrawString(info, mFont, displayText, position + offset, Color.Wheat, DrawLayer.Default);
		}
	}
}
