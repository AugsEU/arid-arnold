#if MONOSOUND_BUILD
using MonoSound;

namespace AridArnold
{
	/// <summary>
	/// Implement sound interface using monogame.
	/// </summary>
	class MonoSoundImpl : SoundImplementation
	{
		const float DEFAULT_DISTANCE_SCALE = 50.0f;
		const float DEFAULT_SPEED_OF_SOUND = 365.0f;
		const float DEFAULT_DOPPLE_SCALE = 0.2f;

		public override void Init(Game game)
		{
			MonoSoundLibrary.Init(game);

			SoundEffect.DistanceScale = DEFAULT_DISTANCE_SCALE;
			SoundEffect.SpeedOfSound = DEFAULT_SPEED_OF_SOUND;
			SoundEffect.DopplerScale = DEFAULT_DOPPLE_SCALE;
			SoundEffect.MasterVolume = 1.0f;
			SoundEffect.Initialize();
		}

		public override void OnExit(Game game)
		{
			MonoSoundLibrary.DeInit();
		}

		public override AudioBuffer LoadAudioBuffer(string path)
		{
			string extension = Path.GetExtension(path);

			// Naive.
			switch (extension)
			{
				case ".ogg":
					return new MSStreamBuffer(path);
				case "":
					return new MGAudioBuffer(path);
			}

			throw new NotImplementedException();
		}

		public override void Update(GameTime gameTime)
		{
		}
	}
}
#endif // MONOSOUND_BUILD