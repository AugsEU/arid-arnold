namespace AridArnold
{
	class CinematicActor
	{
		#region rMembers

		string mName;
		Vector2 mPosition;
		DrawLayer mDrawLayer;
		DrawLayer mInitialDrawLayer;
		SpriteEffects mEffect;
		Texture2D mDrawTexture;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create cinematic actor from xml node
		/// </summary>
		public CinematicActor(XmlNode actorNode)
		{
			mName = actorNode["name"].InnerText;
			mDrawLayer = MonoAlg.GetEnumFromString<DrawLayer>(actorNode["layer"].InnerText);
			mInitialDrawLayer = mDrawLayer;
			mPosition = MonoParse.GetVector(actorNode);
			mDrawTexture = null;
			mEffect = SpriteEffects.None;
		}

		#endregion rInit





		#region rDraw

		/// <summary>
		/// Draw the actor
		/// </summary>
		public void Draw(DrawInfo info)
		{
			if (mDrawTexture is not null)
			{
				MonoDraw.DrawTexture(info, mDrawTexture, mPosition, null, Color.White, 0.0f, Vector2.Zero, 1.0f, mEffect, mDrawLayer);
			}
		}

		#endregion rDraw





		#region rUtil

		/// <summary>
		/// Get position of actor
		/// </summary>
		public Vector2 GetPosition()
		{
			return mPosition;
		}



		/// <summary>
		/// Set position of actor
		/// </summary>
		public void SetPosition(Vector2 pos)
		{
			mPosition = pos;
		}



		/// <summary>
		/// Set draw layer
		/// </summary>
		public void SetDrawLayer(DrawLayer drawLayer)
		{
			mDrawLayer = drawLayer;
		}



		/// <summary>
		/// Load an active animation into an actor
		/// </summary>
		public void SetDrawTexture(Texture2D drawTex)
		{
			mDrawTexture = drawTex;
		}



		/// <summary>
		/// Set reflection settings.
		/// </summary>
		public void SetSpriteEffect(SpriteEffects spriteEffect)
		{
			mEffect = spriteEffect;
		}



		/// <summary>
		/// Name
		/// </summary>
		public string GetName()
		{
			return mName;
		}



		/// <summary>
		/// Reset the actor
		/// </summary>
		public void Reset()
		{
			mDrawTexture = null;
			mPosition = Vector2.Zero;
			mDrawLayer = mInitialDrawLayer;
			mEffect = SpriteEffects.None;
		}

		#endregion rUtil
	}
}
