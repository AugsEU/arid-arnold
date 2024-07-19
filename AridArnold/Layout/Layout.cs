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

		// We allow exactly 1 element to be selected per layout.
		// Perhaps a bit hacky but it works.
		NavElement mSelectedNavElement;

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





		#region rUtil

		/// <summary>
		/// Get element by it's ID
		/// </summary>
		public LayElement GetElementByID(string ID)
		{
			if(ID is null || ID.Length == 0)
			{
				return null;
			}

			// To do: We could make this O(1) but I don't care to at the moment.
			foreach (LayElement element in mElements)
			{
				if(element.GetID() == ID)
				{
					return element;
				}
			}

			return null;
		}



		/// <summary>
		/// Get the current navigation element selected
		/// </summary>
		public NavElement GetSelectedElement()
		{
			return mSelectedNavElement;
		}



		/// <summary>
		/// Select nav element
		/// </summary>
		public void SetSelectedElement(NavElement navElement)
		{
			mSelectedNavElement = navElement;
		}



		/// <summary>
		/// Select nav element by ID
		/// </summary>
		public void SetSelectedElement(string elemID)
		{
			// Note: unsafe cast because we want a crash if this is the wrong type.
			NavElement navElement = (NavElement)GetElementByID(elemID);

			mSelectedNavElement = navElement;
		}

		#endregion rUtil





		#region rFactory

		/// <summary>
		/// Element factory from node
		/// </summary>
		private LayElement GenerateElement(XmlNode node)
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

			return (LayElement)Activator.CreateInstance(elementType, node, this);
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
}
