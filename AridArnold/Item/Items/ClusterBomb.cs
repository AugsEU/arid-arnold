namespace AridArnold
{
	internal class ClusterBomb : OnceItem
	{
		static Vector2[] BOMB_DIR =
		{
			new Vector2(-0.96f, -0.28f),
			new Vector2(-0.84f, -0.54f),
			new Vector2(-0.65f, -0.76f),
			new Vector2(-0.41f, -0.91f),
			new Vector2(-0.14f, -0.98f),

			new Vector2(0.14f, -0.98f),
			new Vector2(0.41f, -0.91f),
			new Vector2(0.65f, -0.76f),
			new Vector2(0.84f, -0.54f),
			new Vector2(0.96f, -0.28f),
		};

		const float BOMB_SPEED = 22.0f;

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
