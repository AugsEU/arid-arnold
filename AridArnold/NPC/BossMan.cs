namespace AridArnold
{
	internal class BossMan : SimpleTalkNPC
	{
		public BossMan(Vector2 pos) : base(pos)
		{
			mStyle.mFillColor = new Color(15, 16, 45, 160);
			mStyle.mBorderColor = new Color(61, 65, 181);
		}

		public override void LoadContent()
		{
			//Setup idle animation.
			Animator idleAnim = new Animator(Animator.PlayType.OneShot,
												("NPC/BossMan/Idle1", 0.6f));

			mIdleAnimation = new IdleAnimator(idleAnim, 0.0f);

			//Talk textures.
			mTalkTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/BossMan/Talk1");
			mAngryTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/BossMan/Angry1");
			mMouthClosedTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/BossMan/Idle1");

			base.LoadContent();
		}
	}
}
