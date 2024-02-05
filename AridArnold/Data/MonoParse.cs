namespace AridArnold
{
	/// <summary>
	/// Class for parsing things.
	/// </summary>
	static class MonoParse
	{
		/// <summary>
		/// Parse vector from xml node. Default = Zero
		/// </summary>
		static public Vector2 GetVector(XmlNode node)
		{
			XmlNode xNode = node.SelectSingleNode("x");
			XmlNode yNode = node.SelectSingleNode("y");
			return new Vector2(GetFloat(xNode), GetFloat(yNode));
		}



		/// <summary>
		/// Parse float from xml node. Default = Zero
		/// </summary>
		static public float GetFloat(XmlNode node, float defaultVal = 0.0f)
		{
			if (node is null)
			{
				return defaultVal;
			}

			return float.Parse(node.InnerText);
		}



		/// <summary>
		/// Parse int from xml node. Default = Zero
		/// </summary>
		static public int GetInt(XmlNode node, int defaultVal = 0)
		{
			if (node is null)
			{
				return defaultVal;
			}

			return int.Parse(node.InnerText);
		}



		/// <summary>
		/// Get inner text of node
		/// </summary>
		static public string GetString(XmlNode node, string defaultVal = "")
		{
			if(node is null)
			{
				return defaultVal;
			}

			return node.InnerText;
		}


		/// <summary>
		/// Parse draw layer from xml node. Default = Default Layer
		/// </summary>
		static public DrawLayer GetDrawLayer(XmlNode node, DrawLayer defaultVal = DrawLayer.Default)
		{
			if (node is null)
			{
				return defaultVal;
			}

			return MonoDraw.GetDrawLayer(node.InnerText);
		}



		/// <summary>
		/// Parse texture from xml node. Default = Dummy
		/// </summary>
		static public Texture2D GetTexture(XmlNode node)
		{
			if (node is null)
			{
				return Main.GetDummyTexture();
			}

			return MonoData.I.MonoGameLoad<Texture2D>(node.InnerText);
		}



		/// <summary>
		/// Parse hex colour from xml node. Default = Black
		/// </summary>
		static public Color GetColor(XmlNode node)
		{
			if (node is null)
			{
				return Color.White;
			}

			return MonoColor.HEXToColor(node.InnerXml);
		}



		/// <summary>
		/// Parse hex colour from xml node with specific default
		/// </summary>
		static public Color GetColor(XmlNode node, Color defaultVal)
		{
			if (node is null)
			{
				return defaultVal;
			}

			return MonoColor.HEXToColor(node.InnerXml);
		}



		/// <summary>
		/// Parse time override from xml node. No default.
		/// </summary>
		static public TimeZoneOverride GetTimeZoneOverride(XmlNode node)
		{
			XmlNode fromTimeNode = node.SelectSingleNode("from");
			XmlNode toTimeNode = node.SelectSingleNode("to");
			XmlNode levelIDNode = node.SelectSingleNode("level");
			XmlNode xNode = node.SelectSingleNode("x");
			XmlNode yNode = node.SelectSingleNode("y");

			TimeZoneOverride retInfo;

			retInfo.mTimeFrom = GetInt(fromTimeNode);
			retInfo.mTimeTo = GetInt(toTimeNode);
			retInfo.mDestinationLevel = GetInt(levelIDNode);
			retInfo.mArnoldSpawnPoint = new Point(GetInt(xNode), GetInt(yNode));

			return retInfo;
		}



		/// <summary>
		/// Parse camera spec struct from xml node.
		/// </summary>
		static public CameraSpec GetCameraSpec(XmlNode node)
		{
			CameraSpec returnValue = new CameraSpec();

			returnValue.mPosition = GetVector(node);
			returnValue.mZoom = GetFloat(node["zoom"], 1.0f);
			returnValue.mRotation = GetFloat(node["rot"]);

			return returnValue;
		}
	}
}
