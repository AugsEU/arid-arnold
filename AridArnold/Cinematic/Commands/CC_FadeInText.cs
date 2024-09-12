using System;

namespace AridArnold
{
	internal class CC_FadeInText : CinematicCommand
	{
		const float FADE_QUANT = 4.0f;

		SpriteFont mFont;
		Vector2 mCentrePos;
		PercentageTimer mFadeTimer;
		Color mTextColor;
		string mStrID;
		string mLocString;
		DrawLayer mLayer;

		public CC_FadeInText(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			mCentrePos = MonoParse.GetVector(cmdNode);

			mStrID = MonoParse.GetString(cmdNode["text"]);
			

			mTextColor = MonoParse.GetColor(cmdNode["color"], new Color(200, 200, 200));

			float fadeTime = MonoParse.GetFloat(cmdNode["fade"]);
			mFadeTimer = new PercentageTimer((double)fadeTime);

			mFont = MonoParse.GetFont(cmdNode["font"], "Pixica-24");

			mLayer = MonoParse.GetEnum<DrawLayer>(cmdNode["layer"], DrawLayer.Default);

			mLocString = "";
		}

		public override void Update(GameTime gameTime, int currentFrame)
		{
			if(mLocString.Length == 0)
			{
				mLocString = LanguageManager.I.GetText(mStrID);
			}
			mFadeTimer.Start();
			mFadeTimer.Update(gameTime);
			base.Update(gameTime, currentFrame);
		}

		public override void Draw(DrawInfo info, int currentFrame)
		{
			if (mLocString.Length == 0)
			{
				return;
			}

			float t = MathF.Floor(FADE_QUANT * mFadeTimer.GetPercentageF()) / FADE_QUANT;
			Color lerpColor = MonoMath.Lerp(Color.Black, mTextColor, t);

			MonoDraw.DrawStringCentredShadow(info, mFont, mCentrePos, lerpColor, mLocString, mLayer);
		}
	}
}
