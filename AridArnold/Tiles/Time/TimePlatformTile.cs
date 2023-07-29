using Microsoft.Xna.Framework;

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
			EventManager.I.AddListener(EventType.TimeChanged, OnTimeChange);
		}



		/// <summary>
		/// Update wall animations
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update(GameTime gameTime)
		{
			mDisplayTimeZone = TimeZoneManager.I.GetCurrentTimeZone();
			Animator anim;
			if(mTimeToAnimator.TryGetValue(mDisplayTimeZone, out anim))
			{
				anim.Update(gameTime);
			}
			else
			{
				LoadNewAnimation(mDisplayTimeZone);
			}

			base.Update(gameTime);
		}


		void OnTimeChange(EArgs eArgs)
		{
			mDisplayTimeZone = TimeZoneManager.I.GetCurrentTimeZone();
			Animator anim;
			if (!mTimeToAnimator.TryGetValue(mDisplayTimeZone, out anim))
			{
				LoadNewAnimation(mDisplayTimeZone);
			}
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
