namespace AridArnold
{
	abstract class ParticleEmitter
	{
		protected Vector2 mSource;

		public ParticleEmitter(Vector2 source)
		{
			mSource = source;
		}

		public abstract void Update(GameTime gameTime);
	}
}
