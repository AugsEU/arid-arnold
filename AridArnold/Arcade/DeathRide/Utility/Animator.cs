namespace GMTK2023
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
		public Animator(PlayType playType, params (string, float)[] frameData)
		{
			mPlaying = false;
			mTotalDuration = 0.0f;
			mPlayHead = 0.0f;
			mPlayType = playType;

			for (int i = 0; i < frameData.Length; i++)
			{
				mTotalDuration += frameData[i].Item2;
				mFrames.Add(new AnimationFrame(MonoData.I.MonoGameLoad<Texture2D>(frameData[i].Item1), frameData[i].Item2));
			}
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
		/// Begin playing
		/// </summary>
		public void Play()
		{
			mPlaying = true;
			mPlayHead = 0.0f;
		}



		/// <summary>
		/// Begin playing at a percent marker
		/// </summary>
		public void Play(float percentPlayed)
		{
			mPlaying = true;
			mPlayHead = percentPlayed * mTotalDuration;
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

			i = Math.Min(i, mFrames.Count - 1);

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
