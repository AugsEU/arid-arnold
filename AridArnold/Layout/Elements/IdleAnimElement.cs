namespace AridArnold
{
	internal class IdleAnimElement : DrawTextureElement
	{
		IdleAnimator mIdleAnim;

		public IdleAnimElement(XmlNode node) : base(node)
		{
			string idleAnimPath = node["anim"].InnerText;
			mIdleAnim = MonoData.I.LoadIdleAnimator(idleAnimPath);
		}

		public override void Update(GameTime gameTime)
		{
			mIdleAnim.Update(gameTime);

			base.Update(gameTime);
		}

		protected override Texture2D GetDrawTexture()
		{
			return mIdleAnim.GetCurrentTexture();
		}
	}
}
