using System.Xml;

namespace AridArnold
{
	/// <summary>
	/// World theming, such as a background and tile texture swaps.
	/// </summary>
	internal class WorldTheme
	{
		#region rMembers

		string mBGName = "";
		List<(string, string)> mRemappedTextures;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Load theme
		/// </summary>
		public WorldTheme(XmlNode themeNode, string id)
		{
			mBGName = themeNode.SelectSingleNode("bg").InnerText;
			mRemappedTextures = new List<(string, string)>();

			XmlNodeList remapNodes = themeNode.SelectNodes("path");
			foreach(XmlNode remapNode in remapNodes)
			{
				string from = remapNode.Attributes["from"].Value;
				string to = "Tiles/" + id + "/" + remapNode.InnerText;

				mRemappedTextures.Add((from, to));
			}
		}



		/// <summary>
		/// Load the theme
		/// </summary>
		public void Load()
		{
			foreach((string, string) path in mRemappedTextures)
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



		/// <summary>
		/// Get the BG name
		/// </summary>
		public string GetBGName()
		{
			return mBGName;
		}

		#endregion rIntialisation
	}
}
