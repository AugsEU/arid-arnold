namespace AridArnold
{
	/// <summary>
	/// Frame of animation
	/// </summary>
	struct AnimationFrame
	{
		public AnimationFrame(Texture2D img, float t)
		{
			mImage = img;
			mDuration = t;
		}

		public Texture2D mImage;
		public float mDuration;
	}





	/// <summary>
	/// Represents an animation
	/// </summary>
	internal class Animator
	{
		#region rType

		public enum PlayType
		{
			OneShot, // Play the animation once then stop
			Repeat // Play the animation on repeat
		};

		#endregion rType





		#region rMembers

		List<AnimationFrame> mFrames = new List<AnimationFrame>();
		PlayType mPlayType;
		float mTotalDuration;
		float mPlayHead;
		bool mPlaying;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Animator constructor
		/// </summary>
		/// <param name="playType">Play mode.(See enum for details)</param>
		public Animator(PlayType playType = PlayType.Repeat)
		{
			mPlaying = false;
			mTotalDuration = 0.0f;
			mPlayHead = 0.0f;
			mPlayType = playType;
		}



		/// <summary>
		/// Add a frame to this animation.
		/// Frames can only be added, not removed.
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		/// <param name="textureName">Texture path to load</param>
		/// <param name="duration">Duration in seconds of this frame</param>
		public void LoadFrame(ContentManager content, string textureName, float duration)
		{
			mTotalDuration += duration;
			mFrames.Add(new AnimationFrame(content.Load<Texture2D>(textureName), duration));
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update animation
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public void Update(GameTime gameTime)
		{
			if (mPlaying)
			{
				mPlayHead += (float)gameTime.ElapsedGameTime.TotalSeconds;

				switch (mPlayType)
				{
					case PlayType.OneShot:
						if (mPlayHead > mTotalDuration)
						{
							Stop();
						}
						break;
					case PlayType.Repeat:
						while (mPlayHead > mTotalDuration)
						{
							mPlayHead -= mTotalDuration;
						}
						break;
					default:
						break;
				}

			}
		}



		/// <summary>
		/// Begin/Resume playing
		/// </summary>
		public void Play()
		{
			mPlaying = true;
			mPlayHead = 0.0f;
		}



		/// <summary>
		/// Stop playing
		/// </summary>
		public void Stop()
		{
			mPlaying = false;
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get texture that is currently showing
		/// </summary>
		/// <returns>Texture that is currently showing</returns>
		public Texture2D GetCurrentTexture()
		{
			float timeLeft = mPlayHead;
			int i = 0;
			for (; i < mFrames.Count; i++)
			{
				timeLeft -= mFrames[i].mDuration;

				if (timeLeft < 0.0f)
				{
					break;
				}
			}

			return mFrames[i].mImage;
		}



		/// <summary>
		/// Get texture at index
		/// </summary>
		/// <param name="index">Frame index you want to access.</param>
		/// <returns>Texture at specified index</returns>
		public Texture2D GetTexture(int index)
		{
			return mFrames[index].mImage;
		}

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Check if we are playing
		/// </summary>
		/// <returns>True if we are playing</returns>
		public bool IsPlaying()
		{
			return mPlaying;
		}



		/// <summary>
		/// Set type of animation play
		/// </summary>
		public void SetType(PlayType type)
		{
			mPlayType = type;
		}

		#endregion rUtility
	}
}
