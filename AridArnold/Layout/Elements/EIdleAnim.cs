namespace AridArnold
{
	internal class EIdleAnim : EDrawTexture
	{
		IdleAnimator mIdleAnim;

		public EIdleAnim(XmlNode node) : base(node)
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
