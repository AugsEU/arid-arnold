namespace AridArnold
{
	/// <summary>
	/// A tile you can collect
	/// </summary>
	abstract class CollectableTile : InteractableTile
	{
		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public CollectableTile(Vector2 position) : base(position)
		{
		}

		protected abstract CollectableType GetCollectableType();

		public override void OnEntityIntersect(Entity entity)
		{
			if (entity is Arnold)
			{
				CollectableManager.I.CollectItem(GetCollectableType());
				mEnabled = false;
				OnCollect();
			}
		}

		/// <summary>
		/// Called when the tile is collected.
		/// </summary>
		public virtual void OnCollect()
		{
		}
	}





	/// <summary>
	/// A flag, represents the end of a world and a checkpoint
	/// </summary>
	class FlagTile : CollectableTile
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
																		, ("Tiles/flag2", 0.3f)
																		, ("Tiles/flag3", 1.2f)
																		, ("Tiles/flag2", 0.3f)
																		, ("Tiles/flag1", 0.3f));
			Animator faulterAnim = new Animator(Animator.PlayType.OneShot
																		, ("Tiles/flag2", 0.3f)
																		, ("Tiles/flag1", 0.3f));
			Animator wavesAnim = new Animator(Animator.PlayType.OneShot
																		, ("Tiles/flag0", 0.3f)
																		, ("Tiles/flag1", 0.3f)
																		, ("Tiles/flag0", 0.3f)
																		, ("Tiles/flag1", 0.3f));
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

		protected override CollectableType GetCollectableType()
		{
			return CollectableType.Flag;
		}
	}





	/// <summary>
	/// Hot dog tile. Gives the player 1 life
	/// </summary>
	class HotDogTile : InteractableTile
	{
		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public HotDogTile(Vector2 position) : base(position)
		{
		}



		/// <summary>
		/// Load all textures and assets
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/HotDog");
		}



		/// <summary>
		/// Give the player a life when intersecting this tile.
		/// </summary>
		public override void OnEntityIntersect(Entity entity)
		{
			if (entity is Arnold)
			{
				// To do: Give lives
				//int livesBefore = ProgressManager.I.GetNumLives();
				//ProgressManager.I.GiveLife();
				//mEnabled = false;

				////If there is actually a life increase
				//if (livesBefore < ProgressManager.I.GetNumLives())
				//{
				//	FXManager.I.AddTextScroller(FontManager.I.GetFont("Pixica Micro-24"), Color.OliveDrab, entity.GetPos(), "+1 Life");
				//}
				//else
				//{
				//	FXManager.I.AddTextScroller(FontManager.I.GetFont("Pixica Micro-24"), Color.White, entity.GetPos(), "+0 Lives");
				//}
			}

			base.OnEntityIntersect(entity);
		}
	}





	/// <summary>
	/// Water bottle tile. Needed to complete most levels.
	/// </summary>
	class WaterBottleTile : CollectableTile
	{
		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public WaterBottleTile(Vector2 position) : base(position)
		{
		}



		/// <summary>
		/// Load all textures and assets
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Bottle");
		}

		protected override CollectableType GetCollectableType()
		{
			return CollectableType.WaterBottle;
		}

		public override void OnCollect()
		{
			FXManager.I.AddAnimator(mPosition, "Tiles/BottleFade.max", DrawLayer.TileEffects);
		}
	}
}
