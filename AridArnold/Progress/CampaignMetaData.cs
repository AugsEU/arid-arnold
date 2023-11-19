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
		string mCampaignName;
		string mCampaignId;
		int mStartRoomID;
		Dictionary<string, byte> mCoinTypeIDs;
		List<TimeZoneOverride> mTimeOverrides;

		public CampaignMetaData(string xmlPath)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(xmlPath);
			XmlNode rootNode = xmlDoc.LastChild;

			// Load campaign ID
			XmlAttribute idAttr = rootNode.Attributes["id"];
			mCampaignId = idAttr.Value;

			// Load campaign name
			XmlNode nameNode = rootNode.SelectSingleNode("name");
			mCampaignName = nameNode.InnerText;

			// Load start room ID
			XmlNode startNode = rootNode.SelectSingleNode("start");
			mStartRoomID = int.Parse(startNode.InnerText);

			// Load coin IDs
			mCoinTypeIDs = new Dictionary<string, byte>();
			XmlNode coinsNode = rootNode.SelectSingleNode("coins");
			XmlNodeList coinTypeNodes = coinsNode.ChildNodes;
			foreach (XmlNode coinTypeNode in coinTypeNodes)
			{
				string worldRoot = coinTypeNode.Attributes["root"].Value;
				byte coindTypeID = byte.Parse(coinTypeNode.InnerText);
				mCoinTypeIDs.Add(worldRoot, coindTypeID);
			}

			// Load time overrides
			mTimeOverrides = new List<TimeZoneOverride>();
			XmlNode timeNode = rootNode.SelectSingleNode("timeOverrides");
			XmlNodeList timeOverrideNodes = timeNode.ChildNodes;
			foreach (XmlNode timeOverrideNode in timeOverrideNodes)
			{
				mTimeOverrides.Add(MonoParse.GetTimeZoneOverride(timeOverrideNode));
			}

			// Validate time overrides
			for(int i = 0; i < mTimeOverrides.Count; i++)
			{
				for(int j = i + 1; j < mTimeOverrides.Count; j++)
				{
					if (mTimeOverrides[i].mTimeTo == mTimeOverrides[j].mTimeTo &&
						mTimeOverrides[i].mTimeFrom == mTimeOverrides[j].mTimeFrom)
					{
						throw new Exception("Invalid timezone override data. Possible duplicates or conflicting destinations.");
					}
				}
			}
		}

		public string GetCampaignName()
		{
			return mCampaignName;
		}

		public string GetCampaignId()
		{
			return mCampaignId;
		}

		public int GetStartRoomID()
		{
			return mStartRoomID;
		}

		public byte GetCoinTypeID(string worldRoot)
		{
			return mCoinTypeIDs[worldRoot];
		}
	}
}
