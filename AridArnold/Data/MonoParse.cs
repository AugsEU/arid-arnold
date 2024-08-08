using System.Globalization;

namespace AridArnold
{
	/// <summary>
	/// Class for parsing things.
	/// </summary>
	static class MonoParse
	{
		/// <summary>
		/// Parse float from xml node. Default = Zero
		/// </summary>
		static public float GetFloat(XmlNode node, float defaultVal = 0.0f)
		{
			if (node is null)
			{
				return defaultVal;
			}

			return float.Parse(node.InnerText, CultureInfo.InvariantCulture.NumberFormat);
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

			return int.Parse(node.InnerText, CultureInfo.InvariantCulture.NumberFormat);
		}



		/// <summary>
		/// Get inner text of node
		/// </summary>
		static public string GetString(XmlNode node, string defaultVal = "")
		{
			if (node is null)
			{
				return defaultVal;
			}

			return node.InnerText;
		}



		/// <summary>
		/// Parse a string attribute
		/// </summary>
		static public string GetStringAttrib(XmlNode node, string attribID, string defaultVal = "")
		{
			if (node is null)
			{
				return defaultVal;
			}

			attribID = attribID.ToLower();
			XmlAttribute attribObj = node.Attributes[attribID];

			if(attribObj is null)
			{
				return defaultVal;
			}

			return attribObj.Value;
		}



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
		/// Parse point from xml node. Default = zero
		/// </summary>
		static public Point GetPoint(XmlNode node)
		{
			XmlNode xNode = node.SelectSingleNode("x");
			XmlNode yNode = node.SelectSingleNode("y");
			return new Point(GetInt(xNode), GetInt(yNode));
		}




		/// <summary>
		/// Parse point from xml node. Default = zero
		/// </summary>
		static public Rectangle GetRectangle(XmlNode node)
		{
			XmlNode xNode = node.SelectSingleNode("x");
			XmlNode yNode = node.SelectSingleNode("y");
			XmlNode wNode = node.SelectSingleNode("width");
			XmlNode hNode = node.SelectSingleNode("height");

			return new Rectangle(GetInt(xNode), GetInt(yNode), GetInt(wNode), GetInt(hNode));
		}



		/// <summary>
		/// Parse point from xml node. Default = zero
		/// </summary>
		static public Rect2f GetRect2f(XmlNode node)
		{
			XmlNode wNode = node.SelectSingleNode("width");
			XmlNode hNode = node.SelectSingleNode("height");

			return new Rect2f(GetVector(node), GetFloat(wNode), GetFloat(hNode));
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
		/// Parse texture from xml node. Default = Dummy
		/// </summary>
		static public Texture2D GetTexture(XmlNode node, Texture2D defaultValue)
		{
			if (node is null)
			{
				return defaultValue;
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
		/// Parse node as enum
		/// </summary>
		static public T GetEnum<T>(XmlNode node, T defaultValue = default(T))
		{
			if(node is null)
			{
				return defaultValue;
			}

			string enumStr = GetString(node);
			return MonoEnum.GetEnumFromString<T>(enumStr);
		}


		/// <summary>
		/// Parse fontID node into sprite font
		/// </summary>
		static public SpriteFont GetFont(XmlNode node, string defaultFontID)
		{
			string fontID = GetString(node, "");

			if (node is null || fontID.Length == 0)
			{
				return FontManager.I.GetFont(defaultFontID);
			}

			return FontManager.I.GetFont(fontID);
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



		/// <summary>
		/// Parse xml node into speechbox style
		/// </summary>
		static public SpeechBoxStyle GetSpeechBoxStyle(XmlNode node)
		{
			SpeechBoxStyle ret = SpeechBoxStyle.DefaultStyle;

			string fontKey = GetString(node["font"]);
			if (fontKey.Length > 0)
			{
				ret.mFont = FontManager.I.GetFont(fontKey);
			}

			ret.mWidth = GetFloat(node["width"], ret.mWidth);
			ret.mLeading = GetFloat(node["leading"], ret.mLeading);
			ret.mKerning = GetFloat(node["kerning"], ret.mKerning);
			ret.mScrollSpeed = GetFloat(node["scrollSpeed"], ret.mScrollSpeed);
			ret.mFramesPerLetter = GetInt(node["framesPerLetter"], ret.mFramesPerLetter);
			ret.mFillColor = GetColor(node["fillColor"], ret.mFillColor);
			ret.mBorderColor = GetColor(node["borderColor"], ret.mFillColor);
			ret.mFlipSpike = node["flipSpike"] is not null;
			return ret;
		}





		/// <summary>
		/// Parse xml node into speechbox style
		/// </summary>
		static public TextBoxStyle GetTextBoxStyle(XmlNode node)
		{
			TextBoxStyle ret = TextBoxStyle.DefaultStyle;

			string fontKey = GetString(node["font"]);
			if (fontKey.Length > 0)
			{
				ret.mFont = FontManager.I.GetFont(fontKey);
			}

			ret.mLeading = GetFloat(node["leading"], ret.mLeading);
			ret.mKerning = GetFloat(node["kerning"], ret.mKerning);
			return ret;
		}
	}
}
