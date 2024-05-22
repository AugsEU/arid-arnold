namespace GMTK2023
{
	/// <summary>
	/// Does idle animation with simple states.
	/// </summary>
	internal class IdleAnimator
	{
		#region rMembers

		Animator mWaitAnim;
		List<Animator> mVariationAnims;
		int mCurrentVariation;
		float mVariationChance;
		MonoRandom mRandom;

		#endregion rMembers


		#region rInitialisation

		/// <summary>
		/// Declare an idle animation with a main animator
		/// </summary>
		public IdleAnimator(Animator waitAnim, float variationChance, params Animator[] variationAnimators)
		{
			mWaitAnim = waitAnim;
			mWaitAnim.SetType(Animator.PlayType.OneShot);
			mWaitAnim.Play();

			mVariationChance = variationChance;
			mVariationAnims = new List<Animator>();
			mCurrentVariation = -1;

			mRandom = new MonoRandom();

			foreach (Animator animator in variationAnimators)
			{
				animator.SetType(Animator.PlayType.OneShot);
				mVariationAnims.Add(animator);
			}
		}

		#endregion rInitialisation


		#region rUpdate

		/// <summary>
		/// Update idle animation.(Required for playing)
		/// </summary>
		public void Update(GameTime gameTime)
		{
			// If we are playing the default idle animation.
			if (mCurrentVariation == -1)
			{
				//Update it.
				mWaitAnim.Update(gameTime);

				// Animation has played out fully.
				if (mWaitAnim.IsPlaying() == false)
				{
					DecideNextAnimation();
				}
			}
			else
			{
				mVariationAnims[mCurrentVariation].Update(gameTime);

				// Animation has played out fully.
				if (mVariationAnims[mCurrentVariation].IsPlaying() == false)
				{
					// Go back to waiting.
					mCurrentVariation = -1;
					mWaitAnim.Play();
				}
			}
		}



		/// <summary>
		/// Decide the next animation.
		/// </summary>
		void DecideNextAnimation()
		{
			if (mVariationAnims.Count > 0 && mRandom.PercentChance(mVariationChance))
			{
				// Variation should play. Select random index and play it.
				mCurrentVariation = mRandom.GetIntRange(0, mVariationAnims.Count - 1);
				mVariationAnims[mCurrentVariation].Play();
			}
			else
			{
				// Else play normal animation
				mWaitAnim.Play();
			}
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get texture, can include variation animations.
		/// </summary>
		public Texture2D GetCurrentTexture()
		{
			if (mCurrentVariation != -1)
			{
				return mVariationAnims[mCurrentVariation].GetCurrentTexture();
			}

			return mWaitAnim.GetCurrentTexture();
		}



		/// <summary>
		/// Get texture of main idle animation.
		/// </summary>
		public Texture2D GetIdleTexture()
		{
			if (mWaitAnim.IsPlaying() == false)
			{
				mWaitAnim.Play();
			}

			return mWaitAnim.GetCurrentTexture();
		}
		#endregion rDraw
	}
}
