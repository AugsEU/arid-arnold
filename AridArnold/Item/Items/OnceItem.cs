
namespace AridArnold
{
	/// <summary>
	/// Item that does effect exactly once.
	/// </summary>
	abstract class OnceItem : Item
	{
		bool mDoneEffect;

		protected OnceItem(string titleID, string descID) : base(titleID, descID)
		{
		}

		public override void Begin()
		{
			mDoneEffect = false;
			base.Begin();
		}

		public override void ActiveUpdate(GameTime gameTime, Arnold arnoldUsingItem)
		{
			if(!mDoneEffect)
			{
				DoEffect();
				mDoneEffect = true;
				EndItem();
			}
			base.ActiveUpdate(gameTime, arnoldUsingItem);
		}

		protected abstract void DoEffect();
	}
}
