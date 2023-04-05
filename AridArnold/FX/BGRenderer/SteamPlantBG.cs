namespace AridArnold
{
	class SteamPlantBG : ImageBGRenderer
	{
		const string ANIM_PATH = "BG/SteamPlant/BG1";


		public SteamPlantBG() : base(ANIM_PATH)
		{
			mElements.Add(new SteamValve(new Vector2(284.0f, 226.0f)));
			mElements.Add(new SteamValve(new Vector2(288.0f, 88.0f)));
			mElements.Add(new SteamValve(new Vector2(418.0f, 88.0f)));
			mElements.Add(new BoilerDial(new Vector2(200.0f, 394.0f)));
		}
	}




	/// <summary>
	/// Valve that turns sometimes
	/// </summary>
	class SteamValve : BGElement
	{
		IdleAnimator mIdleAnimation;

		public SteamValve(Vector2 pos) : base(pos)
		{
			Animator WaitAnim = MonoData.I.LoadAnimator("BG/SteamPlant/Valve1");
			Animator CWAnim = MonoData.I.LoadAnimator("BG/SteamPlant/ValveCW.max");
			Animator CCWAnim = MonoData.I.LoadAnimator("BG/SteamPlant/ValveCCW.max");

			mIdleAnimation = new IdleAnimator(WaitAnim, 15.0f, CWAnim, CCWAnim);
		}

		public override void Update(GameTime gameTime)
		{
			mIdleAnimation.Update(gameTime);
		}

		protected override Texture2D GetDrawTexture()
		{
			return mIdleAnimation.GetCurrentTexture();
		}
	}



	/// <summary>
	/// Dial that fluctuates
	/// </summary>
	class BoilerDial : BGElement
	{
		IdleAnimator mIdleAnimation;

		public BoilerDial(Vector2 pos) : base(pos)
		{
			mIdleAnimation = MonoData.I.LoadIdleAnimator("BG/SteamPlant/DialIdle.mia");
		}

		public override void Update(GameTime gameTime)
		{
			mIdleAnimation.Update(gameTime);
		}

		protected override Texture2D GetDrawTexture()
		{
			return mIdleAnimation.GetCurrentTexture();
		}
	}
}
