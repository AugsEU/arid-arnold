namespace AridArnold
{
	internal class Barbara : SimpleTalkNPC
	{
		#region rInitialisation

		/// <summary>
		/// Put Barbara at a position
		/// </summary>
		public Barbara(Vector2 pos) : base(pos)
		{
			mStyle.mFillColor = new Color(42, 18, 35, 160);
			mStyle.mBorderColor = new Color(115, 37, 198);
		}



		/// <summary>
		/// Load barbara textures.
		/// </summary>
		public override void LoadContent()
		{
			//Setup idle animation.
			Animator idleAnim = new Animator(Animator.PlayType.OneShot,
												("NPC/Barbara/Idle1", 1.0f));

			Animator lookUpAnim = new Animator(Animator.PlayType.OneShot,
												("NPC/Barbara/Idle5", 0.2f),
												("NPC/Barbara/Idle2", 1.2f),
												("NPC/Barbara/Idle5", 0.2f));

			Animator footAnim = new Animator(Animator.PlayType.OneShot,
												("NPC/Barbara/Idle3", 0.7f));

			Animator scratchAnim = new Animator(Animator.PlayType.OneShot,
													("NPC/Barbara/Idle4", 0.2f),
													("NPC/Barbara/Idle1", 0.3f),
													("NPC/Barbara/Idle4", 0.2f));

			mIdleAnimation = new IdleAnimator(idleAnim, 50.0f, lookUpAnim, footAnim, scratchAnim);

			//Talk textures.
			mTalkTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/Barbara/Talk1");
			mAngryTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/Barbara/Angry1");
			mMouthClosedTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/Barbara/Idle1");

			base.LoadContent();
		}

		#endregion rInitialisation
	}
}
