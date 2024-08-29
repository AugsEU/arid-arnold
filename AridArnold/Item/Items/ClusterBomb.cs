namespace AridArnold
{
	internal class ClusterBomb : OnceItem
	{
		static Vector2[] BOMB_DIR =
		{
			new Vector2(0.0f, -1.0f),
			new Vector2(-0.71f, -0.71f),
			new Vector2(0.71f, -0.71f),
			new Vector2(-0.42f, -0.9f),
			new Vector2(0.42f, -0.9f),
		};

		const float BOMB_SPEED = 16.0f;

		public ClusterBomb(int price) : base("Items.ClusterBombTitle", "Items.ClusterBombDesc", price)
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/ClusterBomb/Bomb");
		}

		protected override void DoEffect(Arnold arnold)
		{
			for(int i = 0; i < BOMB_DIR.Length; i++)
			{
				Vector2 vel = BOMB_DIR[i] * BOMB_SPEED;

				LaserBomb testBomb = new LaserBomb(arnold, arnold.GetCentrePos(), vel);

				testBomb.SetGravity(arnold.GetGravityDir());
				EntityManager.I.QueueRegisterEntity(testBomb);
			}
		}
	}
}
