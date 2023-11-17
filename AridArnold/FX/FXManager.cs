namespace AridArnold
{
	/// <summary>
	/// Manages all effects that display on screen.
	/// </summary>
	internal class FXManager : Singleton<FXManager>
	{
		#region rMembers

		List<FX> mFXList = new List<FX>();
		int mScreenWidth = 0;
		int mScreenHeight = 0;

		#endregion





		#region rInitialisation

		/// <summary>
		/// Initialise the FXManager with a screen width and height
		/// </summary>
		public void Init(int screenWidth, int screenHeight)
		{
			mScreenHeight = screenHeight;
			mScreenWidth = screenWidth;
		}

		#endregion rInitialisation





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
		/// Adds a water drop
		/// </summary>
		public void AddDrop(Vector2 pos, float distance, Color mainColor, Color secondColor)
		{
			mFXList.Add(new WaterDrop(pos, distance, mainColor, secondColor));
		}



		/// <summary>
		/// Add animtion
		/// </summary>
		public void AddAnimator(Vector2 pos, Animator anim, DrawLayer drawLayer)
		{
			mFXList.Add(new AnimationFX(pos, anim, drawLayer));
		}



		/// <summary>
		/// Add animation
		/// </summary>
		public void AddAnimator(Vector2 pos, string anim, DrawLayer drawLayer)
		{
			mFXList.Add(new AnimationFX(pos, anim, drawLayer));
		}



		/// <summary>
		/// Add an effect
		/// </summary>
		/// <param name="fx"></param>
		public void AddFX(FX fx)
		{
			mFXList.Add(fx);
		}



		/// <summary>
		/// Clear all effects
		/// </summary>
		public void Clear()
		{
			mFXList.Clear();
		}

		#endregion rAddEffects





		#region rUtility

		/// <summary>
		/// Get the size we can draw
		/// </summary>
		public Point GetDrawableSize()
		{
			return new Point(mScreenWidth, mScreenHeight);
		}

		#endregion rUtility
	}
}
