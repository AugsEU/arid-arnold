namespace GMTK2023
{
	/// <summary>
	/// Represents an effect that plays on screen.
	/// </summary>
	abstract class FX
	{
		public abstract void Update(GameTime gameTime);

		public abstract void Draw(DrawInfo info);

		public abstract bool Finished();
	}
}
