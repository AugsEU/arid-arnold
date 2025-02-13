﻿namespace AridArnold
{
	class EPowerItems : UIPanelBase
	{
		static Vector2 USE_FRAME_OFFSET = new Vector2(52.0f, 26.0f);
		static Vector2 ITEM_OFFSET = new Vector2(62.0f, 36.0f);
		static Vector2 TEXT_OFFSET = new Vector2(95.0f, 124.0f);
		static Vector2 BUTTON_PROMPT = new Vector2(182.0f, 2.0f);

		SpriteFont mFont;
		Texture2D mItemUsableFrame;
		Item mCurrItem;

		public EPowerItems(XmlNode rootNode, Layout parent) : base(rootNode, "UI/InGame/PowerItemBG", parent)
		{
			mItemUsableFrame = MonoData.I.MonoGameLoad<Texture2D>("UI/InGame/ItemUsableFrame");
			mFont = FontManager.I.GetFont("Pixica", 12, true);
			mCurrItem = null;
		}

		public override void Update(GameTime gameTime)
		{
			// Check for new item
			Item newItem = ItemManager.I.GetActiveItem();
			if (!object.ReferenceEquals(newItem, mCurrItem))
			{
				mCurrItem = newItem;
			}

			base.Update(gameTime);
		}

		bool CanAnyArnoldUseItem()
		{
			if(mCurrItem is null)
			{
				return false;
			}

			for(int e = 0; e < EntityManager.I.GetEntityNum(); e++)
			{
				Entity entity = EntityManager.I.GetEntity(e);
				if(entity is Arnold arnold)
				{
					if(mCurrItem.CanUseItem(arnold))
					{
						return true;
					}
				}
			}

			return false;
		}

		public override void Draw(DrawInfo info)
		{
			if (!ShouldDraw())
			{
				return;
			}

			base.Draw(info);

			if (mCurrItem is not null)
			{
				Color textColor = PANEL_WHITE;
				Texture2D itemTex = mCurrItem.GetTexture();
				if (CanAnyArnoldUseItem())
				{
					string useItemStr = LanguageManager.I.GetText("InGame.PromptItem");
					MonoDraw.DrawTextureDepth(info, mItemUsableFrame, GetPosition() + USE_FRAME_OFFSET, GetDepth());
					MonoDraw.DrawStringRight(info, mFont, BUTTON_PROMPT + GetPosition(), textColor, useItemStr, GetDepth());
					textColor = PANEL_GOLD;
				}

				MonoDraw.DrawTextureDepthScale(info, itemTex, GetPosition() + ITEM_OFFSET, 4.0f, GetDepth());

				string title = mCurrItem.GetTitle();
				MonoDraw.DrawStringCentred(info, mFont, GetPosition() + TEXT_OFFSET, textColor, title, GetDepth());
			}
		}

		protected override bool IsUnlocked()
		{
			bool flag = FlagsManager.I.CheckFlag(FlagCategory.kPanelsUnlocked, (uint)PanelUnlockedType.kPowerItem);

			return flag && base.IsUnlocked();
		}
	}
}
