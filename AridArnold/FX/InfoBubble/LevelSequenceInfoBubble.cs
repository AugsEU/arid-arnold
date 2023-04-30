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

		public LevelSequenceInfoBubble(List<Level> levelSequence, Vector2 botCentre, BubbleStyle style) : base(botCentre, style)
		{
			mLevelSequence = levelSequence;

			SetTargetSize(2*BORDER_SIZE + mLevelSequence.Count * 20, 20);

			mLevelIcon = MonoData.I.MonoGameLoad<Texture2D>("Shared/Icons/LevelIcon");
			mShopIcon = MonoData.I.MonoGameLoad<Texture2D>("Shared/Icons/ShopIcon");
		}

		protected override void DrawInner(DrawInfo info, Rectangle area)
		{
			Vector2 offset = new Vector2(BORDER_SIZE, 2);

			for(int i = 0; i < mLevelSequence.Count; i++)
			{
				AuxData.LevelType levelType = mLevelSequence[i].GetAuxData().GetLevelType();

				Texture2D levelIconTex = GetIconForLevel(levelType);

				MonoDraw.DrawTextureDepth(info, levelIconTex, offset, DrawLayer.Bubble);
			}
		}
		Texture2D GetIconForLevel(AuxData.LevelType levelType)
		{
			if(levelType == AuxData.LevelType.Shop)
			{
				return mShopIcon;
			}

			return mLevelIcon;
		}

		protected override void UpdateInternal()
		{
		}
	}
}
