namespace AridArnold
{
	/// <summary>
	/// Displays a simple texture.
	/// </summary>
	internal class ETexture : EDrawTexture
	{
		Texture2D mTexture;

		public ETexture(XmlNode node, Layout parent) : base(node, parent)
		{
			mTexture = MonoParse.GetTexture(node["texture"]);
		}

		protected override Texture2D GetDrawTexture()
		{
			return mTexture;
		}
	}
}
