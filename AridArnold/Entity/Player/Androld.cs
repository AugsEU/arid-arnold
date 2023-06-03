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
		public Androld(Vector2 pos) : base(pos)
		{
			//Immediately complete this timer.
			mTimerSinceStart.SetComplete();
			mPrevDirection = WalkDirection.Right;
		}



		/// <summary>
		/// Load textures and assets
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Arnold/AndroldStand");
			mJumpUpTex = MonoData.I.MonoGameLoad<Texture2D>("Arnold/AndroldJumpUp");
			mJumpDownTex = MonoData.I.MonoGameLoad<Texture2D>("Arnold/AndroldJumpDown");

			mRunningAnimation = new Animator(Animator.PlayType.Repeat,
												("Arnold/AndroldRun1", 0.1f),
												("Arnold/AndroldRun2", 0.1f),
												("Arnold/AndroldRun3", 0.1f),
												("Arnold/AndroldRun4", 0.15f));

			mRunningAnimation.Play();

			//Botch position a bit. Not sure what's happening here.
			mPosition.Y -= 2.0f;
		}

		#endregion rInitialisation





		#region rItem

		/// <summary>
		/// Can we use an item right now?
		/// </summary>
		protected override bool CanUseItem()
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
