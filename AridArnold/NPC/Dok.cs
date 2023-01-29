namespace AridArnold
{
	internal class Dok : SimpleTalkNPC
	{

		#region rMembers

		IdleAnimator mIdleAnimation;
		Texture2D mTalkTexture;
		Texture2D mAngryTexture;

		#endregion rMembers





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





		#region rUpdate

		/// <summary>
		/// Update Dok animations.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mIdleAnimation.Update(gameTime);

			base.Update(gameTime);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw Dok
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			DrawTalking(info);

			base.Draw(info);
		}



		/// <summary>
		/// Get idle texture
		/// </summary>
		protected override Texture2D GetIdleTexture()
		{
			if (IsTalking())
			{
				return mIdleAnimation.GetIdleTexture();
			}

			return mIdleAnimation.GetCurrentTexture();
		}



		/// <summary>
		/// Get normal texture for talking.
		/// </summary>
		protected override Texture2D GetNormalTalkTexture()
		{
			return mTalkTexture;
		}



		/// <summary>
		/// Get exclaim texture.
		/// </summary>
		protected override Texture2D GetExclaimTalkTexture()
		{
			return mAngryTexture;
		}

		#endregion rDraw





		#region rDialog

		/// <summary>
		/// Say something.
		/// </summary>
		protected override void DoNormalSpeak()
		{
			uint curLevel = ProgressManager.I.GetLevelPointHex();

			switch (curLevel)
			{
				case 0x0100:
					AddDialogBox("NPC.Dok.Level10");
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
			AddDialogBox("NPC.Dok.Heckle");
		}

		#endregion rDialog
	}
}
