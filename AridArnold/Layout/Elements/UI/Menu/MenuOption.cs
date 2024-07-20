
using Microsoft.Xna.Framework.Graphics;
using System.Text.RegularExpressions;

namespace AridArnold
{
	abstract class MenuOption : HitBoxNavElement
	{
		const float DEFAULT_OPTION_WIDTH = 800.0f;
		const float DEFAULT_OPTION_SPACING_PERCENT = 0.8f;

		SpriteFont mFont;
		string mDisplayText;
		string[] mOptionStrs;
		Color mDefaultColor;
		Color mHoverColor;
		int mSelectedOption;

		Vector2 mOptionCenPoint;

		public MenuOption(XmlNode rootNode, Layout parent, string[] optionIDs, bool isLoc = false) : base(rootNode, parent)
		{
			mFont = MonoParse.GetFont(rootNode["font"], "Pixica-36");
			string textID = MonoParse.GetString(rootNode["text"]);
			mDisplayText = LanguageManager.I.GetText(textID);

			// Parse options and localise if needed
			mOptionStrs = new string[optionIDs.Length];
			for (int i = 0; i < optionIDs.Length; i++)
			{
				string locStr = isLoc ? optionIDs[i] : LanguageManager.I.GetText(optionIDs[i]);
				mOptionStrs[i] = string.Format("<{0}>", locStr);
			}

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
			mPos.X -= mSize.X * 0.5f;

			// Calculate where to centre options string.
			mOptionCenPoint = mPos;
			mOptionCenPoint.X += mSize.X * DEFAULT_OPTION_SPACING_PERCENT;
			mOptionCenPoint.Y += mSize.Y * 0.5f;

			mSelectedOption = 0;
		}

		public override void Update(GameTime gameTime)
		{
			int prevOption = mSelectedOption;
			if (IsSelected())
			{
				if (InputManager.I.AnyGangPressed(BindingGang.SysLeft))
				{
					mSelectedOption--;
				}
				else if (InputManager.I.AnyGangPressed(BindingGang.SysRight))
				{
					mSelectedOption++;
				}
			}

			mSelectedOption = (mSelectedOption + mOptionStrs.Length) % mOptionStrs.Length;

			if(mSelectedOption != prevOption)
			{
				OnOptionSelect(mSelectedOption);
			}

			base.Update(gameTime);
		}

		protected abstract void OnOptionSelect(int optionIdx);

		public override void Draw(DrawInfo info)
		{
			Vector2 textPos = GetPosition();
			string modText = mDisplayText;
			Color textColor = IsSelected() ? mHoverColor : mDefaultColor;

			textPos.X += mFont.MeasureString(modText).X * 0.5f;
			textPos.Y += mSize.Y * 0.5f;

			MonoDraw.DrawStringCentredShadow(info, mFont, textPos, textColor, modText, GetDepth());
			MonoDraw.DrawStringCentredShadow(info, mFont, mOptionCenPoint, textColor, mOptionStrs[mSelectedOption], GetDepth());
			base.Draw(info);
		}

		public int GetSelectedOptionIdx()
		{
			return mSelectedOption;
		}
	}
}
