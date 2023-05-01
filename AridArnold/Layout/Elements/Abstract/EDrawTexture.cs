namespace AridArnold
{
	/// <summary>
	/// Element that just displays a texture
	/// </summary>
	abstract class EDrawTexture : LayElement
	{
		public EDrawTexture(XmlNode node) : base(node)
		{
		}

		protected abstract Texture2D GetDrawTexture();

		public override void Draw(DrawInfo info)
		{
			MonoDraw.DrawTextureDepth(info, GetDrawTexture(), mPos, mDepth);

			base.Draw(info);
		}
	}
}
