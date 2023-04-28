namespace AridArnold
{
	class CampaignMetaData
	{
		string mCampaignName;
		string mCampaignId;
		int mStartRoomID;

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
	}
}
