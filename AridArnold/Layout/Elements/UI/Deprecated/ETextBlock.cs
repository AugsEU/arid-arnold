
namespace AridArnold
{
	/// <summary>
	/// Shows a block of text with linebreaks and all.
	/// </summary>
	class ETextBlock : LayElement
	{
		SimpleTextBoxRenderer mTextRenderer;

		public ETextBlock(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			string strID = MonoParse.GetString(rootNode["stringID"]);
			float width = MonoParse.GetFloat(rootNode["width"]);
			TextBoxStyle style = MonoParse.GetTextBoxStyle(rootNode);

			mTextRenderer = new SimpleTextBoxRenderer(strID, GetPosition(), width, style);
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
