namespace AridArnold
{
	/// <summary>
	/// Renders the background.
	/// </summary>
	abstract class BGRenderer
	{
		public abstract void Update(GameTime gameTime);

		public abstract void Draw(DrawInfo drawInfo);

		
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


		public override void Draw(DrawInfo drawInfo)
		{
			MonoDraw.DrawTextureDepth(drawInfo, mAnimator.GetCurrentTexture(), Vector2.Zero, MonoDraw.LAYER_BG);
		}

		public override void Update(GameTime gameTime)
		{
			mAnimator.Update(gameTime);
		}
	}
}
