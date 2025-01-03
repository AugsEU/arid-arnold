

namespace AridArnold
{
	/// <summary>
	/// Button to rebind an input.
	/// </summary>
	internal class EInputRebind : MenuButton
	{
		const double BUTTON_TIMEOUT = 3000.0;
		const double UNLOCK_TIME = 100.0;

		string mBaseText;
		InputAction mInputAction;
		bool mInRebindMode;
		MonoTimer mTimeoutTimer;
		PercentageTimer mUnlockSelectTimer;
		bool mIsClashing = false;

		Color mClashColor = Color.OrangeRed;
		Color mOKColor = Color.White;

		public EInputRebind(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			mInputAction = MonoParse.GetEnum<InputAction>(rootNode["bind"]);
			mBaseText = mDisplayText;
			FormatBaseText();
			mTimeoutTimer = new MonoTimer();
			mUnlockSelectTimer = new PercentageTimer(UNLOCK_TIME);

			mOKColor = mDefaultColor;

			mIsClashing = false;
		}

		private void FormatBaseText()
		{
			string inputBindStr = mInRebindMode ? "..." : InputManager.I.GetInputBindSet(mInputAction).ToString();
			string newDisplayText = string.Format("{0} - {1}", mBaseText, inputBindStr);

			if (newDisplayText != mDisplayText)
			{
				mDisplayText = string.Format("{0} - {1}", mBaseText, inputBindStr);
				RecalcSize();
			}
		}

		public override void Update(GameTime gameTime)
		{
			mTimeoutTimer.Update(gameTime);
			mUnlockSelectTimer.Update(gameTime);

			if (mInRebindMode)
			{
				bool success = InputManager.I.AttemptRebind(mInputAction);
				if(success || mTimeoutTimer.GetElapsedMs() > BUTTON_TIMEOUT)
				{
					mTimeoutTimer.FullReset();
					mInRebindMode = false;
					mUnlockSelectTimer.Start();
				}
				GetParent().ForceSetSelectedElement(this);
				GetParent().SetSelectionBlocker(true);
			}

			if(mUnlockSelectTimer.IsPlaying())
			{
				GetParent().ForceSetSelectedElement(this);
				GetParent().SetSelectionBlocker(true);
				if (mUnlockSelectTimer.GetPercentageF() >= 1.0f)
				{
					GetParent().SetSelectionBlocker(false);
					mUnlockSelectTimer.FullReset();
				}
			}

			FormatBaseText();

			bool isClashing = InputManager.I.IsInputActionClashing(mInputAction);
			mDefaultColor = isClashing ? mClashColor : mOKColor;

			base.Update(gameTime);
		}

		public override void DoAction()
		{
			if (mUnlockSelectTimer.IsPlaying())
			{
				return;
			}

			mTimeoutTimer.Start();
			mInRebindMode = true;
			GetParent().SetSelectedElement(this);
			GetParent().SetSelectionBlocker(true);
		}


		protected override bool AllowClick()
		{
			if(mUnlockSelectTimer.IsPlaying())
			{
				return false;
			}
			return base.AllowClick();
		}
	}
}
