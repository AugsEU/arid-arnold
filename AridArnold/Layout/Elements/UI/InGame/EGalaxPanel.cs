using System.Xml.Linq;

namespace AridArnold
{
	class EGalaxPanel : UIPanelBase
	{
		public EGalaxPanel(XmlNode rootNode, Layout parent) : base(rootNode, "UI/InGame/GalaxBG", parent)
		{

		}

		public override void Draw(DrawInfo info)
		{
			base.Draw(info);
		}
	}
}
