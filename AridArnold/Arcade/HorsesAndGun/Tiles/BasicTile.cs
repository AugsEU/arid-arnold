namespace HorsesAndGun.Tiles
{
	internal class BasicTile : TrackTile
	{
		Texture2D mTexture;

		public BasicTile(ContentManager content, string texName = "Arcade/HorsesAndGun/Tiles/basic_tile")
		{
			mTexture = content.Load<Texture2D>(texName);
		}

		public override Texture2D Draw(DrawInfo info)
		{
			return mTexture;
		}

		public override void ApplyEffect(Horse horse, TrackManager trackManager)
		{

		}
	}
}
