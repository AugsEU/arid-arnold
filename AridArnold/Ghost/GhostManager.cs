namespace AridArnold
{
	struct GhostInfo
	{
		public Vector2 position;
		public Vector2 velocity;
		public bool grounded;
		public CardinalDirection gravity;
	}

	/// <summary>
	/// Controls and manages replay ghosts
	/// </summary>
	internal class GhostManager : Singleton<GhostManager>
	{
		#region rConstants

		//Only record every 4th frame.
		const int FRAME_SUB = 4;

		#endregion rConstants





		#region rMembers

		Level mCurrentLevel;

		int mCurrentFrame;
		int mRecordFrame;

		GhostArnold mGhostArnold;

		GhostFile mInputFile;
		GhostFile mOutputFile;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Load content such as textures
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public void Load(ContentManager content)
		{
			mGhostArnold = new GhostArnold(Vector2.Zero);
			mGhostArnold.LoadContent(content);
		}



		/// <summary>
		/// Start level
		/// </summary>
		/// <param name="level">Level we are starting</param>
		public void StartLevel(Level level)
		{
			mCurrentLevel = level;
			mCurrentFrame = 0;
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
			mCurrentFrame++;

			if (mRecordFrame < mInputFile.GetFrameCount())
			{
				mGhostArnold.Update(gameTime);
			}

			if (mCurrentFrame % FRAME_SUB == 0)
			{
				RecordFrame();
				mRecordFrame++;
			}
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
					mOutputFile.RecordFrame(arnold, mRecordFrame);
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
			if (mInputFile.IsEmpty() == false)
			{
				List<GhostInfo> ghosts = mInputFile.ReadFrame(mRecordFrame);

				foreach (GhostInfo ghost in ghosts)
				{
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
			return FrameTimeToString(mOutputFile.GetFrameCount());
		}



		/// <summary>
		/// Get time we need to beat.
		/// </summary>
		/// <returns>Formatted time to beat.</returns>
		public string GetTimeToBeat()
		{
			if (mInputFile.IsEmpty())
			{
				return "";
			}

			return FrameTimeToString(mInputFile.GetFrameCount());
		}



		/// <summary>
		/// Get difference between output and input time.
		/// </summary>
		/// <returns>Integer frame count between output and input file times</returns>
		public int? GetTimeDifference()
		{
			if (mInputFile.IsEmpty() || mOutputFile.IsEmpty())
			{
				return null;
			}

			return mOutputFile.GetFrameCount() - mInputFile.GetFrameCount();
		}



		/// <summary>
		/// Format time (in frames) into a string
		/// TO DO: Put this in utils.
		/// </summary>
		/// <param name="frame">Time in frames</param>
		/// <returns>Formatted time string.</returns>
		public string FrameTimeToString(int frame)
		{
			int ms = (int)(frame * (1000.0f / 60.0f));
			int cs = ms / 10;
			int s = cs / 100;
			int m = s / 60;


			if (m == 0)
			{
				return String.Format("{0:D2} : {1:D2}", s, cs % 100);
			}
			else
			{
				return String.Format("{0:D} : {1:D2} : {2:D2}", m, s % 60, cs % 100);
			}
		}

		#endregion rUtility
	}
}
