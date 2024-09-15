namespace AridArnold
{
	/// <summary>
	/// Tile you can collect once and have permanently
	/// </summary>
	abstract class PermanentCollectableTile : InteractableTile
	{
		const float BOB_SPEED = 0.4f;
		const float BOB_AMOUNT = 1.8f;

		protected bool mIsGhost;
		protected Animator mExitAnim;
		protected bool mHasBeenCollected;
		float mBobAngle = 0.0f;

		AridArnoldSFX? mCollectSFX;
		float mCollectVol;

		public PermanentCollectableTile(Vector2 pos) : base(pos)
		{
			mIsGhost = CollectableManager.I.HasSpecific(mTileMapIndex, GetItemID());
			mExitAnim = null;
			mHasBeenCollected = false;

			LoadSFX(AridArnoldSFX.Collect, 0.5f);

			mBobAngle = pos.X * 0.3f + pos.Y * MathF.PI;
		}

		protected void LoadSFX(AridArnoldSFX sound, float vol)
		{
			mCollectSFX = sound;
			mCollectVol = vol;
		}

		protected abstract CollectableCategory GetCollectableType();

		protected virtual byte GetImplByte() { return 0; }

		public override void Update(GameTime gameTime)
		{
			if (mExitAnim is not null)
				mExitAnim.Update(gameTime);

			if (mHasBeenCollected && (mExitAnim is null || !mExitAnim.IsPlaying()))
			{
				mEnabled = false;
			}

			float dt = Util.GetDeltaT(gameTime);
			mBobAngle += dt * BOB_SPEED;

			mDrawOffset = Util.GetNormal(mRotation) * MathF.Sin(mBobAngle) * BOB_AMOUNT;

			if(mBobAngle > MathF.PI * 2.0f)
			{
				mBobAngle -= MathF.PI * 2.0f;
			}

			base.Update(gameTime);
		}

		private UInt16 GetItemID()
		{
			CollectableCategory category = GetCollectableType();
			byte impl = GetImplByte();

			return CollectableManager.GetCollectableID(category, impl);
		}

		public override void OnEntityIntersect(Entity entity)
		{
			if (mHasBeenCollected)
			{
				return;
			}
			if (entity.OnInteractLayer(InteractionLayer.kPlayer))
			{
				if (mIsGhost == false)
				{
					CollectableManager.I.CollectSpecificItem(GetItemID(), mTileMapIndex);
				}
				OnCollect();

				mHasBeenCollected = true;
				if (mExitAnim is not null)
				{
					mExitAnim.Play();
				}

				if(mCollectSFX.HasValue)
				{
					SFXManager.I.PlaySFX(mCollectSFX.Value, mCollectVol, 0.0f, 0.0f, 0.0f);
				}
			}
		}

		/// <summary>
		/// Called when the tile is collected.
		/// </summary>
		protected virtual void OnCollect()
		{
		}
	}


	/// <summary>
	/// Perm collectable that displays one of two animators.
	/// </summary>
	abstract class PermCollectSimpleTile : PermanentCollectableTile
	{
		protected Animator mFullAnim;
		protected Animator mGhostAnim; // Let's make these virtual haha

		protected PermCollectSimpleTile(Vector2 pos) : base(pos)
		{
		}

		public override void Update(GameTime gameTime)
		{
			mFullAnim.Update(gameTime);
			mGhostAnim.Update(gameTime);
			base.Update(gameTime);
		}

		public override Texture2D GetTexture()
		{
			if (mHasBeenCollected)
			{
				return mExitAnim.GetCurrentTexture();
			}

			if (mIsGhost)
			{
				return mGhostAnim.GetCurrentTexture();
			}

			return mFullAnim.GetCurrentTexture();
		}
	}
}
