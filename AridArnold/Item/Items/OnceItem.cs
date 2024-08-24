
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

		public override void ActOnArnold(GameTime gameTime, Arnold arnold)
		{
			if(!mDoneEffect)
			{
				DoEffect();
				mDoneEffect = true;
				EndItem();
			}
			base.ActOnArnold(gameTime, arnold);
		}

		protected abstract void DoEffect();
	}
}
