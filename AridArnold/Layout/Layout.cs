using System.Linq;
using System.Reflection;

namespace AridArnold
{
	/// <summary>
	/// Represents a layout of elements
	/// </summary>
	internal class Layout
	{
		#region rStatic

		static Dictionary<string, Type> sElementNameMapping = new Dictionary<string, Type>();

		#endregion rStatic





		#region rMembers

		List<LayElement> mElements;

		#endregion rMembers


		#region rInit

		/// <summary>
		/// Create a layout from xml file path
		/// </summary>
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

		/// <summary>
		/// Load a tag
		/// </summary>
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

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update all elements in layout
		/// </summary>
		public void Update(GameTime gameTime)
		{
			foreach (LayElement element in mElements)
			{
				element.Update(gameTime);
			}
		}

		#endregion rUpdate



		#region rDraw

		/// <summary>
		/// Draw all elements in layout
		/// </summary>
		public void Draw(DrawInfo info)
		{
			foreach (LayElement element in mElements)
			{
				if (element.IsVisible())
				{
					element.Draw(info);
				}
			}
		}

		#endregion rDraw


		#region rFactory

		/// <summary>
		/// Element factory from node
		/// </summary>
		private static LayElement GenerateElement(XmlNode node)
		{
			if (sElementNameMapping.Count == 0)
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

		/// <summary>
		/// Called once per program, generate string -> type mapping using reflection
		/// Note: Not threadsafe.
		/// </summary>
		static void GenerateClassNameMap()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			Type elementType = typeof(LayElement);
			IEnumerable<Type> types = assembly.GetTypes().Where(t => elementType.IsAssignableFrom(t) && !t.IsAbstract);

			foreach (Type type in types)
			{
				string typeName = type.Name;
				sElementNameMapping[typeName.ToLower()] = type;
			}
		}

		#endregion rFactory
	}

	/// <summary>
	/// Element in our layout
	/// </summary>
	abstract class LayElement
	{
		Vector2 mPos;
		DrawLayer mDepth;
		Color mColor;
		float mScale;
		bool mVisible;

		public LayElement(XmlNode rootNode)
		{
			mPos = MonoParse.GetVector(rootNode);
			mDepth = MonoParse.GetDrawLayer(rootNode["depth"]);
			mScale = MonoParse.GetFloat(rootNode["scale"], 1.0f);
			mColor = MonoParse.GetColor(rootNode["color"]);
			mVisible = true;
		}

		public virtual void Update(GameTime gameTime) { }

		public virtual void Draw(DrawInfo info) { }

		public DrawLayer GetDepth() { return mDepth; }

		public float GetScale() { return mScale; }

		public Vector2 GetPosition() { return mPos; }

		public Color GetColor() { return mColor; }

		public bool IsVisible() { return mVisible; }

		public void SetVisible(bool visible) { mVisible = visible; }
	}
}
