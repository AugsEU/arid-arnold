namespace AridArnold
{
	abstract class CinematicCommand
	{
		#region rMembers

		MonoRange<int> mFrameSpan;

		#endregion rMembers



		#region rInit

		/// <summary>
		/// Load cinematic command from xml
		/// </summary>
		public CinematicCommand(XmlNode cmdNode)
		{
			string frameSpanStr = cmdNode.Attributes["frames"].Value;
			frameSpanStr = frameSpanStr.Replace(" ", string.Empty).Trim();

			string[] framesStrs = frameSpanStr.Split(",");
			MonoDebug.Assert(framesStrs.Length == 2);

			int firstFrame = int.Parse(framesStrs[0]);
			int lastFrame = int.Parse(framesStrs[2]);
			mFrameSpan = new MonoRange<int>(firstFrame, lastFrame);

			MonoDebug.Assert(mFrameSpan.IsValid());
		}

		#endregion rInit



		#region rUpdate

		/// <summary>
		/// Update cinematic command
		/// </summary>
		public abstract void Update(GameTime gameTime, int currentFrame);



		/// <summary>
		/// Spaceship operator on frame. Negative = Not yet active, Zero = Active, Positive = No longer active
		/// </summary>
		public int FrameSpaceship(int frame)
		{
			if(frame < mFrameSpan.GetMin())
			{
				return frame - mFrameSpan.GetMin();
			}

			return Math.Max(frame - mFrameSpan.GetMax(), 0);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the cinematic command
		/// </summary>
		public abstract void Draw(DrawInfo drawInfo);

		#endregion rDraw


		#region rUtil

		/// <summary>
		/// Get frame range
		/// </summary>
		public MonoRange<int> GetFrameRange()
		{
			return mFrameSpan;
		}

		#endregion rUtil
	}
}
