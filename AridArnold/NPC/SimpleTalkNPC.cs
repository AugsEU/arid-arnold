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

		// Time
		bool mNPCInTime;
		bool mNPCInAge;

		// Textures
		IdleAnimator mIdleAnimation;
		Animator mWalkAnimation;
		Texture2D mTalkTexture;
		Texture2D mAngryTexture;
		Texture2D mMouthClosedTexture;

		// Particles
		List<ParticleEmitter> mEmitters;

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
		public SimpleTalkNPC(Vector2 pos, string dataPath, string talkText, string heckleText, bool npcInTime, bool npcInAge) : base(pos)
		{
			mNPCInTime = npcInTime;
			mNPCInAge = npcInAge;

			mTalking = false;
			mDataPath = Path.Join("Content/", dataPath);
			mTalkText = talkText;
			mHeckleText = heckleText;

			mWalkTimer = new PercentageTimer(WALK_FREQUENCY);
			mWalkTimer.Start();
			mWalkSpeed = WALK_SPEED;
			mIsWalkingType = false;

			mEmitters = new List<ParticleEmitter>();
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

			// System of fallbacks for textures
			List<string> idleAnimPaths = new List<string>{ "Idle" };
			List<string> talkNormalPaths = new List<string> { "TalkNormal" };
			List<string> talkAngryPaths = new List<string> { "TalkAngry" };
			List<string> talkClosedPaths = new List<string> { "Default" };

			if(mNPCInAge || mNPCInTime)
			{
				AppendTimeAllInList(ref idleAnimPaths);
				AppendTimeAllInList(ref talkNormalPaths);
				AppendTimeAllInList(ref talkAngryPaths);
				AppendTimeAllInList(ref talkClosedPaths);
			}

			mIdleAnimation = MonoData.I.LoadFirstThatExistsFolderIdleAnimator(folder, idleAnimPaths);
			mTalkTexture = MonoData.I.LoadFirstThatExistsFolderTexture2D(folder, talkNormalPaths);
			mAngryTexture = MonoData.I.LoadFirstThatExistsFolderTexture2D(folder, talkAngryPaths);
			mMouthClosedTexture = MonoData.I.LoadFirstThatExistsFolderTexture2D(folder, talkClosedPaths);

			if (mIsWalkingType)
			{
				string walkAnim = Path.Combine(folder, "Walk.max");

				if (MonoData.I.FileExists(walkAnim))
				{
					mWalkAnimation = MonoData.I.LoadAnimator(walkAnim);
					mWalkAnimation.Play();
				}
			}

			XmlNode emittersNode = rootNode["emitters"];
			if(emittersNode is not null)
			{
				LoadEmitters(emittersNode);
			}

			base.LoadContent();
		}



		/// <summary>
		/// Load particle emitters
		/// </summary>
		void LoadEmitters(XmlNode emittersNode)
		{
			foreach(XmlNode emitterNode in  emittersNode.ChildNodes)
			{
				ParticleEmitter emitter = ParticleEmitter.FromXML(emitterNode);

				// Assumes gravity is down...
				Vector2 relativePos = emitter.GetPos();
				if(mPrevDirection == WalkDirection.Left)
				{
					relativePos.X = ColliderBounds().Width - relativePos.X;
				}

				relativePos += mPosition;

				emitter.SetPos(relativePos);

				mEmitters.Add(emitter);
			}
		}



		/// <summary>
		/// Append string based on time
		/// </summary>
		string AppendBasedOnTime(string toAppend)
		{
			if(toAppend is null)
			{
				return null;
			}

			if(toAppend.Length == 0)
			{
				return "";
			}

			int time = TimeZoneManager.I.GetCurrentTimeZone();
			int age = TimeZoneManager.I.GetCurrentPlayerAge();

			if(mNPCInTime && mNPCInAge)
			{
				return string.Format("{0}{1}{2}", toAppend, time.ToString(), age.ToString());
			}
			else if(mNPCInTime)
			{
				return string.Format("{0}{1}", toAppend, time.ToString());
			}
			else if(mNPCInAge)
			{
				return string.Format("{0}{1}", toAppend, age.ToString());
			}

			return toAppend;
		}



		/// <summary>
		/// Append time suffix to items in list
		/// </summary>
		void AppendTimeAllInList(ref List<string> paths)
		{
			for(int i = 0; i < paths.Count; i+=2)
			{
				paths.Insert(0, AppendBasedOnTime(paths[i]));
			}
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

			if(EventManager.I.IsSignaled(EventType.TimeChanged))
			{
				// BODGE
				LoadContent();
			}

			mIdleAnimation.Update(gameTime);
			foreach(ParticleEmitter emitter in mEmitters)
			{
				emitter.Update(gameTime);
			}

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

			if(mWalkAnimation is not null) mWalkAnimation.Update(gameTime);

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
		public override Texture2D GetDrawTexture()
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
			string talkID = GetStringIDFromBaseID(mTalkText);
			AddDialogBox(talkID);
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
			string talkID = GetStringIDFromBaseID(mHeckleText);
			AddDialogBox(talkID);
		}



		/// <summary>
		/// Get string ID to load
		/// </summary>
		string GetStringIDFromBaseID(string baseID)
		{
			string timeID = AppendBasedOnTime(baseID);
			if (LanguageManager.I.KeyExists(timeID))
			{
				return timeID;
			}

			return baseID;
		}

		#endregion rDialog
	}
}
