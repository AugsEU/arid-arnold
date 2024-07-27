namespace AridArnold
{
	/// <summary>
	/// Shows a block of text with linebreaks and all.
	/// </summary>
	class ETextBlock : LayElement
	{
		const double TEXT_DELAY_TIME = 60.0;

		TextBoxStyle mTextBoxStyle;
		float mWidth;
		SimpleTextBoxRenderer mTextRenderer;

		// We want a slight delay on showing the text to avoid eratic behaviour
		PercentageTimer mTextQueueTimer;
		string mQueuedTextID;

		string mLoadedID;

		public ETextBlock(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			string strID = MonoParse.GetString(rootNode["stringID"], "");
			mWidth = MonoParse.GetFloat(rootNode["width"]);
			mTextBoxStyle = MonoParse.GetTextBoxStyle(rootNode);

			mTextQueueTimer = new PercentageTimer(TEXT_DELAY_TIME);

			SetText(strID);

			mQueuedTextID = strID;
		}

		public void QueueText(string strID)
		{
			if (mQueuedTextID != strID)
			{
				mTextQueueTimer.ResetStart();
				mQueuedTextID = strID;
			}
		}

		public void SetText(string textID)
		{
			// Already showing this guy
			if (textID == mLoadedID)
			{
				return;
			}
			mLoadedID = textID;
			
			if (textID.Length == 0)
			{
				mTextRenderer = null;
				return;
			}

			mTextRenderer = new SimpleTextBoxRenderer(textID, GetPosition(), mWidth, mTextBoxStyle);
		}

		public override void Update(GameTime gameTime)
		{
			mTextQueueTimer.Update(gameTime);

			if(mTextQueueTimer.GetPercentageF() >= 1.0f)
			{
				SetText(mQueuedTextID);
			}

			if(mTextRenderer is null)
			{
				return;
			}

			mTextRenderer.Update(gameTime);
			base.Update(gameTime);
		}

		public override void Draw(DrawInfo info)
		{
			if (mTextRenderer is null)
			{
				return;
			}

			mTextRenderer.Draw(info);
		}
	}
}
