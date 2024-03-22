using System.Globalization;

namespace AridArnold
{
	struct TimeZoneOverride
	{
		public int mTimeFrom;
		public int mTimeTo;
		public int mDestinationLevel;
		public Point mArnoldSpawnPoint;
	}

	class CampaignMetaData
	{
		#region rMembers

		string mCampaignName;
		string mCampaignId;
		int mStartRoomID;
		Dictionary<string, byte> mCoinTypeIDs;
		List<TimeZoneOverride> mTimeOverrides;
		List<CinematicTrigger> mCinematicTriggers;

		#endregion rMembers


		#region rInit

		/// <summary>
		/// Load meta data from xml path
		/// </summary>
		public CampaignMetaData(string campaignRoot)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(campaignRoot + "Meta.xml");
			XmlNode rootNode = xmlDoc.LastChild;

			LoadBasicData(rootNode);
			LoadCoinData(rootNode);
			LoadTimeOverrides(rootNode);
			LoadCinematicTriggers(campaignRoot, rootNode);
		}



		/// <summary>
		/// Load basic data about the campaign
		/// </summary>
		void LoadBasicData(XmlNode rootNode)
		{
			// Load campaign ID
			XmlAttribute idAttr = rootNode.Attributes["id"];
			mCampaignId = idAttr.Value;

			// Load campaign name
			XmlNode nameNode = rootNode.SelectSingleNode("name");
			mCampaignName = nameNode.InnerText;

			// Load start room ID
			XmlNode startNode = rootNode.SelectSingleNode("start");
			mStartRoomID = int.Parse(startNode.InnerText, CultureInfo.InvariantCulture.NumberFormat);
		}



		/// <summary>
		/// Load coin data
		/// </summary>
		void LoadCoinData(XmlNode rootNode)
		{
			// Load coin IDs
			mCoinTypeIDs = new Dictionary<string, byte>();
			XmlNode coinsNode = rootNode.SelectSingleNode("coins");
			XmlNodeList coinTypeNodes = coinsNode.ChildNodes;
			foreach (XmlNode coinTypeNode in coinTypeNodes)
			{
				string worldRoot = coinTypeNode.Attributes["root"].Value;
				byte coindTypeID = byte.Parse(coinTypeNode.InnerText, CultureInfo.InvariantCulture.NumberFormat);
				mCoinTypeIDs.Add(worldRoot, coindTypeID);
			}
		}



		/// <summary>
		/// Load time override data
		/// </summary>
		void LoadTimeOverrides(XmlNode rootNode)
		{
			// Load time overrides
			mTimeOverrides = new List<TimeZoneOverride>();
			XmlNode timeNode = rootNode.SelectSingleNode("timeOverrides");
			XmlNodeList timeOverrideNodes = timeNode.ChildNodes;
			foreach (XmlNode timeOverrideNode in timeOverrideNodes)
			{
				mTimeOverrides.Add(MonoParse.GetTimeZoneOverride(timeOverrideNode));
			}


			// Validate time overrides
			for (int i = 0; i < mTimeOverrides.Count; i++)
			{
				for (int j = i + 1; j < mTimeOverrides.Count; j++)
				{
					if (mTimeOverrides[i].mTimeTo == mTimeOverrides[j].mTimeTo &&
						mTimeOverrides[i].mTimeFrom == mTimeOverrides[j].mTimeFrom)
					{
						throw new Exception("Invalid timezone override data. Possible duplicates or conflicting destinations.");
					}
				}
			}
		}



		/// <summary>
		/// Load cinematic triggers
		/// </summary>
		void LoadCinematicTriggers(string campaignRoot, XmlNode rootNode)
		{
			mCinematicTriggers = new List<CinematicTrigger>();
			XmlNode cinematicNode = rootNode.SelectSingleNode("cinematics");
			XmlNodeList cineTriggerNodes = cinematicNode.ChildNodes;

			foreach (XmlNode triggerNode in cineTriggerNodes)
			{
				mCinematicTriggers.Add(new CinematicTrigger(campaignRoot, triggerNode));
			}
		}

		#endregion rInit





		#region rGet

		/// <summary>
		/// Get display name
		/// </summary>
		public string GetCampaignName()
		{
			return mCampaignName;
		}



		/// <summary>
		/// Get internal name
		/// </summary>
		public string GetCampaignId()
		{
			return mCampaignId;
		}



		/// <summary>
		/// Get start room level ID
		/// </summary>
		public int GetStartRoomID()
		{
			return mStartRoomID;
		}



		/// <summary>
		/// Get coin id
		/// </summary>
		public byte GetCoinTypeID(string worldRoot)
		{
			return mCoinTypeIDs[worldRoot];
		}



		/// <summary>
		/// Get time override if it exists
		/// </summary>
		public TimeZoneOverride? GetTimeOverride(int fromTime, int toTime)
		{
			foreach (TimeZoneOverride timeZoneOverride in mTimeOverrides)
			{
				if (timeZoneOverride.mTimeTo == toTime && timeZoneOverride.mTimeFrom == fromTime)
				{
					return timeZoneOverride;
				}
			}

			return null;
		}



		/// <summary>
		/// Get list of cinematic triggers
		/// </summary>
		public List<CinematicTrigger> GetCinematicTriggers()
		{
			return mCinematicTriggers;
		}

		#endregion rGet
	}
}
