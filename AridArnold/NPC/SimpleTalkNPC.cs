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
			mDataPath = Path.Join("Content/", dataPath);
			mTalkText = talkText;
			mHeckleText = heckleText;
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

			mStyle = MonoParse.GetSpeechBoxStyle(rootNode["textStyle"]);

			mIdleAnimation = MonoData.I.LoadIdleAnimator(Path.Combine(folder, "Idle.mia"));


			string normTex = Path.Combine(folder, "TalkNormal");
			string angryTex = Path.Combine(folder, "TalkAngry");
			string closedTex = Path.Combine(folder, "Default");

			mTalkTexture = File.Exists(normTex) ? MonoData.I.MonoGameLoad<Texture2D>(normTex) : null;
			mAngryTexture = File.Exists(angryTex) ? MonoData.I.MonoGameLoad<Texture2D>(angryTex) : null;
			mMouthClosedTexture = File.Exists(closedTex) ? MonoData.I.MonoGameLoad<Texture2D>(closedTex) : null;

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
