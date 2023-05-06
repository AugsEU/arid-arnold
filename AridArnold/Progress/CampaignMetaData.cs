namespace AridArnold
{
	class CampaignMetaData
	{
		string mCampaignName;
		string mCampaignId;
		int mStartRoomID;
		Dictionary<string, byte> mCoinTypeIDs;

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
