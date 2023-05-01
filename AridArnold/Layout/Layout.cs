using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace AridArnold
{
	/// <summary>
	/// Represents a layout of elements
	/// </summary>
	internal class Layout
	{
		static Dictionary<string, Type> sElementNameMapping = new Dictionary<string, Type>();

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
			if(sElementNameMapping.Count == 0)
			{
				GenerateClassNameMap();
			}

			string nodeType = "e" + node.Attributes["type"].Value.ToLower();

			if (!sElementNameMapping.TryGetValue(nodeType, out Type elementType))
			{
				throw new Exception("Do not recognise UI element: " + nodeType);
			}

			return (LayElement)Activator.CreateInstance(elementType, node);
		}

		static void GenerateClassNameMap()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			Type elementType = typeof(LayElement);
			IEnumerable <Type> types = assembly.GetTypes().Where(t => elementType.IsAssignableFrom(t) && !t.IsAbstract);

			foreach (Type type in types)
			{
				string typeName = type.Name;
				sElementNameMapping[typeName.ToLower()] = type;
			}
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
