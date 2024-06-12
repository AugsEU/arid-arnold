
namespace AridArnold
{
	internal class EText : LayElement
	{
		enum TextAlign
		{
			Left,
			Centre
		}

		SpriteFont mFont;
		string mStringID;
		TextAlign mAlign;

		public EText(XmlNode rootNode) : base(rootNode)
		{
			string fontName = MonoParse.GetString(rootNode["font"], "Pixica-24");
			mFont = FontManager.I.GetFont(fontName);
			mStringID = MonoParse.GetString(rootNode["stringID"]);

			string alignStr = MonoParse.GetString(rootNode["align"]);
			mAlign = alignStr == "centre" ? TextAlign.Centre : TextAlign.Left;
		}

		public override void Draw(DrawInfo info)
		{
			string str = LanguageManager.I.GetText(mStringID);

			if(mAlign == TextAlign.Left)
			{
				MonoDraw.DrawString(info, mFont, str, GetPosition(), GetColor(), GetDepth());
			}
			else if(mAlign == TextAlign.Centre)
			{
				MonoDraw.DrawStringCentred(info, mFont, GetPosition(), GetColor(), str, GetDepth());
			}
		}
	}
}
