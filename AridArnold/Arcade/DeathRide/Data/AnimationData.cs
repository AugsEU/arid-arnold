namespace GMTK2023
{
	/// <summary>
	/// Utility class for loading animations from data
	/// </summary>
	class AnimationData
	{
		Animator.PlayType mPlayType;
		(string, float)[] mTextures;

		public AnimationData()
		{
			mPlayType = Animator.PlayType.Repeat;
			mTextures = null;
		}

		public AnimationData(string filePath)
		{
			LoadFromFile(filePath);
		}

		private void LoadFromFile(string filePath)
		{
			string extention = Path.GetExtension(filePath);
			switch (extention)
			{
				case "":
				{
					// Load as single texture
					mTextures = new (string, float)[] { (filePath, 1.0f) };
					mPlayType = Animator.PlayType.OneShot;
					break;
				}
				case ".max": // Mono Animation XML
				{
					LoadFromXML("Content/" + filePath);
					break;
				}
				default:
					throw new NotImplementedException();
			}
		}

		private void LoadFromXML(string XMLPath)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(XMLPath);
			XmlNode rootNode = xmlDoc.LastChild;

			string type = rootNode.Attributes["type"].Value.ToLower();

			mPlayType = type == "repeat" ? Animator.PlayType.Repeat : Animator.PlayType.OneShot;

			XmlNodeList textureNodes = rootNode.ChildNodes;

			mTextures = new (string, float)[textureNodes.Count];

			int idx = 0;
			foreach (XmlNode textureNode in textureNodes)
			{
				XmlAttribute timeAttrib = textureNode.Attributes["time"];
				float time = timeAttrib is not null ? float.Parse(timeAttrib.Value) : 1.0f;
				mTextures[idx++] = (textureNode.InnerText, time);
			}
		}

		public Animator GenerateAnimator()
		{
			return new Animator(mPlayType, mTextures);
		}
	}
}
