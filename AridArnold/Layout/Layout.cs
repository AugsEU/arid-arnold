namespace AridArnold
{
	/// <summary>
	/// Represents a layout of elements
	/// </summary>
	internal class Layout
	{
		List<LayElement> mElements;

		public Layout(string layoutFile)
		{
			layoutFile = "Content/" + layoutFile;

			mElements = new List<LayElement>();

			MonoDebug.Assert(Path.GetExtension(layoutFile) == ".mlo"); // Mono Layout

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(layoutFile);
			XmlNode rootNode = xmlDoc.LastChild;

			LoadSubElements(rootNode);
		}

		private void LoadSubElements(XmlNode rootNode)
		{
			foreach (XmlNode node in rootNode.ChildNodes)
			{
				if (node.Name == "element")
				{
					mElements.Add(GenerateElement(node));
				}
				else
				{
					// Probably do something smart with this later, for now just recursively search for elements
					LoadSubElements(node);
				}
			}
		}

		public void Update(GameTime gameTime)
		{
			foreach (LayElement element in mElements)
			{
				element.Update(gameTime);
			}
		}

		public void Draw(DrawInfo info)
		{
			foreach (LayElement element in mElements)
			{
				element.Draw(info);
			}
		}

		// Factory
		private static LayElement GenerateElement(XmlNode node)
		{
			string nodeType = node.Attributes["type"].Value.ToLower();

			// Maybe use reflection instead?
			switch (nodeType)
			{
				case "idleanim":
					return new IdleAnimElement(node);
				case "texture":
					return new TextureElement(node);
				case "animator":
					return new AnimatorElement(node);
				case "mirrorbg":
					return new MirrorBG(node);
			}

			throw new Exception("Do not recognise layout element: " + nodeType);
		}
	}

	/// <summary>
	/// Element in our layout
	/// </summary>
	abstract class LayElement
	{
		protected Vector2 mPos;
		protected DrawLayer mDepth;

		public LayElement(XmlNode rootNode)
		{
			mPos = MonoParse.GetVector(rootNode);
			mDepth = MonoDraw.GetDrawLayer(rootNode["depth"].InnerText);
		}

		public virtual void Update(GameTime gameTime) { }

		public virtual void Draw(DrawInfo info) { }

		public DrawLayer GetDepth() { return mDepth; }
	}
}
