
namespace AridArnold
{
	class ScrollArrow : MenuButton
	{
		MenuScrollList mScrollList;
		bool mIsUp;
		bool mShow;

		public ScrollArrow(XmlNode rootNode, Layout parent, MenuScrollList scrollList, bool up) : base(rootNode, parent)
		{
			mScrollList = scrollList;

			string arrowTexPath = MonoParse.GetString(rootNode["arrowTex"], "UI/Menu/ScrollArrow");
			mTexture = MonoData.I.MonoGameLoad<Texture2D>(arrowTexPath);
		}

		public void SetShow(bool show)
		{
			mShow = show;
		}

		public bool GetShow()
		{
			return mShow;
		}

		public override void DoAction()
		{
			if(mIsUp)
			{
				mScrollList.IncrementSelected(-1);
			}
			else
			{
				mScrollList.IncrementSelected(1);
			}
		}

		protected override SpriteEffects GetSpriteEffect()
		{
			return mIsUp ? SpriteEffects.None : SpriteEffects.FlipVertically;
		}
	}
}
