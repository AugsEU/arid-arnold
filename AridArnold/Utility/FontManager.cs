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
		public void LoadAllFonts()
		{
			mFonts.Add("Pixica-12", MonoData.I.MonoGameLoad<SpriteFont>("Fonts/Pixica12"));
			mFonts.Add("Pixica-24", MonoData.I.MonoGameLoad<SpriteFont>("Fonts/Pixica24"));
			mFonts.Add("Pixica-36", MonoData.I.MonoGameLoad<SpriteFont>("Fonts/Pixica36"));

			mFonts.Add("Pixica-12-b", MonoData.I.MonoGameLoad<SpriteFont>("Fonts/Pixica12b"));
			mFonts.Add("Pixica-24-b", MonoData.I.MonoGameLoad<SpriteFont>("Fonts/Pixica24b"));
			mFonts.Add("Pixica-36-b", MonoData.I.MonoGameLoad<SpriteFont>("Fonts/Pixica36b"));

			mFonts.Add("PixicaMicro-12", MonoData.I.MonoGameLoad<SpriteFont>("Fonts/PixicaMicro12"));
			mFonts.Add("PixicaMicro-24", MonoData.I.MonoGameLoad<SpriteFont>("Fonts/PixicaMicro24"));
			mFonts.Add("PixicaMicro-36", MonoData.I.MonoGameLoad<SpriteFont>("Fonts/PixicaMicro36"));

			mFonts.Add("PixicaMicro-12-b", MonoData.I.MonoGameLoad<SpriteFont>("Fonts/PixicaMicro12b"));
			mFonts.Add("PixicaMicro-24-b", MonoData.I.MonoGameLoad<SpriteFont>("Fonts/PixicaMicro24b"));
			mFonts.Add("PixicaMicro-36-b", MonoData.I.MonoGameLoad<SpriteFont>("Fonts/PixicaMicro36b"));
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



		/// <summary>
		/// Get a font of a specific name.
		/// </summary>
		/// <param name="key">Font name</param>
		/// <returns>SpriteFont reference</returns>
		public SpriteFont GetFont(string fontType, int size, bool bold = false)
		{
			string key = string.Format("{0}-{1}{2}", fontType, size.ToString(), bold ? "-b" : "");
			return mFonts.GetValueOrDefault(key);
		}

		#endregion rUtility
	}
}
