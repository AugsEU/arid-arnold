namespace AridArnold
{
	/// <summary>
	/// Class for parsing things.
	/// </summary>
	static class MonoParse
	{
		static public Vector2 GetVector(XmlNode node)
		{
			XmlNode xNode = node.SelectSingleNode("x");
			XmlNode yNode = node.SelectSingleNode("y");
			return new Vector2(GetFloat(xNode), GetFloat(yNode));
		}



		static public float GetFloat(XmlNode node, float defaultVal = 0.0f)
		{
			if(node is null)
			{
				return defaultVal;
			}

			return float.Parse(node.InnerText);
		}



		static public int GetInt(XmlNode node, int defaultVal = 0)
		{
			if (node is null)
			{
				return defaultVal;
			}

			return int.Parse(node.InnerText);
		}



		static public DrawLayer GetDrawLayer(XmlNode node, DrawLayer defaultVal = DrawLayer.Default)
		{
			if(node is null)
			{
				return defaultVal;
			}

			return MonoDraw.GetDrawLayer(node.InnerText);
		}



		static public Texture2D GetTexture(XmlNode node)
		{
			if(node is null)
			{
				return Main.GetDummyTexture();
			}

			return MonoData.I.MonoGameLoad<Texture2D>(node.InnerText);
		}



		static public Color GetColor(XmlNode node)
		{
			if(node is null)
			{
				return Color.White;
			}

			return MonoColor.HEXToColor(node.InnerXml);
		}



		static public TimeZoneOverride GetTimeZoneOverride(XmlNode node)
		{
			XmlNode fromTimeNode = node.SelectSingleNode("from");
			XmlNode toTimeNode = node.SelectSingleNode("to");
			XmlNode levelIDNode = node.SelectSingleNode("level");
			XmlNode xNode = node.SelectSingleNode("x");
			XmlNode yNode = node.SelectSingleNode("y");

			TimeZoneOverride retInfo;

			retInfo.mTimeFrom			= GetInt(fromTimeNode);
			retInfo.mTimeTo				= GetInt(toTimeNode);
			retInfo.mDestinationLevel	= GetInt(levelIDNode);
			retInfo.mArnoldSpawnPoint = new Point(GetInt(xNode), GetInt(yNode));

			return retInfo;
		}
	}
}
