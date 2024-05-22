namespace GMTK2023
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
		public void LoadAllFonts()
		{
			mFonts.Add("Scream-36", MonoData.I.MonoGameLoad<SpriteFont>("Fonts/ScreamBig"));
			mFonts.Add("Scream-24", MonoData.I.MonoGameLoad<SpriteFont>("Fonts/Scream"));
			mFonts.Add("Pixica-24", MonoData.I.MonoGameLoad<SpriteFont>("Fonts/Pixica"));
			mFonts.Add("Pixica-12", MonoData.I.MonoGameLoad<SpriteFont>("Fonts/PixicaMedium"));
			mFonts.Add("Pixica Micro-24", MonoData.I.MonoGameLoad<SpriteFont>("Fonts/PixicaSmall"));
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
