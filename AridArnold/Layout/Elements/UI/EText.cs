
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
		bool mShadow;

		public EText(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			string fontName = MonoParse.GetString(rootNode["font"], "Pixica-24");
			mFont = FontManager.I.GetFont(fontName);
			mStringID = MonoParse.GetString(rootNode["stringID"]);

			string alignStr = MonoParse.GetString(rootNode["align"]);
			mAlign = alignStr == "centre" ? TextAlign.Centre : TextAlign.Left;

			mShadow = rootNode["shadow"] is not null;
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
				if(mShadow)
				{
					MonoDraw.DrawStringCentredShadow(info, mFont, GetPosition(), GetColor(), str, 2.0f, GetDepth());
				}
				else
				{
					MonoDraw.DrawStringCentred(info, mFont, GetPosition(), GetColor(), str, GetDepth());
				}
			}
		}
	}
}
