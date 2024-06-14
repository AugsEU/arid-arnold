
namespace AridArnold
{
	abstract class UIPanelBase : LayElement
	{
		public static Color PANEL_WHITE = new Color(127, 127, 127);
		public static Color PANEL_GOLD = new Color(255, 182, 0);

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
