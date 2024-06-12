
namespace AridArnold
{
	abstract class UIPanelBase : LayElement
	{
		Texture2D mPanelBG;

		protected UIPanelBase(XmlNode rootNode, string bgPath) : base(rootNode)
		{
			mPanelBG = MonoData.I.MonoGameLoad<Texture2D>(bgPath);
		}

		public override void Draw(DrawInfo info)
		{
			MonoDraw.DrawTextureDepth(info, mPanelBG, GetPosition(), GetDepth());
			base.Draw(info);
		}
	}
}
