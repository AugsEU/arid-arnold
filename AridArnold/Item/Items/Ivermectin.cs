namespace AridArnold
{
	internal class Ivermectin : OnceItem
	{
		public Ivermectin(int price) : base("Items.IvermectinTitle", "Items.IvermectinDesc", price)
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/Ivermectin/Box");
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
