
namespace AridArnold
{
	/// <summary>
	/// Panel in GalaxView
	/// </summary>
	abstract class UIPanelBase : LayElement
	{
		protected const double FLASH_TIME = 3000.0;
		public static Color PANEL_WHITE = new Color(127, 127, 127);
		public static Color PANEL_GOLD = new Color(255, 182, 0);

		Texture2D mPanelBG;
		MonoTimer mUnlockFlashTimer;
		bool mPrevIsUnlocked;

		protected UIPanelBase(XmlNode rootNode, string bgPath, Layout parent) : base(rootNode, parent)
		{
			mPanelBG = MonoData.I.MonoGameLoad<Texture2D>(bgPath);
			mUnlockFlashTimer = new MonoTimer();
			mPrevIsUnlocked = IsUnlocked();
		}

		public override void Update(GameTime gameTime)
		{
			bool isUnlocked = IsUnlocked();
			if(!mPrevIsUnlocked && isUnlocked)
			{
				mUnlockFlashTimer.FullReset();
				mUnlockFlashTimer.Start();
			}
			mPrevIsUnlocked = isUnlocked;
			base.Update(gameTime);
		}

		protected virtual bool IsUnlocked()
		{
			return true;
		}

		protected bool ShouldDraw()
		{
			if(!mPrevIsUnlocked)
			{
				return false;
			}

			if(mUnlockFlashTimer.IsPlaying() && mUnlockFlashTimer.GetElapsedMs() <= FLASH_TIME)
			{
				const double INTERVAL = 500.0f;
				return mUnlockFlashTimer.GetElapsedMs() % INTERVAL > INTERVAL * 0.5f;
			}

			return true;
		}

		public override void Draw(DrawInfo info)
		{
			if (!ShouldDraw()) return;
			MonoDraw.DrawTextureDepth(info, mPanelBG, GetPosition(), GetDepth());
			base.Draw(info);
		}
	}
}
