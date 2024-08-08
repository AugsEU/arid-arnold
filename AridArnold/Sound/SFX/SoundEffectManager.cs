using Microsoft.Xna.Framework;

namespace AridArnold
{
	class SoundEffectManager : Singleton<SoundEffectManager>
	{
		#region rMembers

		List<Vector2> mListeners;
		List<GameSFX> mSoundEffects;

		#endregion rMembers




		#region rInit

		/// <summary>
		/// Initialise sound effect manager.
		/// </summary>
		public void Init()
		{
			mListeners = new List<Vector2>();
			mSoundEffects = new List<GameSFX>();
		}

		#endregion rInit



		#region rPlay

		/// <summary>
		/// Add a sound effect, play it immediately.
		/// </summary>
		public void AddSoundEffect(GameSFX gameSFX, float fadeInTime = 0.0f)
		{
			mSoundEffects.Add(gameSFX);
			gameSFX.Begin(fadeInTime);
		}



		/// <summary>
		/// Add a listener
		/// </summary>
		public void AddListener(Vector2 listener)
		{
			mListeners.Add(listener);
		}



		/// <summary>
		/// Update sound effects.
		/// </summary>
		public void Update(GameTime gameTime)
		{
			foreach(GameSFX gameSFX in mSoundEffects)
			{
				gameSFX.UpdateListeners(mListeners);
				gameSFX.Update(gameTime);
			}

			// Clear finished sound effects.
			mSoundEffects.RemoveAll(s => s.IsFinished());

			// Listeners are cleared every frame.
			mListeners.Clear();
		}



		/// <summary>
		/// End all sound effects
		/// </summary>
		public void EndAllSFX(float fadeOutTime)
		{
			foreach (GameSFX gameSFX in mSoundEffects)
			{
				gameSFX.Stop(fadeOutTime);
			}
		}

		#endregion rPlay
	}
}
