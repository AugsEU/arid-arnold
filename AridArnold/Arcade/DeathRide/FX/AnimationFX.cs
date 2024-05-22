namespace GMTK2023
{
	class AnimationFX : FX
	{
		Vector2 mPosition;
		Animator mAnimator;
		DrawLayer mLayer;

		public AnimationFX(Vector2 position, Animator animator, DrawLayer layer)
		{
			Init(position, animator, layer);
		}

		public AnimationFX(Vector2 position, string animator, DrawLayer layer)
		{
			Init(position, MonoData.I.LoadAnimator(animator), layer);
		}

		void Init(Vector2 position, Animator animator, DrawLayer layer)
		{
			mPosition = position;
			mAnimator = animator;
			mLayer = layer;
			mAnimator.Play();
		}

		public override void Draw(DrawInfo info)
		{
			MonoDraw.DrawTextureDepth(info, mAnimator.GetCurrentTexture(), mPosition, mLayer);
		}

		public override bool Finished()
		{
			return !mAnimator.IsPlaying();
		}

		public override void Update(GameTime gameTime)
		{
			mAnimator.Update(gameTime);
		}
	}
}
