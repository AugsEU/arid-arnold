namespace GMTK2023
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
		/// Add animtion
		/// </summary>
		public void AddAnimator(Vector2 pos, Animator anim, DrawLayer drawLayer)
		{
			mFXList.Add(new AnimationFX(pos, anim, drawLayer));
		}

		/// <summary>
		/// Add animtion
		/// </summary>
		public void AddFlame(Vector2 pos, DrawLayer drawLayer)
		{
			pos.X += 4.0f;
			pos.Y += 4.0f;
			float ft = 0.1f;
			Animator anim = new Animator(Animator.PlayType.OneShot
				, ("Fire/Flame1", ft)
				, ("Fire/Flame2", ft)
				, ("Fire/Flame3", ft)
				, ("Fire/Flame4", ft)
				, ("Fire/Flame5", ft)
				, ("Fire/Flame6", ft)
				, ("Fire/Flame7", ft)
				, ("Fire/Flame8", ft));
			mFXList.Add(new AnimationFX(pos, anim, drawLayer));
		}

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
		public void AddTextScroller(Color colour, Vector2 pos, string text, float upSpeed = 4.1f, float maxHeight = 10.0f, float time = 20.0f)
		{
			SpriteFont font = FontManager.I.GetFont("Pixica-24");
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
