using System.Xml;

namespace AridArnold
{
	internal class World
	{
		#region rMembers

		string mName;
		string mID;
		WorldTheme mTheme;
		List<Level> mLevels;


		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Load world from XML node
		/// </summary>
		public World(XmlNode worldNode)
		{
			// Load basic attributes
			mID = worldNode.Attributes["id"].Value;
			mName = worldNode.SelectSingleNode("name").InnerText;

			// Load theme
			XmlNode themeNode = worldNode.SelectSingleNode("theme");

			mTheme = new WorldTheme(themeNode, mID);

			// Load levels
			mLevels = new List<Level>();

			XmlNode levelListNode = worldNode.SelectSingleNode("levels");

			foreach(XmlNode levelNode in levelListNode)
			{
				LoadLevel(levelNode);
			}
		}



		/// <summary>
		/// Load level from XML node.
		/// </summary>
		void LoadLevel(XmlNode levelNode)
		{
			string levelID = levelNode.Attributes["id"].Value;
			string levelType = levelNode.SelectSingleNode("type").InnerText;

			Level levelToAdd = null;
			switch (levelType)
			{
				case "collectWater":
					int numWater = Convert.ToInt32(levelNode.SelectSingleNode("waterNeeded").InnerText);
					levelToAdd = new CollectWaterLevel(levelID, numWater);
					break;
				case "collectFlag":
					levelToAdd = new CollectFlagLevel(levelID);
					break;
				default:
					throw new NotImplementedException();
			}

			XmlNode bgLayoutNode = levelNode.SelectSingleNode("bg");

			if(bgLayoutNode is not null)
			{
				levelToAdd.SetBGLayoutPath(bgLayoutNode.InnerText);
			}

			mLevels.Add(levelToAdd);
		}



		/// <summary>
		/// Load into the world.
		/// Including manifests
		/// </summary>
		public void LoadWorld()
		{
			mTheme.Load();
		}



		/// <summary>
		/// Unload the world.
		/// Including manifests
		/// </summary>
		public void UnloadWorld()
		{
			mTheme.Unload();
		}

		#endregion rInitialisation





		#region rAccess

		/// <summary>
		/// Get name of world(for display)
		/// </summary>
		public string GetName()
		{
			return mName;
		}



		/// <summary>
		/// Get Unique ID of world.
		/// </summary>
		public string GetID()
		{
			return mID;
		}



		/// <summary>
		/// Get a level.
		/// </summary>
		public Level GetLevel(int idx)
		{
			return mLevels[idx];
		}



		/// <summary>
		/// How many levels in this world?
		/// </summary>
		public int GetNumberOfLevels()
		{
			return mLevels.Count;
		}



		/// <summary>
		/// Get themeing for this world.
		/// </summary>
		public WorldTheme GetTheme()
		{
			return mTheme;
		}

		#endregion rAccess
	}
}
