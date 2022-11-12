namespace AridArnold
{
	/// <summary>
	/// Stores all fonts and loads them at the start of the game.
	/// </summary>
	internal class FontManager : Singleton<FontManager>
	{
		#region rMembers

		Dictionary<string, SpriteFont> mFonts = new Dictionary<string, SpriteFont>();

		#endregion rMembers





		#region rUtility

		/// <summary>
		/// Loads all fonts
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public void LoadAllFonts(ContentManager content)
		{
			mFonts.Add("Pixica-24", content.Load<SpriteFont>("Fonts/Pixica"));
			mFonts.Add("Pixica-12", content.Load<SpriteFont>("Fonts/Pixica-Medium"));
			mFonts.Add("Pixica Micro-24", content.Load<SpriteFont>("Fonts/Pixica-Small"));
		}



		/// <summary>
		/// Get a font of a specific name.
		/// </summary>
		/// <param name="key">Font name</param>
		/// <returns>SpriteFont reference</returns>
		public SpriteFont GetFont(string key)
		{
			return mFonts.GetValueOrDefault(key);
		}

		#endregion rUtility
	}
}
