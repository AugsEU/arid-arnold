using System.Globalization;

namespace AridArnold
{
	/// <summary>
	/// A command applied over multiple frames of a cutscene
	/// </summary>
	abstract class CinematicCommand
	{
		#region rMembers

		protected MonoRange<int> mFrameSpan;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Load cinematic command from xml
		/// </summary>
		public CinematicCommand(XmlNode cmdNode, GameCinematic parent)
		{
			string frameSpanStr = cmdNode.Attributes["frames"].InnerText;
			frameSpanStr = frameSpanStr.Replace(" ", string.Empty).Trim();

			string[] framesStrs = frameSpanStr.Split(",");
			MonoDebug.Assert(framesStrs.Length == 2 || framesStrs.Length == 1);

			if (framesStrs.Length == 1)
			{
				int frame = int.Parse(framesStrs[0], CultureInfo.InvariantCulture.NumberFormat);
				mFrameSpan = new MonoRange<int>(frame, frame);
			}
			else if (framesStrs.Length == 2)
			{
				int firstFrame = int.Parse(framesStrs[0], CultureInfo.InvariantCulture.NumberFormat);
				int lastFrame = int.Parse(framesStrs[1], CultureInfo.InvariantCulture.NumberFormat);
				mFrameSpan = new MonoRange<int>(firstFrame, lastFrame);
			}
			else
			{
				throw new NotImplementedException("Cine: frames attribute incorrectly formatted");
			}

			MonoDebug.Assert(mFrameSpan.IsValid());
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update cinematic command
		/// </summary>
		public virtual void Update(GameTime gameTime, int currentFrame) { }



		/// <summary>
		/// Spaceship operator on frame. Negative = Not yet active, Zero = Active, Positive = No longer active
		/// </summary>
		public int FrameSpaceship(int frame)
		{
			if (frame < mFrameSpan.GetMin())
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
		public virtual void Draw(DrawInfo info, int currentFrame) { }

		#endregion rDraw





		#region rUtil

		/// <summary>
		/// Get frame range
		/// </summary>
		public MonoRange<int> GetFrameRange()
		{
			return mFrameSpan;
		}



		/// <summary>
		/// Get percentage of activeness
		/// </summary>
		protected float GetActivePercent(int frame)
		{
			return (frame - (float)mFrameSpan.GetMin()) / (mFrameSpan.GetMax() - mFrameSpan.GetMin());
		}



		/// <summary>
		/// Reset elements of this command
		/// </summary>
		virtual public void Reset()
		{

		}

		#endregion rUtil
	}
}
