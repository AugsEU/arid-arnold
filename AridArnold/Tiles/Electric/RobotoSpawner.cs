namespace AridArnold
{
	internal class RobotoSpawner : SquareTile
	{
		public RobotoSpawner(Vector2 position) : base(position)
		{
		}

		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/")
			base.LoadContent();
		}
	}
}
