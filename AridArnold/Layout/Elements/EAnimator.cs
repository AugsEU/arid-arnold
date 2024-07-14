namespace AridArnold
{
	internal class EAnimator : EDrawTexture
	{
		Animator mAnim;

		public EAnimator(XmlNode node, Layout parent) : base(node, parent)
		{
			string animPath = node["anim"].InnerText;
			mAnim = MonoData.I.LoadAnimator(animPath);
			mAnim.Play();
		}

		public override void Update(GameTime gameTime)
		{
			mAnim.Update(gameTime);

			base.Update(gameTime);
		}

		protected override Texture2D GetDrawTexture()
		{
			return mAnim.GetCurrentTexture();
		}
	}
}
