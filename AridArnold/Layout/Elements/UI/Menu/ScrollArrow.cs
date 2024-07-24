
namespace AridArnold
{
	class ScrollArrow : MenuButton
	{
		MenuScrollList mScrollList;
		bool mIsUp;

		public ScrollArrow(XmlNode rootNode, Layout parent, MenuScrollList scrollList, bool up) : base(rootNode, parent)
		{
			mScrollList = scrollList;

			string arrowTexPath = MonoParse.GetString(rootNode["arrowTex"], "UI/Menu/ScrollArrow");
			mTexture = MonoData.I.MonoGameLoad<Texture2D>(arrowTexPath);

			mSize.X = mTexture.Width;
			mSize.Y = mTexture.Height;

			mIsUp = up;
		}

		public override void Draw(DrawInfo info)
		{
			base.Draw(info);
		}

		public override void DoAction()
		{
			if(mIsUp)
			{
				mScrollList.ScrollList(-1);
			}
			else
			{
				mScrollList.ScrollList(1);
			}
		}

		protected override SpriteEffects GetSpriteEffect()
		{
			return mIsUp ? SpriteEffects.None : SpriteEffects.FlipVertically;
		}
	}
}
