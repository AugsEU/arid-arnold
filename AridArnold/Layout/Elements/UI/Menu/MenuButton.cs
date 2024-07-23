
namespace AridArnold
{
	abstract class MenuButton : HitBoxNavElement
	{
		#region rType

		protected enum HoverState
		{
			kDefault,
			kHover,
			kPress,
			NumHoverStates
		}

		#endregion rType





		#region rMembers

		protected Texture2D mTexture;

		PercentageTimer mClickTimer;
		SpriteFont mFont;
		string mDisplayText;
		Color mDefaultColor;
		Color mHoverColor;

		#endregion rMembers


		#region rInit

		/// <summary>
		/// Create a menu button
		/// </summary>
		public MenuButton(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			mClickTimer = new PercentageTimer(150.0);

			mTexture = MonoParse.GetTexture(rootNode["texture"], null);

			mFont = MonoParse.GetFont(rootNode["font"], "Pixica-36");
			string textID = MonoParse.GetString(rootNode["text"]);
			mDisplayText = LanguageManager.I.GetText(textID);

			mDefaultColor = MonoParse.GetColor(rootNode["textColor"], Color.Gray);

			// Create hover color by brightening the default, or by loading from XML
			Color hoverDefault = mDefaultColor;
			MonoColor.BrightenColour(ref hoverDefault, 1.0f);
			mHoverColor = MonoParse.GetColor(rootNode["textHoverColor"], hoverDefault);

			// Set size if not already set
			if (mSize.X == 0.0f && mSize.Y == 0.0f)
			{
				if (mTexture is not null)
				{
					// By texture if we have one
					mSize.X = mTexture.Width;
					mSize.Y = mTexture.Height;
				}
				else
				{
					// By font if we have no texture
					mSize = mFont.MeasureString(mDisplayText);

					// Centre horizontally
					mPos.X -= mSize.X * 0.5f;
				}
			}
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update the menu button
		/// </summary>
		public override void Update(GameTime gameTime)
		{
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
				bool mouseClick = MouseClicked();

				if (enterKey || mouseClick)
				{
					mClickTimer.FullReset();
					mClickTimer.Start();
				}
			}

			base.Update(gameTime);
		}



		/// <summary>
		/// Do the button specific action
		/// </summary>
		public abstract void DoAction();

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the menu button
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			DrawButtonTexture(info);
			DrawButtonString(info);

			base.Draw(info);
		}



		/// <summary>
		/// Draw the button's texture
		/// </summary>
		void DrawButtonTexture(DrawInfo info)
		{
			if (mTexture is null)
			{
				return;
			}

			HoverState hoverState = GetHoverState();
			Vector2 texPosition = GetPosition();
			Color texColor = GetButtonColor();

			if (hoverState == HoverState.kPress)
			{
				texPosition.Y += 2.0f;
			}

			Vector2 shadowPos = texPosition + new Vector2(2.0f, 2.0f);
			Color shadowColor = texColor * 0.2f;

			MonoDraw.DrawTexture(info, mTexture, shadowPos, null, shadowColor, 0.0f, Vector2.Zero, 1.0f, GetSpriteEffect(), GetDepth());
			MonoDraw.DrawTexture(info, mTexture, texPosition, null, texColor, 0.0f, Vector2.Zero, 1.0f, GetSpriteEffect(), GetDepth());
		}



		/// <summary>
		/// Draw the button's text
		/// </summary>
		void DrawButtonString(DrawInfo info)
		{
			HoverState hoverState = GetHoverState();
			Vector2 textPos = GetPosition() + mSize * 0.5f;
			string modText = mDisplayText;
			Color textColor = GetButtonColor();

			if (hoverState == HoverState.kPress)
			{
				textPos.Y += 2.0f;
			}

			if(mTexture is null && IsSelected())
			{
				modText = string.Format(".{0}.", modText);
			}

			MonoDraw.DrawStringCentredShadow(info, mFont, textPos, textColor, modText, GetDepth());
		}



		/// <summary>
		/// Calculate the text color
		/// </summary>
		Color GetButtonColor()
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

		#endregion rDraw





		#region rUtil

		/// <summary>
		/// Calculate which hover state we are in
		/// </summary>
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

		/// <summary>
		/// Get sprite effects to draw button texture
		/// </summary>
		protected virtual SpriteEffects GetSpriteEffect()
		{
			return SpriteEffects.None;
		}

		#endregion rUtil
	}
}
