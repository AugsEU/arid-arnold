namespace AridArnold
{
	internal class AnimatorElement : DrawTextureElement
	{
		Animator mAnim;

		public AnimatorElement(XmlNode node) : base(node)
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
