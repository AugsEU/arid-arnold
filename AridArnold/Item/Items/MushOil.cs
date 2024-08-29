namespace AridArnold
{
	internal class MushOil : OnceItem
	{
		public MushOil(int price) : base("Items.MushOilTitle", "Items.MushOilDesc", price)
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/MushOil/Bottle");
		}

		protected override void DoEffect(Arnold arnold)
		{
			arnold.SetBouncyMode();
		}
	}
}
