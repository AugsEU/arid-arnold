﻿using Microsoft.Xna.Framework;
using System.Linq;
using System.Reflection;

namespace AridArnold
{
	internal class GameCinematic
	{
		#region rConstant

		const int CINE_FRAME_RATE = 60;


#if DEBUG
		const int DEBUG_FRAME_SKIP = 0;
#endif

		#endregion rConstant





		#region rStatic

		static Dictionary<string, Type> sCommandNameMapping = new Dictionary<string, Type>();

		#endregion rStatic





		#region rMembers

		List<CinematicActor> mActors;
		List<CinematicCommand> mCommands;

		int mTotalFrameCount = 0;
		int mLastFrameCompleted = -1;
		double mElapsedTime;
		bool mIsPlaying;
		bool mAllowSkip;

		bool mPaused;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Load cinematic from mci file
		/// </summary>
		public GameCinematic(string path)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(path);
			XmlNode rootNode = xmlDoc.LastChild;

			LoadActors(rootNode);
			LoadCommands(rootNode);

			mElapsedTime = 0.0;
			mLastFrameCompleted = -1;
			mIsPlaying = false;

			mPaused = false;

			mAllowSkip = rootNode["noSkip"] is null;
		}



		/// <summary>
		/// Load actors
		/// </summary>
		void LoadActors(XmlNode rootNode)
		{
			mActors = new List<CinematicActor>();
			XmlNode allActorsNode = rootNode.SelectSingleNode("actors");
			XmlNodeList actorNodes = allActorsNode.ChildNodes;
			foreach (XmlNode actorNode in actorNodes)
			{
				mActors.Add(new CinematicActor(actorNode));
			}
		}



		/// <summary>
		/// Load all game commands
		/// </summary>
		void LoadCommands(XmlNode rootNode)
		{
			mCommands = new List<CinematicCommand>();
			XmlNode allCommandsNode = rootNode.SelectSingleNode("commands");
			XmlNodeList commandNodes = allCommandsNode.ChildNodes;
			foreach (XmlNode commandNode in commandNodes)
			{
				if (commandNode.NodeType == XmlNodeType.Comment)
				{
					continue;
				}

				mCommands.Add(CreateCommand(commandNode));
			}

			mTotalFrameCount = 0;
			foreach (CinematicCommand command in mCommands)
			{
				int cmdEnd = command.GetFrameRange().GetMax();
				if (cmdEnd != int.MaxValue)
				{
					mTotalFrameCount = Math.Max(mTotalFrameCount, command.GetFrameRange().GetMax());
				}
			}
		}



		/// <summary>
		/// Create a cinematic command
		/// </summary>
		CinematicCommand CreateCommand(XmlNode node)
		{
			if (sCommandNameMapping.Count == 0)
			{
				GenerateTypeMaps();
			}
			string xmlName = node.Name.ToLower();

			if (!sCommandNameMapping.TryGetValue(xmlName, out Type elementType))
			{
				throw new Exception("Do not recognise cinematic command: " + xmlName);
			}

			return (CinematicCommand)Activator.CreateInstance(elementType, node, this);
		}



		/// <summary>
		/// Called once per program, generate string -> type mapping using reflection
		/// Note: Not threadsafe.
		/// </summary>
		static void GenerateTypeMaps()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			Type elementType = typeof(CinematicCommand);
			IEnumerable<Type> types = assembly.GetTypes().Where(t => elementType.IsAssignableFrom(t) && !t.IsAbstract);

			foreach (Type type in types)
			{
				string typeName = type.Name;
				sCommandNameMapping[typeName.ToLower()] = type;
			}
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update cinematic
		/// </summary>
		public void Update(GameTime gameTime)
		{
			if (!mIsPlaying || gameTime.ElapsedGameTime.TotalMilliseconds <= 1.0) return;

			foreach (CinematicActor cinematicActor in mActors)
			{
				cinematicActor.Update(gameTime);
			}

#if DEBUG
			DebugLoop:
#endif
			if (mPaused)
			{
				// Keep them updated while paused.
				UpdateAllCommands(gameTime, mLastFrameCompleted);
			}
			else
			{
				mElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

				int runUpToFrame = GetFrameFromElapsedTime();

				while (mLastFrameCompleted < runUpToFrame)
				{
					AdvanceFrame(gameTime);
					if (mPaused)
					{
						// Stop advancing frames.
						break;
					}
				}
			}


			if (mLastFrameCompleted >= mTotalFrameCount)
			{
				mIsPlaying = false;
			}
#if DEBUG
			if (GetFrameFromElapsedTime() < DEBUG_FRAME_SKIP)
			{
				goto DebugLoop;
			}
#endif
		}



		/// <summary>
		/// Do update work for frames
		/// </summary>
		public void AdvanceFrame(GameTime gameTime)
		{
			int frameNum = mLastFrameCompleted + 1; // Ensure frames are done sequentially.

			UpdateAllCommands(gameTime, frameNum);

			mLastFrameCompleted = frameNum;
		}



		/// <summary>
		/// Update all the commands
		/// </summary>
		/// <param name="gameTime"></param>
		public void UpdateAllCommands(GameTime gameTime, int frameNum)
		{
			mPaused = false;
			foreach (CinematicCommand command in mCommands)
			{
				int commandSpaceship = command.FrameSpaceship(frameNum);
				if (commandSpaceship == 0)
				{
					command.Update(gameTime, frameNum);

					if (command.RequestPause())
					{
						mPaused = true;
					}
				}
			}
		}



		/// <summary>
		/// Start the cinematic
		/// </summary>
		public void PlayFromStart()
		{
			mElapsedTime = 0.0;
			mLastFrameCompleted = -1;
			mIsPlaying = true;
			mPaused = false;
			FullReset();
		}



		/// <summary>
		/// Skip to end
		/// </summary>
		public void SkipToEnd()
		{
			mElapsedTime = (double)mTotalFrameCount / CINE_FRAME_RATE;
			mLastFrameCompleted = mTotalFrameCount;
			mIsPlaying = false;
			mPaused = false;
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the cinematic
		/// </summary>
		public void Draw(DrawInfo info)
		{
			int frameNum = GetFrameFromElapsedTime();

			if(mLastFrameCompleted < 1)
			{
				return;
			}

			foreach (CinematicCommand command in mCommands)
			{
				int commandSpaceship = command.FrameSpaceship(frameNum);
				if (commandSpaceship == 0)
				{
					command.Draw(info, frameNum);
				}
			}

			foreach (CinematicActor actor in mActors)
			{
				actor.Draw(info);
			}
		}

		#endregion rDraw





		#region rUtil


		/// <summary>
		/// Reset all actors and such
		/// </summary>
		public void FullReset()
		{
			Camera screenCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.ScreenCamera);
			screenCam.Reset();

			foreach (CinematicCommand command in mCommands)
			{
				command.Reset();
			}

			foreach (CinematicActor actor in mActors)
			{
				actor.Reset();
			}
		}


		/// <summary>
		/// Get the frame we should be on given the time we have spent watching.
		/// </summary>
		public int GetFrameFromElapsedTime()
		{
			return (int)(mElapsedTime * CINE_FRAME_RATE + 0.0001); // Calculate frame we should be on.
		}



		/// <summary>
		/// Are we playing or paused?
		/// </summary>
		public bool IsPlaying()
		{
			return mIsPlaying;
		}



		/// <summary>
		/// Cutscene done?
		/// </summary>
		public bool IsComplete()
		{
			return mLastFrameCompleted >= mTotalFrameCount;
		}



		/// <summary>
		/// Get an actor by name
		/// </summary>
		public CinematicActor GetActorByName(string name)
		{
			foreach (CinematicActor actor in mActors)
			{
				if (actor.GetName().Equals(name, StringComparison.CurrentCultureIgnoreCase))
				{
					return actor;
				}
			}

			throw new Exception("Could not find actor named: " + name);
		}


		/// <summary>
		/// Can we skip this one?
		/// </summary>
		public bool IsSkippable()
		{
			return mAllowSkip;
		}

		#endregion rUtil
	}
}
