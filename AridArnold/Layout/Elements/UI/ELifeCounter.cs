namespace AridArnold
{
	class ELifeCounter : LayElement
	{
		const float SPACING = 30.0f;

		Texture2D mLifeTexture;
		Texture2D mEmptyTexture;

		public ELifeCounter(XmlNode rootNode) : base(rootNode)
		{
			mLifeTexture = MonoData.I.MonoGameLoad<Texture2D>("UI/ArnoldLife");
			mEmptyTexture = MonoData.I.MonoGameLoad<Texture2D>("UI/ArnoldLifeEmpty");
		}

		public override void Draw(DrawInfo info)
		{
			if(CampaignManager.I.CanLoseLives() == false)
			{
				// Don't draw
				return;
			}

			int currLives = CampaignManager.I.GetLives();
			int maxLives = CampaignManager.MAX_LIVES;
			Vector2 position = mPos;

			for(int i = 0; i < maxLives; i++)
			{
				if(i < currLives)
				{
					MonoDraw.DrawTextureDepth(info, mLifeTexture, position, mDepth);
				}
				else
				{
					MonoDraw.DrawTextureDepth(info, mEmptyTexture, position, mDepth);
				}

				position.Y += SPACING;
			}

			base.Draw(info);
		}
	}
}
