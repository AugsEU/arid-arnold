namespace AridArnold
{
	/// <summary>
	/// A flag, represents the end of a world and a checkpoint
	/// </summary>
	class FlagTile : TransientCollectableTile
	{
		IdleAnimator mIdleAnimator;

		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public FlagTile(Vector2 position) : base(position)
		{
		}

		/// <summary>
		/// Load all textures
		/// </summary>
		public override void LoadContent()
		{
			Animator fallDownAnim = new Animator(Animator.PlayType.OneShot
																		, ("Tiles/Flag3", 0.3f)
																		, ("Tiles/Flag4", 1.2f)
																		, ("Tiles/Flag3", 0.3f)
																		, ("Tiles/Flag2", 0.3f));
			Animator faulterAnim = new Animator(Animator.PlayType.OneShot
																		, ("Tiles/Flag3", 0.3f)
																		, ("Tiles/Flag2", 0.3f));
			Animator wavesAnim = new Animator(Animator.PlayType.OneShot
																		, ("Tiles/Flag1", 0.3f)
																		, ("Tiles/Flag2", 0.3f)
																		, ("Tiles/Flag1", 0.3f)
																		, ("Tiles/Flag2", 0.3f));
			mIdleAnimator = new IdleAnimator(wavesAnim, 40.0f, faulterAnim, fallDownAnim);
		}

		public override void Update(GameTime gameTime)
		{
			mIdleAnimator.Update(gameTime);

			base.Update(gameTime);
		}

		public override Texture2D GetTexture()
		{
			return mIdleAnimator.GetCurrentTexture(); ;
		}

		public override void OnEntityIntersect(Entity entity)
		{
			if (entity is Arnold)
			{
				// To do: Collect flag and return to hub?
			}

			base.OnEntityIntersect(entity);
		}

		protected override TransientCollectable GetCollectableType()
		{
			return TransientCollectable.Flag;
		}
	}
}
