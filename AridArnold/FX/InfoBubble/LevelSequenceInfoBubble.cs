namespace AridArnold
{
	internal class LevelSequenceInfoBubble : InfoBubble
	{
		const int BORDER_SIZE = 10;
		const int ICON_WIDTH = 11;
		const int ICON_SPACING = 5;

		// Public static hack
		static Texture2D mLevelIcon = null;
		static Texture2D mShopIcon = null;
		static Texture2D mWaterIcon = null;
		static Texture2D mFlagIcon = null;
		static Texture2D mFountainIcon = null;
		static Texture2D mEmptyIcon = null;

		Texture2D mLevelCircle;
		Texture2D mLevelCircleComp;

		bool mSeqCompleted;

		List<Level> mLevelSequence;


		public LevelSequenceInfoBubble(List<Level> levelSequence, Vector2 botCentre, BubbleStyle style) : base(botCentre, style)
		{
			mLevelSequence = levelSequence;

			SetTargetSize(2 * BORDER_SIZE + mLevelSequence.Count * (ICON_WIDTH + ICON_SPACING) - ICON_SPACING, 20);

			mLevelIcon = MonoData.I.MonoGameLoad<Texture2D>("Shared/Icons/LevelEmpty");
			mShopIcon = MonoData.I.MonoGameLoad<Texture2D>("Shared/Icons/LevelShop");
			mWaterIcon = MonoData.I.MonoGameLoad<Texture2D>("Shared/Icons/LevelWater");
			mFlagIcon = MonoData.I.MonoGameLoad<Texture2D>("Shared/Icons/LevelKey");
			mEmptyIcon = MonoData.I.MonoGameLoad<Texture2D>("Shared/Icons/LevelEmpty");
			mFountainIcon = MonoData.I.MonoGameLoad<Texture2D>("Shared/Icons/LevelFountain");

			mLevelCircle = MonoData.I.MonoGameLoad<Texture2D>("Shared/Icons/LevelCircle");
			mLevelCircleComp = MonoData.I.MonoGameLoad<Texture2D>("Shared/Icons/LevelCircleComp");
			mSeqCompleted = false;
		}

		protected override void DrawInner(DrawInfo info, Rectangle area)
		{
			Vector2 offset = new Vector2(BORDER_SIZE, 2) + new Vector2(area.X, area.Y);

			for (int i = 0; i < mLevelSequence.Count; i++)
			{
				AuxData.LevelType levelType = mLevelSequence[i].GetAuxData().GetLevelType();

				Texture2D levelIconTex = GetIconForLevel(levelType);

				MonoDraw.DrawTextureDepth(info, levelIconTex, offset, DrawLayer.Bubble);

				Texture2D circleTex = mSeqCompleted ? mLevelCircleComp : mLevelCircle;
				MonoDraw.DrawTextureDepth(info, circleTex, offset + new Vector2(0.0f, 9.0f), DrawLayer.Bubble);

				offset.X += ICON_SPACING + ICON_WIDTH;
			}
		}
		public static Texture2D GetIconForLevel(AuxData.LevelType levelType)
		{
			switch (levelType)
			{
				case AuxData.LevelType.CollectWater:
					return mWaterIcon;
				case AuxData.LevelType.CollectKey:
					return mFlagIcon;
				case AuxData.LevelType.Shop:
					return mShopIcon;
				case AuxData.LevelType.Empty:
					return mEmptyIcon;
				case AuxData.LevelType.Fountain:
					return mFountainIcon;
			}

			return mLevelIcon;
		}

		protected override void UpdateInternal()
		{
		}

		public void SetSeqCompleted(bool comp)
		{
			mSeqCompleted = comp;
		}
	}
}
