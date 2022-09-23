namespace AridArnold
{
    internal class FontManager : Singleton<FontManager>
    {
        Dictionary<string, SpriteFont> mFonts = new Dictionary<string, SpriteFont>();

        public void LoadAllFonts(ContentManager content)
        {
            mFonts.Add("Pixica-24", content.Load<SpriteFont>("Fonts/Pixica"));
            mFonts.Add("Pixica Micro-24", content.Load<SpriteFont>("Fonts/Pixica-Small"));
        }

        public SpriteFont GetFont(string key)
        {
            return mFonts.GetValueOrDefault(key);
        }
    }
}
