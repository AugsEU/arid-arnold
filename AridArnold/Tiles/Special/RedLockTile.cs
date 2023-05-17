using AridArnold.Tiles.Basic;

namespace AridArnold
{
    internal class RedLockTile : SquareTile
	{
		public RedLockTile(Vector2 position) : base(position)
		{
			EventManager.I.AddListener(EventType.RedKeyUsed, KeyUsedCallback);
		}

		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/RedLock");
		}

		void KeyUsedCallback(EArgs args)
		{
			mEnabled = false;
		}
	}
}
