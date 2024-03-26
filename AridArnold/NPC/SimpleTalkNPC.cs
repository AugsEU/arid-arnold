using System.IO;

namespace AridArnold
{
	/// <summary>
	/// A simple NPC that talks when you get near.
	/// </summary>
	class SimpleTalkNPC : NPC
	{
		#region rConstants

		const float TALK_DISTANCE = 29.0f;
		const float WALK_FREQUENCY = 10000.0f;
		const float WALK_SPEED = 3.0f;

		#endregion rConstants





		#region rMembers

		// Data path
		string mDataPath;

		// Textures
		IdleAnimator mIdleAnimation;
		Animator mWalkAnimation;
		Texture2D mTalkTexture;
		Texture2D mAngryTexture;
		Texture2D mMouthClosedTexture;

		// Talking
		bool mTalking;
		string mTalkText;
		string mHeckleText;

		// Walking
		bool mIsWalkingType;
		PercentageTimer mWalkTimer;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Put SimpleTalkNPC at a position
		/// </summary>
		public SimpleTalkNPC(Vector2 pos, string dataPath, string talkText, string heckleText) : base(pos)
		{
			mTalking = false;
			mDataPath = Path.Join("Content/", dataPath);
			mTalkText = talkText;
			mHeckleText = heckleText;

			mWalkTimer = new PercentageTimer(WALK_FREQUENCY);
			mWalkTimer.Start();
			mWalkSpeed = WALK_SPEED;
			mIsWalkingType = false;
		}



		/// <summary>
		/// Load content
		/// </summary>
		public override void LoadContent()
		{
			string folder = Path.GetDirectoryName(mDataPath);

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(mDataPath);
			XmlNode rootNode = xmlDoc.LastChild;

			mIsWalkingType = rootNode["walk"] is not null;
			mStyle = MonoParse.GetSpeechBoxStyle(rootNode["textStyle"]);
			mIdleAnimation = MonoData.I.LoadIdleAnimator(Path.Combine(folder, "Idle.mia"));

			string normTex = Path.Combine(folder, "TalkNormal");
			string angryTex = Path.Combine(folder, "TalkAngry");
			string closedTex = Path.Combine(folder, "Default");

			mTalkTexture = MonoData.I.FileExists(normTex) ? MonoData.I.MonoGameLoad<Texture2D>(normTex) : null;
			mAngryTexture = MonoData.I.FileExists(angryTex) ? MonoData.I.MonoGameLoad<Texture2D>(angryTex) : null;
			mMouthClosedTexture = MonoData.I.FileExists(closedTex) ? MonoData.I.MonoGameLoad<Texture2D>(closedTex) : null;

			if(mIsWalkingType)
			{
				string walkAnim = Path.Combine(folder, "Walk.max");

				mWalkAnimation = MonoData.I.LoadAnimator(walkAnim);
				mWalkAnimation.Play();
			}

			base.LoadContent();
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			float talkDistance = TALK_DISTANCE;
			if (mTalking)
			{
				talkDistance *= 2.0f;
			}

			if (EntityManager.I.AnyNearMe(talkDistance, this, typeof(Arnold), typeof(Androld)))
			{
				if (!mTalking)
				{
					DoNormalSpeak();
				}

				mTalking = true;
			}
			else
			{
				if (mTalking && IsTalking())
				{
					HecklePlayer();
				}
				mTalking = false;
			}

			if (mIsWalkingType)
			{
				WalkUpdate(gameTime);
			}

			mIdleAnimation.Update(gameTime);

			base.Update(gameTime);
		}

		#endregion rUpdate





		#region rWalk

		/// <summary>
		/// Do walking update
		/// </summary>
		public void WalkUpdate(GameTime gameTime)
		{
			if (mWalkTimer.GetPercentageF() >= 1.0f)
			{
				mWalkTimer.Reset();
			}

			mWalkAnimation.Update(gameTime);

			if (WantsWalk() && !IsTalking())
			{
				WalkAround();
			}
			else
			{
				SetWalkDirection(WalkDirection.None);
			}
		}



		/// <summary>
		/// Does the timer
		/// </summary>
		public bool WantsWalk()
		{
			return mIsWalkingType && mWalkTimer.GetPercentageF() < 0.5f;
		}



		/// <summary>
		/// Walk around.
		/// </summary>
		void WalkAround()
		{
			bool canGoWhereFacing = false;

			switch (GetPrevWalkDirection())
			{
				case WalkDirection.Left:
					canGoWhereFacing = CheckSolid(-1, 1) && !CheckSolid(-1, 0);
					break;
				case WalkDirection.Right:
					canGoWhereFacing = CheckSolid(1, 1) && !CheckSolid(1, 0);
					break;
				case WalkDirection.None:
					break;
				default:
					break;
			}

			WalkDirection newWalkDir = GetPrevWalkDirection();

			if (!canGoWhereFacing)
			{
				newWalkDir = Util.InvertDirection(newWalkDir);
			}

			SetWalkDirection(newWalkDir);
			SetPrevWalkDirection(newWalkDir);
		}

		#endregion rWalk





		#region rDraw

		/// <summary>
		/// Just draw talking texture.
		/// </summary>
		protected override Texture2D GetDrawTexture()
		{
			if (GetWalkDirection() != WalkDirection.None && mWalkAnimation is not null)
			{
				return mWalkAnimation.GetCurrentTexture();
			}

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



		/// <summary>
		/// Get texture when mouth is closed.
		/// </summary>
		protected override Texture2D GetMouthClosedTexture()
		{
			return mMouthClosedTexture;
		}

		#endregion rDraw





		#region rDialog

		/// <summary>
		/// Say something.
		/// </summary>
		protected void DoNormalSpeak()
		{
			AddDialogBox(mTalkText);
		}


		/// <summary>
		/// Shout at the player for leaving early.
		/// </summary>
		protected void HecklePlayer()
		{
			if (mHeckleText == "")
			{
				GetCurrentBlock().Stop();
				return;
			}
			AddDialogBox(mHeckleText);
		}

		#endregion rDialog
	}
}
