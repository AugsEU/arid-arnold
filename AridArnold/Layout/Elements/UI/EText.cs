
namespace AridArnold
{
	internal class EText : LayElement
	{
		SpriteFont mFont;
		string mStringID;

		public EText(XmlNode rootNode) : base(rootNode)
		{
			string fontName = MonoParse.GetString(rootNode["font"], "Pixica-24");
			mFont = FontManager.I.GetFont(fontName);
			mStringID = MonoParse.GetString(rootNode["stringID"]);
		}

		public override void Draw(DrawInfo info)
		{
			string str = LanguageManager.I.GetText(mStringID);
			MonoDraw.DrawString(info, mFont, str, GetPosition(), GetColor(), GetDepth());
		}
	}
}
