namespace HorsesAndGun
{
	internal abstract class TrackTile
	{
		public abstract Texture2D Draw(DrawInfo info);

		public virtual void Update(GameTime gameTime) { }

		public abstract void ApplyEffect(Horse horse, TrackManager trackManager);
	}
}
