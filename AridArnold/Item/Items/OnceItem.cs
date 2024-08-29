
namespace AridArnold
{
	/// <summary>
	/// Item that does effect exactly once.
	/// </summary>
	abstract class OnceItem : Item
	{
		bool mDoneEffect;

		protected OnceItem(string titleID, string descID, int price) : base(titleID, descID, price)
		{
		}

		public override void Begin()
		{
			mDoneEffect = false;
			base.Begin();
		}

		public override void ActOnArnold(GameTime gameTime, Arnold arnold)
		{
			if(!mDoneEffect)
			{
				DoEffect(arnold);
				mDoneEffect = true;
				EndItem();
			}
			base.ActOnArnold(gameTime, arnold);
		}

		protected abstract void DoEffect(Arnold arnold);
	}
}
