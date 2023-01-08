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
		/// <param name="content">Monogame content manager</param>
		public override void LoadContent(ContentManager content)
		{
			mTexture = content.Load<Texture2D>("Arnold/androld-stand");
			mJumpUpTex = content.Load<Texture2D>("Arnold/androld-jump-up");
			mJumpDownTex = content.Load<Texture2D>("Arnold/androld-jump-down");

			mRunningAnimation = new Animator(content, Animator.PlayType.Repeat,
												("Arnold/androld-run0", 0.1f),
												("Arnold/androld-run1", 0.1f),
												("Arnold/androld-run2", 0.1f),
												("Arnold/androld-run3", 0.15f));

			mRunningAnimation.Play();

			//Botch position a bit. Not sure what's happening here.
			mPosition.Y -= 2.0f;
		}

		#endregion rInitialisation
	}
}
