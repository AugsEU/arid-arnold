namespace AridArnold
{
	/// <summary>
	/// Displays a simple texture.
	/// </summary>
	internal class ETexture : EDrawTexture
	{
		Texture2D mTexture;

		public ETexture(XmlNode node) : base(node)
		{
			mTexture = MonoParse.GetTexture(node["texture"]);
		}

		protected override Texture2D GetDrawTexture()
		{
			return mTexture;
		}
	}
}
