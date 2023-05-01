namespace AridArnold
{
	internal class LevelSequenceInfoBubble : InfoBubble
	{
		const int BORDER_SIZE = 10;
		const int ICON_WIDTH = 11;
		const int ICON_SPACING = 5;

		List<Level> mLevelSequence;
		Texture2D mLevelIcon;
		Texture2D mShopIcon;
		Texture2D mWaterIcon;
		Texture2D mFlagIcon;

		public LevelSequenceInfoBubble(List<Level> levelSequence, Vector2 botCentre, BubbleStyle style) : base(botCentre, style)
		{
			mLevelSequence = levelSequence;

			SetTargetSize(2*BORDER_SIZE + mLevelSequence.Count * (ICON_WIDTH + ICON_SPACING) - ICON_SPACING, 20);

			mLevelIcon = MonoData.I.MonoGameLoad<Texture2D>("Shared/Icons/LevelDefault");
			mShopIcon = MonoData.I.MonoGameLoad<Texture2D>("Shared/Icons/LevelShop");
			mWaterIcon = MonoData.I.MonoGameLoad<Texture2D>("Shared/Icons/LevelWater");
			mFlagIcon = MonoData.I.MonoGameLoad<Texture2D>("Shared/Icons/LevelFlag");

		}

		protected override void DrawInner(DrawInfo info, Rectangle area)
		{
			Vector2 offset = new Vector2(BORDER_SIZE, 2) + new Vector2(area.X, area.Y);

			for(int i = 0; i < mLevelSequence.Count; i++)
			{
				AuxData.LevelType levelType = mLevelSequence[i].GetAuxData().GetLevelType();

				Texture2D levelIconTex = GetIconForLevel(levelType);

				MonoDraw.DrawTextureDepth(info, levelIconTex, offset, DrawLayer.Bubble);

				offset.X += ICON_SPACING + ICON_WIDTH;
			}
		}
		Texture2D GetIconForLevel(AuxData.LevelType levelType)
		{
			switch (levelType)
			{
				case AuxData.LevelType.CollectWater:
					return mWaterIcon;
				case AuxData.LevelType.CollectFlag:
					return mFlagIcon;
				case AuxData.LevelType.Shop:
					return mShopIcon;
			}

			return mLevelIcon;
		}

		protected override void UpdateInternal()
		{
		}
	}
}
