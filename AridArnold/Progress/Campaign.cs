using System;
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
		int mCurrentWorld;

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

			mCurrentWorld = -1;
		}

		#endregion rInitialisation





		#region rAccess

		/// <summary>
		/// Get world at index.
		/// </summary>
		public World GetCurrentWorld()
		{
			return mWorlds[mCurrentWorld];
		}



		/// <summary>
		/// Set the world.
		/// </summary>
		public void SetCurrentWorldIdx(int idx)
		{
			if(mCurrentWorld != idx)
			{
				if(mCurrentWorld != -1)
				{
					// Unload previous world
					mWorlds[mCurrentWorld].UnloadWorld();
				}

				mWorlds[idx].LoadWorld();
			}

			mCurrentWorld = idx;
		}



		/// <summary>
		/// How many worlds are there in the campaign?
		/// </summary>
		public int GetNumberOfWorlds()
		{
			return mWorlds.Count;
		}



		/// <summary>
		/// Get total number of levels.
		/// </summary>
		/// <returns></returns>
		public int GetTotalNumberOfLevels()
		{
			int total = 0;
			foreach (World world in mWorlds)
			{
				total += world.GetNumberOfLevels();
			}
			return total;
		}


		/// <summary>
		/// Get total number of levels before level point.
		/// </summary>
		/// <returns></returns>
		public int GetLevelNumber(LevelPoint point)
		{
			int total = 0;
			for(int i = 0; i < point.mWorldIndex; i++)
			{
				total += mWorlds[i].GetNumberOfLevels();
			}
			return total + 1;
		}

		#endregion rAccess
	}
}
