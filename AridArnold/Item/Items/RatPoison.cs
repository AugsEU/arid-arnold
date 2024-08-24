namespace AridArnold
{
	internal class RatPoison : OnceItem
	{
		public static Color[] POISON_COLORS = new Color[]
		{
			new Color(255, 82, 25),
			new Color(216, 10, 24),
			new Color(200, 90, 70)
		};

		public RatPoison() : base("Items.RatPoisonTitle", "Items.RatPoisonDesc")
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/RatPoison/Box");
		}

		public override int GetPrice()
		{
			return 3;
		}

		protected override void DoEffect()
		{
			// Kill all rats
			int numEnt = EntityManager.I.GetEntityNum();
			for(int i = 0; i < numEnt; i++)
			{
				Entity ent = EntityManager.I.GetEntity(i);
				if(ent is Trundle rat)
				{
					Rect2f ratBounds = ent.ColliderBounds();
					Vector2 start = new Vector2(ratBounds.min.X, ratBounds.max.Y);
					Vector2 end = ratBounds.max;

					DustUtil.EmitSwooshLine(start, end, new Vector2(0.0f, -10.0f), POISON_COLORS);
					rat.Kill();
				}
			}
		}
	}
}
