namespace AridArnold
{
	/// <summary>
	/// Tile you can collect once and have permanently
	/// </summary>
	abstract class PermanentCollectableTile : InteractableTile
	{
		protected bool mIsGhost;

		public PermanentCollectableTile(Vector2 pos) : base(pos)
		{
			mIsGhost = CollectableManager.I.HasSpecific(CalculateSpecificID());
		}

		protected abstract PermanentCollectable GetCollectableType();

		protected virtual byte GetImplByte() { return (byte)((mTileMapIndex.X + mTileMapIndex.Y) >> 8); }

		private UInt16 GetItemType()
		{
			byte item = (byte)GetCollectableType();
			byte impl = GetImplByte();

			UInt16 ret = (UInt16)(((UInt16)item << 8) | impl);

			return ret;
		}

		private UInt32 GetLevelID()
		{
			return (UInt32)(CampaignManager.I.GetCurrentLevel().GetID());
		}

		protected UInt64 CalculateSpecificID()
		{
			byte xPos = (byte)mTileMapIndex.X;
			byte yPos = (byte)mTileMapIndex.Y;
			UInt32 levelID = GetLevelID();
			UInt16 item = GetItemType();

			UInt64 ret = ((UInt64)xPos << 56) |
						 ((UInt64)yPos << 48) |
						 ((UInt64)levelID << 16) |
						 item;

			return ret;
		}

		public override void OnEntityIntersect(Entity entity)
		{
			if (entity is Arnold)
			{
				if (mIsGhost == false)
				{
					CollectableManager.I.CollectPermanentItem(GetItemType(), CalculateSpecificID());
				}
				mEnabled = false;
				OnCollect();
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
			if(mIsGhost)
			{
				return mGhostAnim.GetCurrentTexture();
			}

			return mFullAnim.GetCurrentTexture();
		}
	}
}
