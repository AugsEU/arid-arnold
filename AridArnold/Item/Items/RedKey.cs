namespace AridArnold
{
	internal class RedKey : Item
	{
		public RedKey()
		{
		}

		public override Animator GenerateAnimator()
		{
			return MonoData.I.LoadAnimator("Items/RedKey/Key");
		}

		public override int GetPrice()
		{
			return 3;
		}

		public override void UseItem()
		{
			throw new NotImplementedException();
		}
	}
}
