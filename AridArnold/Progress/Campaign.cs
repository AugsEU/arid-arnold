using System.Xml;

namespace AridArnold
{
	// Campaigns are comprised of many worlds
	// Worlds are comprised of many levels

	/// <summary>
	/// Represents a Campaign of Worlds
	/// </summary>
	class Campaign
	{
		#region rMembers

		string mID;
		string mName;
		List<World> mWorlds;

		#endregion rMembers





		#region rInitialisation

		public Campaign(string xmlPath)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(xmlPath);
			XmlNode rootNode = xmlDoc.LastChild;

			mID = rootNode.Attributes["id"].Value;
			mName = rootNode.SelectSingleNode("name").InnerText;

			// Load world data
			mWorlds = new List<World>();

			XmlNode worldListNode = rootNode.SelectSingleNode("worlds");

			foreach(XmlNode worldNode in worldListNode.ChildNodes)
			{
				mWorlds.Add(new World(worldNode));
			}
		}

		#endregion rInitialisation





		#region rAccess

		/// <summary>
		/// Get world at index.
		/// </summary>
		public World GetWorld(int idx)
		{
			return mWorlds[idx];
		}



		/// <summary>
		/// How many worlds are there in the campaign?
		/// </summary>
		public int GetNumberOfWorlds()
		{
			return mWorlds.Count;
		}

		#endregion rAccess
	}
}
