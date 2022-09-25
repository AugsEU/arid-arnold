namespace AridArnold
{
	/// <summary>
	/// Manages all effects that display on screen.
	/// </summary>
	internal class FXManager : Singleton<FXManager>
	{
		#region rMembers

		List<FX> mFXList = new List<FX>();

		#endregion





		#region rUpdate

		/// <summary>
		/// Update all effects
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public void Update(GameTime gameTime)
		{
			for (int i = 0; i < mFXList.Count; i++)
			{
				FX fx = mFXList[i];

				if (fx.Finished())
				{
					mFXList.RemoveAt(i);
					i--;
				}
				else
				{
					fx.Update(gameTime);
				}
			}
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draws all effects
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		public void Draw(DrawInfo info)
		{
			foreach (FX fx in mFXList)
			{
				fx.Draw(info);
			}
		}

		#endregion rDraw





		#region rAddEffects

		/// <summary>
		/// Add small piece of text that scrolls up.
		/// </summary>
		/// <param name="font">Font to draw text in</param>
		/// <param name="colour">Colour of text</param>
		/// <param name="pos">Starting position</param>
		/// <param name="text">String to display</param>
		/// <param name="upSpeed">Speed at which text goes up</param>
		/// <param name="maxHeight">Maximum height difference reached by text</param>
		/// <param name="time">Time that text shows up</param>
		public void AddTextScroller(SpriteFont font, Color colour, Vector2 pos, string text, float upSpeed = 4.1f, float maxHeight = 10.0f, float time = 20.0f)
		{
			mFXList.Add(new ScrollerTextFX(font, colour, pos, text, upSpeed, maxHeight, time));
		}



		/// <summary>
		/// Clear all effects
		/// </summary>
		public void Clear()
		{
			mFXList.Clear();
		}

		#endregion rAddEffects
	}
}
