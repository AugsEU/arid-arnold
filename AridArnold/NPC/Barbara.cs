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
			mStyle.mScrollSpeed = 0.9f;
			mStyle.mFramesPerLetter = 14;
		}



		/// <summary>
		/// Load barbara textures.
		/// </summary>
		public override void LoadContent(ContentManager content)
		{
			//Setup idle animation.
			Animator idleAnim = new Animator(content, Animator.PlayType.OneShot,
												("NPC/Barbara/Idle1", 1.0f));

			Animator lookUpAnim = new Animator(content, Animator.PlayType.OneShot,
												("NPC/Barbara/Idle5", 0.2f),
												("NPC/Barbara/Idle2", 1.2f),
												("NPC/Barbara/Idle5", 0.2f));

			Animator footAnim = new Animator(content, Animator.PlayType.OneShot,
												("NPC/Barbara/Idle3", 0.7f));

			Animator scratchAnim = new Animator(content, Animator.PlayType.OneShot,
													("NPC/Barbara/Idle4", 0.2f),
													("NPC/Barbara/Idle1", 0.3f),
													("NPC/Barbara/Idle4", 0.2f));

			mIdleAnimation = new IdleAnimator(idleAnim, 50.0f, lookUpAnim, footAnim, scratchAnim);

			//Talk textures.
			mTalkTexture = content.Load<Texture2D>("NPC/Barbara/Talk1");
			mAngryTexture = content.Load<Texture2D>("NPC/Barbara/Angry1");

			base.LoadContent(content);
		}

		#endregion rInitialisation




		#region rDialog

		/// <summary>
		/// Say something.
		/// </summary>
		protected override void DoNormalSpeak()
		{
			uint curLevel = ProgressManager.I.GetLevelPointHex();

			switch (curLevel)
			{
				case 0x0000:
					AddDialogBox("NPC.Barbara.Level0");
					break;
				case 0x0100:
					AddDialogBox("NPC.Barbara.Level10");
					break;
				default:
					break;
			}
		}


		/// <summary>
		/// Shout at the player for leaving early.
		/// </summary>
		protected override void HecklePlayer()
		{
			AddDialogBox("NPC.Barbara.Heckle");
		}

		#endregion rDialog
	}
}
