using System.IO;

namespace AridArnold
{
	/// <summary>
	/// A simple NPC that talks when you get near.
	/// </summary>
	abstract class SimpleTalkNPC : NPC
	{
		#region rConstants

		const float TALK_DISTANCE = 29.0f;

		#endregion rConstants





		#region rMembers

		bool mTalking;
		protected IdleAnimator mIdleAnimation;
		protected Texture2D mTalkTexture;
		protected Texture2D mAngryTexture;
		protected Texture2D mMouthClosedTexture;

		protected string mTalkText;
		protected string mHeckleText;
		string mDataPath;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Put SimpleTalkNPC at a position
		/// </summary>
		public SimpleTalkNPC(Vector2 pos, string dataPath, string talkText, string heckleText) : base(pos)
		{
			mTalking = false;
			mDataPath = dataPath;
			mTalkText = talkText;
			mHeckleText = heckleText;
		}



		/// <summary>
		/// Load content
		/// </summary>
		public override void LoadContent()
		{
			DirectoryInfo dirInfo = Directory.GetParent(mDataPath);
			string folder = dirInfo.Name;

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(mDataPath);
			XmlNode rootNode = xmlDoc.LastChild;

			mStyle = MonoParse.GetSpeechBoxStyle(rootNode["textStyle"]);

			mIdleAnimation = MonoData.I.LoadIdleAnimator(Path.Combine(folder, "Idle.mia"));
			mTalkTexture = MonoData.I.MonoGameLoad<Texture2D>(Path.Combine(folder, "TalkNormal"));
			mAngryTexture = MonoData.I.MonoGameLoad<Texture2D>(Path.Combine(folder, "TalkAngry"));
			mMouthClosedTexture = MonoData.I.MonoGameLoad<Texture2D>(Path.Combine(folder, "Default"));

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

			mIdleAnimation.Update(gameTime);

			base.Update(gameTime);
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
		/// Set text displayed when talking to the player.
		/// </summary>
		/// <param name="talkText">Talk text</param>
		public void SetTalkText(string talkText)
		{
			mTalkText = talkText;
		}



		/// <summary>
		/// Set's text that will be used to heckle the player
		/// </summary>
		/// <param name="heckleText">String ID of heckle text</param>
		public void SetHeckleText(string heckleText)
		{
			mHeckleText = heckleText;
		}

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
