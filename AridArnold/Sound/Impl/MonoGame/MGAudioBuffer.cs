namespace AridArnold
{
	struct SoundEffectCacheInfo
	{
		public SoundEffect mSoundEffect;
		public int mNumInstances;

		public SoundEffectCacheInfo(SoundEffect soundEffect)
		{
			mSoundEffect = soundEffect;
			mNumInstances = 1;
		}
	}

	/// <summary>
	/// MonoGame's version of a sound buffer. Use mainly for sound effects
	/// </summary>
	class MGAudioBuffer : AudioBuffer
	{
		#region rConstants

		// Soft limit on how many seconds of sound effects to aim to store in the cache.
		const int CACHE_RUNTIME_LIMIT = 10 * 60;

		#endregion rConstants





		#region rMembers

		// Think about this for memory usage...
		static Dictionary<string, SoundEffectCacheInfo> sSoundEffectCache = new Dictionary<string, SoundEffectCacheInfo>();

		string mPath;
		SoundEffectInstance mSoundEffect;

		AudioListener[] mListenerInfos = new AudioListener[] { new AudioListener() };
		AudioEmitter mEmitterInfo = new AudioEmitter();

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create streamed buffer
		/// </summary>
		public MGAudioBuffer(string path)
		{
			mPath = path;

			lock (sSoundEffectCache)
			{
				SoundEffectCacheInfo effectInfo;
				if (sSoundEffectCache.TryGetValue(path, out effectInfo))
				{
					effectInfo.mNumInstances += 1;
				}
				else
				{
					effectInfo.mSoundEffect = MonoData.I.MonoGameLoad<SoundEffect>(path);
					effectInfo.mNumInstances = 1;
				}

				sSoundEffectCache[path] = effectInfo;

				mSoundEffect = effectInfo.mSoundEffect.CreateInstance();
			}

		}



		/// <summary>
		/// Called when deleted.
		/// </summary>
		~MGAudioBuffer()
		{
			lock (sSoundEffectCache)
			{
				SoundEffectCacheInfo effectInfo = sSoundEffectCache[mPath];
				effectInfo.mNumInstances -= 1;
				sSoundEffectCache[mPath] = effectInfo;

				if(effectInfo.mNumInstances == 0)
				{
					CheckClearCache();
				}
			}
		}



		/// <summary>
		/// Check to find elements to clear.
		/// </summary>
		static void CheckClearCache()
		{
			TimeSpan totalRunTime = TimeSpan.Zero;
			foreach(var infoKV in sSoundEffectCache)
			{
				totalRunTime += infoKV.Value.mSoundEffect.Duration;
			}

			if((int)totalRunTime.TotalSeconds > CACHE_RUNTIME_LIMIT)
			{
				// Find a single element to delete.
				foreach (var infoKV in sSoundEffectCache)
				{
					if(infoKV.Value.mNumInstances == 0)
					{
						sSoundEffectCache.Remove(infoKV.Key);
						break;
					}
				}
			}
		}

		#endregion rInit





		#region rPlay

		/// <summary>
		/// Play the stream.
		/// </summary>
		public override void Play()
		{
			mSoundEffect.Play();
			Refresh3DParams();
		}

		/// <summary>
		/// Resume playing
		/// </summary>
		public override void Resume()
		{
			mSoundEffect.Resume();
		}

		/// <summary>
		/// Pause playing.
		/// </summary>
		public override void Pause()
		{
			mSoundEffect.Pause();
		}

		/// <summary>
		/// Stop playing completely.
		/// </summary>
		public override void Stop()
		{
			mSoundEffect.Stop();
		}

		#endregion rPlay





		#region rUtil

		public override SoundState GetState()
		{
			return mSoundEffect.State;
		}

		public override bool GetLoop()
		{
			return mSoundEffect.IsLooped;
		}

		public override void SetLoop(bool loop)
		{
			mSoundEffect.IsLooped = loop;
		}

		public override float GetPan()
		{
			return mSoundEffect.Pan;
		}

		public override void SetPan(float pan)
		{
			mSoundEffect.Pan = pan;
		}

		public override float GetPitch()
		{
			return mSoundEffect.Pitch;
		}

		public override void SetPitch(float pitch)
		{
			mSoundEffect.Pitch = pitch;
		}

		public override float GetVolume()
		{
			return mSoundEffect.Volume;
		}

		public override void SetVolume(float volume)
		{
			mSoundEffect.Volume = volume;
		}

		public override void SetEmitter(AudioPositionInfo info)
		{
			mEmitterInfo.Position = info.mPosition;
			mEmitterInfo.Up = info.mUp;
			mEmitterInfo.Forward = info.mForward;
			mEmitterInfo.Velocity = info.mVelocity;

			Refresh3DParams();
		}

		public override void SetListeners(params AudioPositionInfo[] infos)
		{
			mListenerInfos = new AudioListener[infos.Length];

			for(int i = 0; i < infos.Length; i++)
			{
				mListenerInfos[i] = new AudioListener();
				mListenerInfos[i].Position = infos[i].mPosition;
				mListenerInfos[i].Up = infos[i].mUp;
				mListenerInfos[i].Forward = infos[i].mForward;
				mListenerInfos[i].Velocity = infos[i].mVelocity;
			}

			Refresh3DParams();
		}

		private void Refresh3DParams()
		{
			mSoundEffect.Apply3D(mListenerInfos, mEmitterInfo);
		}

		#endregion rUtil
	}
}
