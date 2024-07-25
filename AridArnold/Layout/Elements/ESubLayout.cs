namespace AridArnold
{
	/// <summary>
	/// Include an entire layout as a layout element.
	/// </summary>
	internal class ESubLayout : LayElement
	{
		Layout mSubLayout;

		public ESubLayout(XmlNode node, Layout parent) : base(node, parent)
		{
			string layoutPath = MonoParse.GetString(node["path"]);
			mSubLayout = new Layout(layoutPath);

			// Move all the elements by a delta
			Vector2 delta = GetPosition();

			List<LayElement> layElements = mSubLayout.GetElementList();
			foreach(LayElement layElement in layElements)
			{
				Vector2 elementPos = layElement.GetPosition();
				elementPos += delta;

				layElement.SetPos(elementPos);
			}
		}

		public override void Draw(DrawInfo info)
		{
			mSubLayout.Draw(info);
			base.Draw(info);
		}

		public override void Update(GameTime gameTime)
		{
			mSubLayout.Update(gameTime);
			base.Update(gameTime);
		}
	}
}
