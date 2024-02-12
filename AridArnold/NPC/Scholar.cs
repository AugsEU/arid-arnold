namespace AridArnold
{
	internal class Scholar : WalkingSimpleNPC
	{
		public Scholar(Vector2 pos) : base(pos)
		{
			mStyle.mFillColor = new Color(30, 30, 30, 160);
			mStyle.mBorderColor = new Color(57, 37, 188);
		}

		public override void LoadContent()
		{
			//Setup idle animation.
			Animator idleAnim = new Animator(Animator.PlayType.Repeat,
												("NPC/Scholar/Idle1", 1.0f));

			Animator itchAnim = new Animator(Animator.PlayType.Repeat,
									("NPC/Scholar/Idle1", 0.2f),
									("NPC/Scholar/Idle2", 0.2f),
									("NPC/Scholar/Idle1", 0.2f),
									("NPC/Scholar/Idle2", 0.2f));

			mIdleAnimation = new IdleAnimator(idleAnim, 50.0f, itchAnim);

			//Talk textures.
			mTalkTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/Scholar/Talk1");
			mAngryTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/Scholar/Angry1");
			mMouthClosedTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/Scholar/Idle1");

			mWalkAnim = new Animator(Animator.PlayType.Repeat,
										("NPC/Scholar/Idle1", 0.2f),
										("NPC/Scholar/Walk1", 0.2f));
			mWalkAnim.Play();

			base.LoadContent();
		}
	}
}
