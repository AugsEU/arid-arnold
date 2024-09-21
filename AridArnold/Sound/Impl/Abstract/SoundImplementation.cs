using Microsoft.Xna.Framework.Graphics;

namespace AridArnold
{
	/// <summary>
	/// Represents the interface with the underlying system.
	/// </summary>
	abstract class SoundImplementation
	{
		public abstract void Init(Game game);
		public abstract void OnExit(Game game);

		public AudioBuffer LoadAudioBuffer<T>(T aridArnoldSFX) where T : Enum
		{
			string filePath = MonoEnum.GetFilePath(aridArnoldSFX);
			return MonoSound.Impl.LoadAudioBuffer(filePath);
		}

		public abstract void Update(GameTime gameTime);

		abstract public AudioBuffer LoadAudioBuffer(string path);
	}

	static class MonoSound
	{
		// Swap this out to change implementation.
		static SoundImplementation mImpl = new MonoSoundImpl();

		public static SoundImplementation Impl { get { return mImpl; } }
	}
}
