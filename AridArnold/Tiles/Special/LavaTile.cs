
namespace AridArnold
{
	class LavaTile : InteractableTile
	{
		Animator mLavaAnimation;

		public LavaTile(Vector2 position) : base(position)
		{
		}

		public override void LoadContent()
		{
			MonoRandom rng = new MonoRandom(0);
			rng.ChugNumber(mTileMapIndex.X);

			string[] animPaths = { "Tiles/Hell/LavaAnim1.max", "Tiles/Hell/LavaAnim2.max" };
			int bubbleIdx = rng.GetIntRange(0, animPaths.Length - 1);
			int frameDelay = rng.GetIntRange(-1, 1);

			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Hell/Lava1");
			mLavaAnimation = MonoData.I.LoadAnimator(animPaths[bubbleIdx]);
			mLavaAnimation.Play();
			mLavaAnimation.MoveFrames(frameDelay);
		}

		public override void Update(GameTime gameTime)
		{
			mLavaAnimation.Update(gameTime);
			base.Update(gameTime);
		}

		public override Texture2D GetTexture()
		{
			return mLavaAnimation.GetCurrentTexture();
		}

		public override void OnEntityIntersect(Entity entity)
		{
			if (MonoAlg.TestFlag(entity.GetInteractionLayer(), InteractionLayer.kPlayer))
			{
				entity.Kill();
			}
		}
	}
}
