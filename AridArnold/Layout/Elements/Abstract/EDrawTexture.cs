namespace AridArnold
{
	/// <summary>
	/// Element that just displays a texture
	/// </summary>
	abstract class EDrawTexture : LayElement
	{
		SpriteEffects mSpriteEffect;

		public EDrawTexture(XmlNode node, Layout parent) : base(node, parent)
		{
			string flipStr = MonoParse.GetString(node["flip"], "None");
			mSpriteEffect = MonoAlg.GetEnumFromString<SpriteEffects>(flipStr);
		}

		protected abstract Texture2D GetDrawTexture();

		protected virtual float GetRotation() { return 0.0f; }

		public override void Draw(DrawInfo info)
		{
			Texture2D drawTex = GetDrawTexture();
			float rotation = GetRotation();
			Vector2 rotOrigin = new Vector2(drawTex.Width / 2.0f, drawTex.Height / 2.0f) * GetScale();
			Vector2 rotDest = MonoMath.Rotate(rotOrigin, rotation);
			Vector2 drawPos = GetPosition();
			drawPos += rotOrigin - rotDest;
			MonoDraw.DrawTexture(info, drawTex, drawPos, null, GetColor(), GetRotation(), Vector2.Zero, GetScale(), mSpriteEffect, GetDepth());

			base.Draw(info);
		}
	}
}
