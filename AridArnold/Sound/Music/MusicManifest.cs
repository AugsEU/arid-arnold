﻿namespace AridArnold
{
	struct MusicManifestEntry
	{

		const float DEFAULT_FADE_IN = 700.0f;

		public string mFileName;
		public float mVolume;
		public bool mNoLoop;
		public float mFadeIn;

		public MusicManifestEntry(XmlNode node, string basePath)
		{
			mFileName = MonoParse.GetString(node["file"], "");
			mFileName = Path.Join(basePath, mFileName);

			mVolume = MonoParse.GetFloat(node["vol"], 1.0f);
			mFadeIn = MonoParse.GetFloat(node["fadeIn"], DEFAULT_FADE_IN);

			mNoLoop = node["noLoop"] is not null; 
		}
	}


	class MusicManifest
	{
		Dictionary<string, MusicManifestEntry> mData;

		public MusicManifest(string path)
		{
			mData = new Dictionary<string, MusicManifestEntry>();

			path = "Content/" + path;
			string extention = Path.GetExtension(path);
			string directoryPath = Path.GetDirectoryName(path);
			MonoDebug.Assert(extention == ".xml");

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(path);
			XmlNode rootNode = xmlDoc.LastChild;

			XmlNodeList musicNodes = rootNode.SelectNodes("music");

			foreach(XmlNode node in musicNodes )
			{
				string id = node.Attributes["id"].InnerText.ToLower();
				mData[id] = new MusicManifestEntry(node, directoryPath);
			}
		}

		public MusicTrack LoadTrack(string id)
		{
			if(id is null || id.Length == 0)
			{
				return null;
			}

			id = id.ToLower();

			MusicManifestEntry entry = mData[id];

			return new MusicTrack(id, entry);
		}
	}
}
