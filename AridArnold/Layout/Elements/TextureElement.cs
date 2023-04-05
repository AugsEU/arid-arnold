namespace AridArnold
{
	/// <summary>
	/// Displays a simple texture.
	/// </summary>
	internal class TextureElement : DrawTextureElement
	{
		Texture2D mTexture;

		public TextureElement(XmlNode node) : base(node)
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
