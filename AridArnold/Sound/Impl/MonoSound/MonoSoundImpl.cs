namespace AridArnold
{
	/// <summary>
	/// Implement sound interface using monogame.
	/// </summary>
	class MonoSoundImpl : SoundImplementation
	{
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
	}
}
