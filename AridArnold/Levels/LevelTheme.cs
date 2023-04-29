namespace AridArnold
{
	internal class LevelTheme
	{
		#region rMembers

		List<(string, string)> mRemappedTextures;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Load theme
		/// </summary>
		public LevelTheme(string xmlPath, string root)
		{
			xmlPath = "Content/" + xmlPath;
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(xmlPath);
			XmlNode rootNode = xmlDoc.LastChild;

			mRemappedTextures = new List<(string, string)>();

			XmlNodeList remapNodes = rootNode.SelectNodes("path");
			foreach (XmlNode remapNode in remapNodes)
			{
				string from = remapNode.Attributes["from"].Value;
				string to = "Tiles/" + root + "/" + remapNode.InnerText;

				mRemappedTextures.Add((from, to));
			}
		}



		/// <summary>
		/// Load the theme
		/// </summary>
		public void Load()
		{
			foreach ((string, string) path in mRemappedTextures)
			{
				MonoData.I.AddPathRemap(path.Item1, path.Item2);
			}
		}



		/// <summary>
		/// Unload theme.
		/// </summary>
		public void Unload()
		{
			foreach ((string, string) path in mRemappedTextures)
			{
				MonoData.I.RemovePathRemap(path.Item1);
			}
		}

		#endregion rIntialisation
	}
}
