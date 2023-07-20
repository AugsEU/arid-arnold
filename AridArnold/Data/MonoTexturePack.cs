using static AridArnold.Animator;

namespace AridArnold
{
	class MonoTexturePack
	{
		Dictionary<string, Texture2D> mTextures;

		public MonoTexturePack()
		{
			mTextures = new Dictionary<string, Texture2D>();
		}

		public MonoTexturePack(string path)
		{
			mTextures = new Dictionary<string, Texture2D>();
			LoadTexturePack(path);
		}

		public Texture2D GetTexture(string codeName)
		{
			// NOT SAFE but better to crash so we catch issues.
			return mTextures[codeName.ToLower()];
		}

		public void LoadTexturePack(string path)
		{
			path = "Content/" + path;
			string extention = Path.GetExtension(path);
			MonoDebug.Assert(extention == ".mtp");

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(path);
			XmlNode rootNode = xmlDoc.LastChild;

			XmlNodeList textureNodes = rootNode.ChildNodes;

			foreach (XmlNode textureNode in textureNodes)
			{
				string codeName = textureNode.Attributes["code"].Value.ToLower();
				string texturePath = textureNode.InnerText;
				mTextures.Add(codeName, MonoData.I.MonoGameLoad<Texture2D>(texturePath));
			}
		}
	}
}
