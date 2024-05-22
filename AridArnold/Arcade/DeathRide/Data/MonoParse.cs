namespace GMTK2023
{
	/// <summary>
	/// Class for parsing things.
	/// None of these are safe, so just don't pass in invalid data 5head
	/// </summary>
	static class MonoParse
	{
		static public Vector2 GetVector(XmlNode node)
		{
			XmlNode xNode = node.SelectSingleNode("x");
			XmlNode yNode = node.SelectSingleNode("y");
			return new Vector2(GetFloat(xNode), GetFloat(yNode));
		}

		static public float GetFloat(XmlNode node)
		{
			return float.Parse(node.InnerText);
		}
	}
}
