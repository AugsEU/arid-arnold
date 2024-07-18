namespace AridArnold
{

	/// <summary>
	/// A class that simply does a fade effect within an area
	/// </summary>
	abstract class Fade
	{
		protected static Rectangle DEFAULT_FADE_AREA = new Rectangle(0, 0, GameScreen.GAME_AREA_WIDTH, GameScreen.GAME_AREA_HEIGHT);

		protected Rectangle mArea; // The area to fade in/out

		/// <summary>
		/// Create fade within area
		/// </summary>
		public Fade(Rectangle area)
		{
			SetArea(area);
		}

		/// <summary>
		/// Set the area to draw in.
		/// </summary>
		public void SetArea(Rectangle area)
		{
			mArea = area;
		}

		/// <summary>
		/// Draw the fade at a specific percentage(0.0f to 1.0f)
		/// </summary>
		public abstract void DrawAtTime(DrawInfo info, float time);
	}
}
