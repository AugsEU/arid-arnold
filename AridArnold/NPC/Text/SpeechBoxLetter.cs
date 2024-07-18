namespace AridArnold
{
	internal class SpeechBoxLetter
	{
		#region rMembers

		SpriteFont mFont;
		Vector2 mPosition;
		char mCharacter;
		LetterAnimation mAnimation;
		MonoTimer mLifeTimer;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Speech box with a font
		/// </summary>
		public SpeechBoxLetter(SpriteFont font, char character, Vector2 pos, LetterAnimation anim)
		{
			mFont = font;
			mPosition = pos;
			mCharacter = character;
			mAnimation = anim;
			mLifeTimer = new MonoTimer();
			mLifeTimer.Start();
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update letter animation
		/// </summary>
		public void Update(GameTime gameTime)
		{
			mLifeTimer.Update(gameTime);
		}



		/// <summary>
		/// Move letter up by dy.
		/// </summary>
		public void MoveUp(float dy)
		{
			mPosition.Y -= dy;
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw this character.
		/// </summary>
		public void Draw(DrawInfo drawInfo)
		{
			float lifeTime = (float)mLifeTimer.GetElapsedMs();
			Vector2 drawPos = mPosition + mAnimation.GetPosition(lifeTime);
			drawPos.Y = MonoMath.Round(drawPos.Y);
			MonoDraw.DrawString(drawInfo, mFont, mCharacter.ToString(), mPosition + mAnimation.GetPosition(lifeTime), mAnimation.GetColor(lifeTime), DrawLayer.Bubble);
		}

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Get base position of text
		/// </summary>
		public Vector2 GetPosition()
		{
			return mPosition;
		}

		#endregion rUtility
	}
}
