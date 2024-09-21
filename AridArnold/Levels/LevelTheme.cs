namespace AridArnold
{
	internal class LevelTheme
	{
		#region rMembers

		List<(string, string)> mRemappedTextures;
		Color mHubArrowColours;
		string mDisplayName;
		string mMusicID;

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

			mHubArrowColours = MonoParse.GetColor(rootNode["exitColour"], Color.Black);

			mDisplayName = MonoParse.GetString(rootNode["name"], "World");
			mMusicID = MonoParse.GetString(rootNode["music"], "");
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

			MusicManager.I.RequestTrackPlay(mMusicID);
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





		#region rGet

		/// <summary>
		/// Get colour for exit arrows
		/// </summary>
		public Color GetExitColor()
		{
			return mHubArrowColours;
		}



		/// <summary>
		/// Display name
		/// </summary>
		public string GetDisplayName()
		{
			return mDisplayName;
		}


		/// <summary>
		/// Get the theme's music
		/// </summary>
		public string GetMusicID()
		{
			return mMusicID;
		}

		#endregion rGet
	}
}
