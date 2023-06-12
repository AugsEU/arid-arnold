namespace AridArnold
{
	/// <summary>
	/// Grill vogel NPC
	/// </summary>
	class GrillVogel : NPC
	{
		#region rConstants

		const float TALK_DISTANCE = 30.0f;
		const int NUM_STORIES = 16;
		const int NUM_GREETINGS = 8;

		#endregion rConstants





		#region rMembers

		bool mTalking;
		protected IdleAnimator mIdleAnimation;
		protected Texture2D mTalkTexture;
		protected Texture2D mAngryTexture;

		MonoRandom mStoryRandom;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Put SimpleTalkNPC at a position
		/// </summary>
		public GrillVogel(Vector2 pos) : base(pos)
		{
			mTalking = false;
			mStyle.mFont = FontManager.I.GetFont("Pixica-24");
			mStyle.mScrollSpeed = 1.6f;
			mStyle.mFramesPerLetter = 20;
			mStyle.mBorderColor = new Color(204, 122, 0, 200);
			mStyle.mFillColor = new Color(50, 50, 0, 60);

			mStoryRandom = new MonoRandom();
		}



		/// <summary>
		/// Load barbara textures.
		/// </summary>
		public override void LoadContent()
		{
			//Setup idle animation.
			Animator idleAnim = new Animator(Animator.PlayType.OneShot,
												("NPC/GrillVogel/Idle1", 3.8f));

			Animator blinkAnim = new Animator(Animator.PlayType.OneShot,
												("NPC/GrillVogel/Idle2", 0.05f),
												("NPC/GrillVogel/Idle3", 0.05f),
												("NPC/GrillVogel/Idle2", 0.1f));

			mIdleAnimation = new IdleAnimator(idleAnim, 70.0f, blinkAnim);

			//Talk textures.
			mTalkTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/GrillVogel/Talk1");
			mAngryTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/GrillVogel/Angry1");

			base.LoadContent();
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update Grill Vogel
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			if (!mTalking && EntityManager.I.AnyNearMe(TALK_DISTANCE, this, typeof(Arnold)))
			{
				mTalking = true;
				SayRandomGreeting();
			}

			if (mTalking)
			{
				if (GetCurrentBlock().GetNumStringsInQueue() < 2)
				{
					QueueUpInaneStories();
				}
			}

			mIdleAnimation.Update(gameTime);

			base.Update(gameTime);
		}



		/// <summary>
		/// Queue up a bunch of inane stories.
		/// </summary>
		private void QueueUpInaneStories()
		{
			List<string> storyStringIDs = new List<string>();
			for(int i = 0; i < NUM_STORIES; ++i)
			{
				string stringID = "NPC.GrillVogel.Inane" + i.ToString();
				storyStringIDs.Add(stringID);
			}

			MonoAlg.ShuffleList(ref storyStringIDs, ref mStoryRandom);

			foreach(string stringID in storyStringIDs)
			{
				AppendToDialog(stringID);
			}
		}

		/// <summary>
		/// Say a random greeting.
		/// </summary>
		private void SayRandomGreeting()
		{
			int randomGreetingIdx = mStoryRandom.GetIntRange(0, NUM_GREETINGS - 1);

			AddDialogBox("NPC.GrillVogel.Intro" + randomGreetingIdx.ToString());
		}
		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Just draw talking texture.
		/// </summary>
		protected override Texture2D GetDrawTexture()
		{
			return GetTalkingDrawTexture();
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
	}
}
