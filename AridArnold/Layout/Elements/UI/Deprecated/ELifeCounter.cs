#if DEPRECATED_UI
namespace AridArnold
{
	class ELifeCounter : LayElement
	{
		const float SPACING = 38.0f;

		Texture2D mLifeTexture;
		Texture2D mEmptyTexture;

		public ELifeCounter(XmlNode rootNode) : base(rootNode)
		{
			mLifeTexture = MonoData.I.MonoGameLoad<Texture2D>("UI/ArnoldLife");
			mEmptyTexture = MonoData.I.MonoGameLoad<Texture2D>("UI/ArnoldLifeEmpty");
		}

		public override void Draw(DrawInfo info)
		{
			if (CampaignManager.I.CanLoseLives() == false)
			{
				// Don't draw
				return;
			}

			int currLives = CampaignManager.I.GetLives();
			int maxLives = CampaignManager.MAX_LIVES;
			Vector2 position = GetPosition();

			for (int i = 0; i < maxLives; i++)
			{
				Texture2D toDraw = i < currLives ? mLifeTexture : mEmptyTexture;
				MonoDraw.DrawTexture(info, toDraw, position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, GetDepth());

				position.Y += SPACING;
			}

			base.Draw(info);
		}
	}
}
#endif // DEPRECATED_UI
