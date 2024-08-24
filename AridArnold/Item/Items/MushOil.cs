namespace AridArnold
{
	internal class MushOil : OnceItem
	{
		public MushOil() : base("Items.MushOilTitle", "Items.MushOilDesc")
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/MushOil/Bottle");
		}

		public override int GetPrice()
		{
			return 1;
		}

		protected override void DoEffect(Arnold arnold)
		{
			arnold.SetBouncyMode();
		}
	}
}
