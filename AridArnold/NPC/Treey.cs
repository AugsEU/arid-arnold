namespace AridArnold
{
	internal class Treey : WalkingSimpleNPC
	{
		public Treey(Vector2 pos) : base(pos)
		{
			mStyle.mFillColor = new Color(30, 30, 40, 160);
			mStyle.mBorderColor = new Color(47, 61, 252);
		}

		public override void LoadContent()
		{
			//Setup idle animation.
			Animator idleAnim = new Animator(Animator.PlayType.Repeat,
												("NPC/Treey/Idle1", 1.0f));

			mIdleAnimation = new IdleAnimator(idleAnim, 50.0f);

			//Talk textures.
			mTalkTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/Treey/Talk1");
			mAngryTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/Treey/Talk1");
			mMouthClosedTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/Treey/Idle1");

			mWalkAnim = new Animator(Animator.PlayType.Repeat,
										("NPC/Treey/Walk1", 0.2f),
										("NPC/Treey/Walk2", 0.2f),
										("NPC/Treey/Walk3", 0.2f));
			mWalkAnim.Play();

			//HACK
			mPosition -= GravityVecNorm();

			base.LoadContent();
		}
	}
}
