using Microsoft.Xna.Framework;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Input;
using static AridArnold.RailNode;

namespace AridArnold
{
	internal class GameCinematic
	{
		#region rConstant

		const int CINE_FRAME_RATE = 60;

		#endregion rConstant





		#region rStatic

		static Dictionary<string, Type> sCommandNameMapping = new Dictionary<string, Type>();

		#endregion rStatic





		#region rMembers

		List<CinematicActor> mActors;
		List<CinematicCommand> mCommands;

		int mTotalFrameCount = 0;
		double mElapsedTime;
		bool mIsPlaying;

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
			mIsPlaying = false;
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
				mCommands.Add(CreateCommand(commandNode));
			}

			mTotalFrameCount = 0;
			foreach (CinematicCommand command in mCommands)
			{
				mTotalFrameCount = Math.Max(mTotalFrameCount, command.GetFrameRange().GetMax());
			}
		}



		/// <summary>
		/// Create a cinematic command
		/// </summary>
		static CinematicCommand CreateCommand(XmlNode node)
		{
			if(sCommandNameMapping.Count == 0)
			{
				GenerateTypeMaps();
			}
			string xmlName = node.Name;

			if (!sCommandNameMapping.TryGetValue(xmlName, out Type elementType))
			{
				throw new Exception("Do not recognise cinematic command: " + xmlName);
			}

			return (CinematicCommand)Activator.CreateInstance(elementType, node);
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
			if (!mIsPlaying) return;

			mElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

			int frameNum = GetCurrentFrame();
			foreach(CinematicCommand command in mCommands)
			{
				int commandSpaceship = command.FrameSpaceship(frameNum);
				if (commandSpaceship == 0)
				{
					command.Update(gameTime, frameNum);
				}
			}
		}


		/// <summary>
		/// Start the cinematic
		/// </summary>
		public void Play()
		{
			mElapsedTime = 0.0;
			mIsPlaying = true;
		}


		/// <summary>
		/// Skip to end
		/// </summary>
		public void EndPlayback()
		{
			mElapsedTime = (double)mTotalFrameCount/CINE_FRAME_RATE;
			mIsPlaying = false;
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the cinematic
		/// </summary>
		public void Draw(DrawInfo info)
		{
			int frameNum = GetCurrentFrame();
			foreach (CinematicCommand command in mCommands)
			{
				int commandSpaceship = command.FrameSpaceship(frameNum);
				if (commandSpaceship == 0)
				{
					command.Draw(info);
				}
			}
		}

		#endregion rDraw





		#region rUtil

		/// <summary>
		/// Get the current "frame"
		/// </summary>
		int GetCurrentFrame()
		{
			return (int)(mElapsedTime * CINE_FRAME_RATE + 0.0001);
		}

		#endregion rUtil
	}
}
