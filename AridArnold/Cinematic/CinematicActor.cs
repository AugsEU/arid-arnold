namespace AridArnold
{
	class CinematicActor
	{
		#region rMembers

		string mName;
		Vector2 mPosition;
		DrawLayer mDrawLayer;
		DrawLayer mInitialDrawLayer;
		Animator mActiveAnimation;

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
			mActiveAnimation = null;
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update the animations
		/// </summary>
		public void Update(GameTime gameTime)
		{
			if(mActiveAnimation != null)
			{
				mActiveAnimation.Update(gameTime);
			}
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the actor
		/// </summary>
		public void Draw(DrawInfo info)
		{
			if(mActiveAnimation is not null)
			{
				MonoDraw.DrawTextureDepth(info, mActiveAnimation.GetCurrentTexture(), mPosition, mDrawLayer);
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
		public void SetActiveAnimation(Animator activeAnimation)
		{
			mActiveAnimation = activeAnimation;
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
			mActiveAnimation = null;
			mPosition = Vector2.Zero;
			mDrawLayer = mInitialDrawLayer;
		}

		#endregion rUtil
	}
}
