namespace AridArnold
{
	internal class Ivermectin : OnceItem
	{
		public Ivermectin() : base("Items.IvermectinTitle", "Items.IvermectinDesc")
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/Ivermectin/Box");
		}

		public override int GetPrice()
		{
			return 3;
		}

		public override bool CanUseItem(Arnold arnold)
		{
			if(arnold.IsHorseMode())
			{
				return false;
			}
			return base.CanUseItem(arnold);
		}

		protected override void DoEffect(Arnold arnold)
		{
			arnold.SetHorseMode();
		}
	}
}
