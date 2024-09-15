namespace AridArnold
{
	struct GhostInfo
	{
		public bool mEnabled;
		public Vector2 mPosition;
		public Vector2 mVelocity;
		public bool mGrounded;
		public CardinalDirection mGravity;
		public WalkDirection mWalkDirection;
		public WalkDirection mPrevWalkDirection;
	}

	/// <summary>
	/// Controls and manages replay ghosts
	/// </summary>
	internal class GhostManager : Singleton<GhostManager>
	{
		#region rMembers

		int mRecordFrame;

		GhostArnold mGhostArnold;

		GhostFile mInputFile;
		GhostFile mOutputFile;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Load content such as textures
		/// </summary>
		public void Load()
		{
			mGhostArnold = new GhostArnold(Vector2.Zero);
			mGhostArnold.LoadContent();
		}



		/// <summary>
		/// Start level
		/// </summary>
		/// <param name="level">Level we are starting</param>
		public void StartLevel(Level level)
		{
			AuxData.LevelType levelType = level.GetAuxData().GetLevelType();
			if (levelType == AuxData.LevelType.Hub || levelType == AuxData.LevelType.Shop)
			{
				mInputFile = null;
				mOutputFile = null;
				return;
			}

			mRecordFrame = 0;

			mInputFile = new GhostFile(level);
			mOutputFile = new GhostFile(level);

			mGhostArnold.StartLevel();

			mInputFile.Load();
		}



		/// <summary>
		/// End level. Saves a new ghost replay if it is the fastest on record.
		/// </summary>
		/// <param name="levelWin">Did we win the level?</param>
		public void EndLevel(bool levelWin)
		{
			if(mInputFile is null || mOutputFile is null)
			{
				return;
			}

			if (levelWin)
			{
				//Check if we have set a new record
				if (mInputFile.IsEmpty() || mOutputFile.GetFrameCount() < mInputFile.GetFrameCount())
				{
					mOutputFile.Save();
				}
			}
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update ghost manager
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public void Update(GameTime gameTime)
		{
			if (mInputFile is not null && mGhostArnold is not null && mRecordFrame < mInputFile.GetFrameCount())
			{
				mGhostArnold.Update(gameTime);
			}

			if (mOutputFile is not null)
			{
				RecordFrame();
			}

			mRecordFrame++;
		}



		/// <summary>
		/// Record a frame in the output file.
		/// </summary>
		public void RecordFrame()
		{
			for (int i = 0; i < EntityManager.I.GetEntityNum(); i++)
			{
				Entity entity = EntityManager.I.GetEntity(i);

				if (entity is Arnold)
				{
					Arnold arnold = (Arnold)entity;
					bool success = mOutputFile.RecordFrame(arnold, mRecordFrame);
					if(!success)
					{
						StopRecording();
						return;
					}
				}
			}
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw all ghosts
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		public void Draw(DrawInfo info)
		{
			if(OptionsManager.I.GetGhostDisplay() == false)
			{
				// Don't display ghosts dummy.
				return;
			}

			if (mInputFile is not null && mInputFile.IsEmpty() == false)
			{
				List<GhostInfo> ghosts = mInputFile.ReadFrame(mRecordFrame);

				foreach (GhostInfo ghost in ghosts)
				{
					if (!ghost.mEnabled) continue;
					mGhostArnold.SetGhostInfo(ghost);
					mGhostArnold.Draw(info);
				}
			}
		}

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Get current time recorded in output file
		/// </summary>
		/// <returns>Formatted current time</returns>
		public string GetTime()
		{
			return mOutputFile is null ? "" : MonoText.GetTimeTextFromFrames(mOutputFile.GetFrameCount());
		}



		/// <summary>
		/// Get time we need to beat.
		/// </summary>
		/// <returns>Formatted time to beat.</returns>
		public string GetTimeToBeat()
		{
			if (mInputFile is null || mInputFile.IsEmpty())
			{
				return "";
			}

			return MonoText.GetTimeTextFromFrames(mInputFile.GetFrameCount());
		}



		/// <summary>
		/// Get difference between output and input time.
		/// </summary>
		/// <returns>Integer frame count between output and input file times</returns>
		public int? GetTimeDifference()
		{
			if (mInputFile is null || mOutputFile is null || mInputFile.IsEmpty() || mOutputFile.IsEmpty())
			{
				return null;
			}

			return mOutputFile.GetFrameCount() - mInputFile.GetFrameCount();
		}



		/// <summary>
		/// Stop recording because we are disqualified?
		/// </summary>
		public void StopRecording()
		{
			mOutputFile = null;
		}



		/// <summary>
		/// Get the output file
		/// </summary>
		public GhostFile GetOutputFile()
		{
			return mOutputFile;
		}

		#endregion rUtility
	}
}
