namespace AridArnold
{
	/// <summary>
	/// Re-skin of Arnold class. Functionally the same.
	/// </summary>
	internal class Androld : Arnold
	{
		#region rInitialisation

		/// <summary>
		/// Construct Androld from position
		/// </summary>
		/// <param name="pos">Starting position</param>
		public Androld(Vector2 pos) : base(pos, false)
		{
			//Immediately complete this timer.
			mTimerSinceStart.SetComplete();
			mPrevDirection = WalkDirection.Right;
		}

		public override void LoadContent()
		{
			base.LoadContent();

			// Turn off jump sound.
			LoadSFX(null, AridArnoldSFX.ArnoldWalk, 0.2f);
		}

		protected override void InitTexturePacks()
		{
			mYoungTexturePack = new MonoTexturePack("Arnold/Androld.mtp");
			mOldTexturePack = new MonoTexturePack("Arnold/Androld.mtp");
		}

		#endregion rInitialisation





		#region rItem

		/// <summary>
		/// Can we use an item right now?
		/// </summary>
		public override bool CanUseItem()
		{
			// Androld can't use an item
			return false;
		}


		/// <summary>
		/// Can we buy an item?
		/// </summary>
		public override bool CanBuyItem()
		{
			// Androld can't buy an item
			return false;
		}

		#endregion rItem
	}
}
