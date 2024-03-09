namespace AridArnold
{
	internal class TimePlatformTile : PlatformTile
	{
		Dictionary<int, Animator> mTimeToAnimator;
		string mAnimName;
		int mDisplayTimeZone;



		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public TimePlatformTile(CardinalDirection rot, Vector2 position, string animName) : base(rot, position)
		{
			mAnimName = animName;
			mTimeToAnimator = new Dictionary<int, Animator>();
			mDisplayTimeZone = TimeZoneManager.I.GetCurrentTimeZone();
			LoadNewAnimation(mDisplayTimeZone);
		}



		/// <summary>
		/// Update wall animations
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update(GameTime gameTime)
		{
			mDisplayTimeZone = TimeZoneManager.I.GetCurrentTimeZone();
			Animator anim;
			if (EventManager.I.IsSignaled(EventType.TimeChanged))
			{
				mDisplayTimeZone = TimeZoneManager.I.GetCurrentTimeZone();
				if (!mTimeToAnimator.TryGetValue(mDisplayTimeZone, out anim))
				{
					LoadNewAnimation(mDisplayTimeZone);
				}
			}

			if (mTimeToAnimator.TryGetValue(mDisplayTimeZone, out anim))
			{
				anim.Update(gameTime);
			}
			else
			{
				LoadNewAnimation(mDisplayTimeZone);
			}

			base.Update(gameTime);
		}


		/// <summary>
		/// Load tile animation for zone
		/// </summary>
		void LoadNewAnimation(int time)
		{
			Animator newAnim = MonoData.I.LoadAnimator(mAnimName + time.ToString());
			mTimeToAnimator.Add(time, newAnim);
		}



		/// <summary>
		/// Get texture for this tile
		/// </summary>
		public override Texture2D GetTexture()
		{
			return mTimeToAnimator[mDisplayTimeZone].GetCurrentTexture();
		}
	}
}
