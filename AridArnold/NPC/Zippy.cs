namespace AridArnold
{
	/// <summary>
	/// A racoon NPC
	/// </summary>
	internal class Zippy : SimpleTalkNPC
	{

		#region rMembers

		IdleAnimator mIdleAnimation;
		Texture2D mTalkTexture;
		Texture2D mAngryTexture;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Put Zippy at a position
		/// </summary>
		public Zippy(Vector2 pos) : base(pos)
		{
			mStyle.mScrollSpeed = 1.1f;
			mStyle.mFramesPerLetter = 11;
		}



		/// <summary>
		/// Load Zippy textures.
		/// </summary>
		public override void LoadContent(ContentManager content)
		{
			//Setup idle animation.
			Animator idleAnim = new Animator(content, Animator.PlayType.OneShot,
												("NPC/Zippy/Idle1", 0.3f));

			Animator breatheOut = new Animator(content, Animator.PlayType.OneShot,
												("NPC/Zippy/Idle2", 0.8f));

			Animator sniffAnim = new Animator(content, Animator.PlayType.OneShot,
												("NPC/Zippy/Idle3", 0.2f),
												("NPC/Zippy/Idle1", 0.8f),
												("NPC/Zippy/Idle3", 0.2f));

			Animator tailAnim = new Animator(content, Animator.PlayType.OneShot,
												("NPC/Zippy/Idle4", 0.3f),
												("NPC/Zippy/Idle5", 0.4f),
												("NPC/Zippy/Idle4", 0.2f),
												("NPC/Zippy/Idle1", 0.8f),
												("NPC/Zippy/Idle4", 0.25f),
												("NPC/Zippy/Idle5", 0.3f),
												("NPC/Zippy/Idle4", 0.2f));

			mIdleAnimation = new IdleAnimator(idleAnim, 90.0f, breatheOut, tailAnim, sniffAnim);

			//Talk textures.
			mTalkTexture = content.Load<Texture2D>("NPC/Zippy/Talk1");
			mAngryTexture = content.Load<Texture2D>("NPC/Zippy/Angry1");

			base.LoadContent(content);
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update Zippy animations.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mIdleAnimation.Update(gameTime);

			base.Update(gameTime);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw Zippy
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
				case 0x0001:
					AddDialogBox("NPC.Zippy.Level1");
					break;
				case 0x0002:
					AddDialogBox("NPC.Zippy.Level2");
					break;
				case 0x0003:
					AddDialogBox("NPC.Zippy.Level3");
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
			AddDialogBox("NPC.Zippy.Heckle");
		}

		#endregion rDialog
	}
}
