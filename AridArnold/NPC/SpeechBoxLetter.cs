namespace AridArnold
{
	internal class SpeechBoxLetter
	{
		#region rMembers

		SpriteFont mFont;
		Vector2 mPosition;
		Vector2 mAnimationDisplacement;
		char mCharacter;
		Color mColor;

		#endregion rMembers


		#region rInitialisation

		/// <summary>
		/// Speech box with a font
		/// </summary>
		public SpeechBoxLetter(SpriteFont font, char character, Vector2 pos, Color color)
		{
			mFont = font;
			mPosition = pos;
			mAnimationDisplacement = Vector2.Zero;
			mCharacter = character;
			mColor = color;
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update letter animation
		/// </summary>
		public void Update(GameTime gameTime)
		{
			
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
			MonoDraw.DrawString(drawInfo, mFont, mCharacter.ToString(), mPosition + mAnimationDisplacement, mColor, MonoDraw.LAYER_TEXT);
		}

		#endregion rDraw


	}
}
