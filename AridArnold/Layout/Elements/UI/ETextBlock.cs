namespace AridArnold
{
	/// <summary>
	/// Shows a block of text with linebreaks and all.
	/// </summary>
	class ETextBlock : LayElement
	{
		TextBoxStyle mTextBoxStyle;
		float mWidth;
		SimpleTextBoxRenderer mTextRenderer;

		public ETextBlock(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			string strID = MonoParse.GetString(rootNode["stringID"]);
			mWidth = MonoParse.GetFloat(rootNode["width"]);
			mTextBoxStyle = MonoParse.GetTextBoxStyle(rootNode);

			SetText(strID);
		}

		public void SetText(string textID)
		{
			mTextRenderer = new SimpleTextBoxRenderer(textID, GetPosition(), mWidth, mTextBoxStyle);
		}

		public override void Update(GameTime gameTime)
		{
			mTextRenderer.Update(gameTime);
			base.Update(gameTime);
		}

		public override void Draw(DrawInfo info)
		{
			mTextRenderer.Draw(info);
		}
	}
}
