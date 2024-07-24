
namespace AridArnold
{
	class ESaveFileList : MenuScrollList
	{
		const float SAVE_FILE_ENTRY_HEIGHT = 80.0f;

		public ESaveFileList(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			string fontId = MonoParse.GetString(rootNode["font"], "Pixica-36");
			List<ProfileSaveInfo> saveGames = SaveManager.I.GetSaveFileList();
			for(int i = 0; i < saveGames.Count; i++)
			{
				string idStr = GenerateChildId(i, saveGames.Count);
				ProfileSaveInfo info = saveGames[i];
				SaveGameEntry newEntry = new SaveGameEntry(idStr, Vector2.Zero, new Vector2(mSize.X, SAVE_FILE_ENTRY_HEIGHT), fontId, parent, info);
				AddChild(newEntry);
			}

			LinkAllElements();
		}
	}
}
