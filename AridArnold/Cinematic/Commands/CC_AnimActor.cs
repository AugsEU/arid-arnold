

namespace AridArnold
{
	internal class CC_AnimActor : CC_ActorCommand
	{
		Animator mAnimation;

		public CC_AnimActor(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			mAnimation = MonoData.I.LoadAnimator(cmdNode["anim"].InnerText);
			mAnimation.Play();
		}


		public override void Update(GameTime gameTime, int currentFrame)
		{
			mAnimation.Update(gameTime);
			mTargetActor.SetDrawTexture(mAnimation.GetCurrentTexture());
		}
	}
}
