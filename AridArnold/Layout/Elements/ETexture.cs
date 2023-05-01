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
			string texturePath = node["texture"].InnerText;
			mTexture = MonoData.I.MonoGameLoad<Texture2D>(texturePath);
		}

		protected override Texture2D GetDrawTexture()
		{
			return mTexture;
		}
	}
}
