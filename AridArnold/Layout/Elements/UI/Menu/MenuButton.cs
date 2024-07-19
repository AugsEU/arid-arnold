
namespace AridArnold
{
	abstract class MenuButton : ActionableNavElement
	{
		protected enum HoverState
		{
			kDefault,
			kHover,
			kPress,
			NumHoverStates
		}

		Texture2D mTexture;

		PercentageTimer mClickTimer;
		SpriteFont mFont;
		string mDisplayText;
		Color mDefaultColor;
		Color mHoverColor;

		public MenuButton(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			mClickTimer = new PercentageTimer(300.0);

			mTexture = MonoParse.GetTexture(rootNode["texture"], null);

			mFont = MonoParse.GetFont(rootNode["font"], "Pixica-36");
			string textID = MonoParse.GetString(rootNode["text"]);
			mDisplayText = LanguageManager.I.GetText(textID);

			mDefaultColor = MonoParse.GetColor(rootNode["textColor"], Color.Gray);

			// Creat hover color by brightening the default, or by loading from XML
			Color hoverDefault = mDefaultColor;
			MonoColor.BrightenColour(ref hoverDefault, 1.0f);
			mHoverColor = MonoParse.GetColor(rootNode["textHoverColor"], hoverDefault);

			// Set size if not already set
			if(mTexture is not null)
			{
				// By texture if we have one
				Vector2 texSize = CalcTextureSize();
				if (mSize.X == 0.0f) mSize.X = mTexture.Width;
				if (mSize.Y == 0.0f) mSize.Y = mTexture.Height / (int)HoverState.NumHoverStates; // Texture is in 3 parts
			}
			else
			{
				// By font if we have no texture
				mSize = mFont.MeasureString(mDisplayText);
			}
		}

		Vector2 CalcTextureSize()
		{
			if(mTexture is null)
			{
				return Vector2.Zero;
			}

			// Get size of displayed texture area
			return new Vector2(mTexture.Width, mTexture.Height / (int)HoverState.NumHoverStates);
		}

		public override void Update(GameTime gameTime)
		{
			Rect2f rect = new Rect2f(GetPosition(), mSize.X, mSize.Y);

			mClickTimer.Update(gameTime);

			if (mClickTimer.IsPlaying())
			{
				// Block selection in press anim
				GetParent().SetSelectionBlocker(true);

				if (mClickTimer.GetPercentageF() >= 1.0f)
				{
					DoAction();
					mClickTimer.FullReset();

					// Unblock
					GetParent().SetSelectionBlocker(false);
				}
			}
			else if (IsSelected())
			{
				bool enterKey = InputManager.I.AnyGangPressed(BindingGang.SysConfirm);
				bool mouseClick = InputManager.I.KeyPressed(InputAction.SysLClick) && InputManager.I.MouseInRect(rect);

				if (enterKey || mouseClick)
				{
					mClickTimer.FullReset();
					mClickTimer.Start();
				}
			}

			base.Update(gameTime);
		}

		public override void Draw(DrawInfo info)
		{
			Vector2 centrePos = GetPosition() + mSize * 0.5f;

			HoverState hoverState = GetHoverState();

			if(hoverState == HoverState.kPress)
			{
				centrePos.Y += 2.0f;
			}

			Color textColor = GetTextColor();
			MonoDraw.DrawStringCentred(info, mFont, centrePos, textColor, mDisplayText, GetDepth());

			if (mTexture is not null)
			{
				Vector2 textureSize = CalcTextureSize();
				Vector2 texPosition = centrePos - textureSize * 0.5f;

				int srcRectIdx = (int)GetHoverState();
				Rectangle srcRect = new Rectangle(0, (int)(textureSize.Y * srcRectIdx), (int)textureSize.X, (int)textureSize.Y);

				MonoDraw.DrawTexture(info, mTexture, texPosition, srcRect, GetColor(), 0.0f, Vector2.Zero, GetScale(), SpriteEffects.None, GetDepth());
			}

			base.Draw(info);
		}

		Color GetTextColor()
		{
			HoverState hoverState = GetHoverState();
			switch (hoverState)
			{
				case HoverState.kDefault:
					return mDefaultColor;
				case HoverState.kHover:
					return mHoverColor;
				case HoverState.kPress:
					Color newColor = mHoverColor * 0.2f;
					newColor.A = mHoverColor.A;
					return newColor;
			}

			throw new NotImplementedException();
		}

		protected HoverState GetHoverState()
		{
			if (mClickTimer.IsPlaying())
			{
				return HoverState.kPress;
			}

			if (IsSelected())
			{
				return HoverState.kHover;
			}

			return HoverState.kDefault;
		}
	}
}
