using Microsoft.Xna.Framework;

namespace AridArnold
{
	class SFXManager : Singleton<SFXManager>
	{
		#region rMembers

		List<AudioPositionInfo> mCurrListeners;
		List<AudioPositionInfo> mNewListeners;
		List<GameSFX> mSoundEffects;

		#endregion rMembers




		#region rInit

		/// <summary>
		/// Initialise sound effect manager.
		/// </summary>
		public void Init()
		{
			mCurrListeners = new List<AudioPositionInfo>();
			mNewListeners = new List<AudioPositionInfo>();
			mSoundEffects = new List<GameSFX>();
		}

		#endregion rInit





		#region rPlay

		/// <summary>
		/// Add a sound effect, play it immediately.
		/// </summary>
		public void PlaySFX(GameSFX gameSFX, float fadeInTime = 0.0f)
		{
			if(gameSFX is null)
			{
				return;
			}

			mSoundEffects.Add(gameSFX);

			// Update the listeners immediately.
			gameSFX.UpdateListeners(mCurrListeners);
			gameSFX.Begin(fadeInTime);
		}



		/// <summary>
		/// Add a basic sound effect to be played immediately
		/// </summary>
		public void PlaySFX(AridArnoldSFX effect, float maxVol, float minPitch = 0.0f, float maxPitch = 0.0f, float fadeIn = 0.0f)
		{
			PlaySFX(new GameSFX(effect, maxVol, minPitch, maxPitch), fadeIn);
		}



		/// <summary>
		/// Add a listener
		/// </summary>
		public void AddListener(MovingEntity listener)
		{
			AudioPositionInfo entityListenerInfo = new AudioPositionInfo(listener.GetPos(), listener.GetVelocity());

			entityListenerInfo.mForward = new Vector3(0.0f, 0.0f, 1.0f);
			entityListenerInfo.mPosition.Z = 20.0f;

			mNewListeners.Add(entityListenerInfo);
		}



		/// <summary>
		/// Update sound effects.
		/// </summary>
		public void Update(GameTime gameTime)
		{
			if(mCurrListeners.Count == 0)
			{
				AudioPositionInfo newInfo = new AudioPositionInfo();
				newInfo.mPosition = new Vector3(280.0f, 280.0f, 0.0f);
				mCurrListeners.Add(newInfo);
			}

			foreach(GameSFX gameSFX in mSoundEffects)
			{
				gameSFX.UpdateListeners(mCurrListeners);
				gameSFX.Update(gameTime);
			}

			// Clear finished sound effects.
			mSoundEffects.RemoveAll(s => s.IsFinished());

			// Update listeners new one has anything in it. Otherwise keep the previous frame's.
			if(mNewListeners.Count > 0)
			{
				mCurrListeners = new List<AudioPositionInfo>(mNewListeners);
			}

			// Listeners are cleared every frame.
			mNewListeners.Clear();
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
