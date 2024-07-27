namespace AridArnold
{
	abstract class MenuOption : HitBoxNavElement
	{
		#region rConstants

		const float DEFAULT_OPTION_WIDTH = 800.0f;
		const float DEFAULT_OPTION_SPACING_PERCENT = 0.8f;

		#endregion rConstants




		#region rMembers

		SpriteFont mFont;
		string mDisplayText;
		
		Color mDefaultColor;
		Color mHoverColor;
		int mSelectedOption;
		bool mFirstUpdate;

		Vector2 mOptionCenPoint;

		string mDescTextBlockID;

		#endregion rMembers




		#region rInit

		/// <summary>
		/// Create a menu option
		/// </summary>
		public MenuOption(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
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
				// By font to get height
				mSize = mFont.MeasureString(mDisplayText);
				mSize.X = DEFAULT_OPTION_WIDTH;
			}

			// Centre horizontally
			mPos.X -= mSize.X * (DEFAULT_OPTION_SPACING_PERCENT - 0.35f);

			// Calculate where to centre options string.
			mOptionCenPoint = mPos;
			mOptionCenPoint.X += mSize.X * DEFAULT_OPTION_SPACING_PERCENT;
			mOptionCenPoint.Y += mSize.Y * 0.5f;

			mSelectedOption = 0;
			mFirstUpdate = true;

			mDescTextBlockID = MonoParse.GetString(rootNode["explainBox"], "");
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update the menu option
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update(GameTime gameTime)
		{
			SyncOption();

			int prevOption = mSelectedOption;
			if (IsSelected())
			{
				if (InputManager.I.AnyGangPressed(BindingGang.SysLeft))
				{
					mSelectedOption--;
				}
				else if (InputManager.I.AnyGangPressed(BindingGang.SysRight) || MouseClicked())
				{
					mSelectedOption++;
				}
			}

			int numOptions = GetNumOptions();
			mSelectedOption = (mSelectedOption + numOptions) % numOptions;

			if(mSelectedOption != prevOption || mFirstUpdate)
			{
				OnOptionSelect(mSelectedOption);
			}

			UpdateDescription(gameTime);

			base.Update(gameTime);

			mFirstUpdate = false;
		}



		/// <summary>
		/// Update description box.
		/// </summary>
		private void UpdateDescription(GameTime gameTime)
		{
			if(mDescTextBlockID.Length == 0)
			{
				return;
			}

			ETextBlock textBlock = (ETextBlock)GetParent().GetElementByID(mDescTextBlockID);

			if (IsSelected())
			{
				string descStrID = GetDescriptionStrID(mSelectedOption);
				textBlock.QueueText(descStrID);
			}

			// Hack to un-show description 
			NavElement selectedElement = GetParent().GetSelectedElement();
			if(selectedElement is null || selectedElement is not MenuOption)
			{
				textBlock.QueueText("");
			}
		}


		/// <summary>
		/// Called when a new option is selected
		/// </summary>
		protected abstract void OnOptionSelect(int optionIdx);



		/// <summary>
		/// Called every update to sync this UI widget with the underlying value
		/// </summary>
		protected abstract void SyncOption();

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the widget
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			Vector2 textPos = GetPosition();
			string modText = mDisplayText;
			Color textColor = IsSelected() ? mHoverColor : mDefaultColor;

			textPos.X += mFont.MeasureString(modText).X * 0.5f;
			textPos.Y += mSize.Y * 0.5f;

			MonoDraw.DrawStringCentredShadow(info, mFont, textPos, textColor, modText, GetDepth());
			MonoDraw.DrawStringCentredShadow(info, mFont, mOptionCenPoint, textColor, GetOptionStr(mSelectedOption), GetDepth());
			base.Draw(info);
		}

		#endregion rDraw





		#region rUtil

		/// <summary>
		/// Get the index of the selected option
		/// </summary>
		public int GetSelectedOptionIdx()
		{
			return mSelectedOption;
		}



		/// <summary>
		/// Set the selected option index
		/// </summary>
		public void SetSelectedOptionsIdx(int idx)
		{
			mSelectedOption = idx;
		}


		/// <summary>
		/// How many options do we have?
		/// </summary>
		protected abstract int GetNumOptions();



		/// <summary>
		/// Get string to display for nth option
		/// </summary>
		protected abstract string GetOptionStr(int optionIdx);


		/// <summary>
		/// Get a string id to display in the options description box.
		/// </summary>
		protected virtual string GetDescriptionStrID(int optionIdx)
		{
			return "";
		}

		#endregion rUtil
	}
}
