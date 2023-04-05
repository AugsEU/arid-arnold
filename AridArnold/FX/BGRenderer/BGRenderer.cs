namespace AridArnold
{
	/// <summary>
	/// Renders the background.
	/// </summary>
	abstract class BGRenderer
	{
		protected List<BGElement> mElements;

		public BGRenderer()
		{
			mElements = new List<BGElement>();
		}

		public virtual void Update(GameTime gameTime)
		{
			foreach (BGElement element in mElements)
			{
				element.Update(gameTime);
			}
		}

		public virtual void Draw(DrawInfo info)
		{
			foreach (BGElement element in mElements)
			{
				element.Draw(info);
			}
		}

		
		/// <summary>
		/// Factory.
		/// </summary>
		public static BGRenderer GetRenderer(string name)
		{
			name = name.ToLower();
			switch(name)
			{
				case "steamplant":
					return new SteamPlantBG();
				default: // By default interpret name as anim path.
					return new ImageBGRenderer(name);
			}
		}
	}

	/// <summary>
	/// Renders a basic animation as the background.
	/// </summary>
	class ImageBGRenderer : BGRenderer
	{
		Animator mAnimator;

		public ImageBGRenderer(string animationPath)
		{
			mAnimator = MonoData.I.LoadAnimator(animationPath);
		}


		public override void Draw(DrawInfo info)
		{
			MonoDraw.DrawTextureDepth(info, mAnimator.GetCurrentTexture(), Vector2.Zero, MonoDraw.LAYER_BG);

			base.Draw(info);
		}

		public override void Update(GameTime gameTime)
		{
			mAnimator.Update(gameTime);

			base.Update(gameTime);
		}
	}

	/// <summary>
	/// Element in the background.
	/// </summary>
	abstract class BGElement
	{
		Vector2 mPos;

		public BGElement(Vector2 pos)
		{
			mPos = pos;
		}

		public abstract void Update(GameTime gameTime);

		protected abstract Texture2D GetDrawTexture();

		public void Draw(DrawInfo info)
		{
			MonoDraw.DrawTextureDepth(info, GetDrawTexture(), mPos, MonoDraw.LAYER_BG_ELEMENT);
		}
	}
}
