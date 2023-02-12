namespace AridArnold
{
	internal class Dok : SimpleTalkNPC
	{
		#region rInitialisation

		/// <summary>
		/// Put Dok at a position
		/// </summary>
		public Dok(Vector2 pos) : base(pos)
		{
			mStyle.mScrollSpeed = 0.5f;
			mStyle.mFramesPerLetter = 22;
		}



		/// <summary>
		/// Load Dok textures.
		/// </summary>
		public override void LoadContent(ContentManager content)
		{
			//Setup idle animation.
			Animator idleAnim = new Animator(content, Animator.PlayType.OneShot,
												("NPC/Dok/Idle1", 1.2f));

			Animator breatheOut = new Animator(content, Animator.PlayType.OneShot,
												("NPC/Dok/Idle2", 0.8f));

			Animator stickSmack = new Animator(content, Animator.PlayType.OneShot,
												("NPC/Dok/Idle3", 0.3f),
												("NPC/Dok/Idle1", 0.8f),
												("NPC/Dok/Idle3", 0.3f));

			mIdleAnimation = new IdleAnimator(idleAnim, 90.0f, breatheOut, stickSmack);

			//Talk textures.
			mTalkTexture = content.Load<Texture2D>("NPC/Dok/Talk1");
			mAngryTexture = content.Load<Texture2D>("NPC/Dok/Angry1");

			base.LoadContent(content);
		}

		#endregion rInitialisation
	}
}
