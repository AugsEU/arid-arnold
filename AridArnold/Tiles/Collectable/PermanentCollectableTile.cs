﻿using AridArnold.Tiles.Basic;

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
			mIsGhost = CollectableManager.I.HasSpecific(mTileMapIndex, GetItemType());
		}

		protected abstract PermanentCollectable GetCollectableType();

		protected virtual byte GetImplByte() { return 0; }

		private UInt16 GetItemType()
		{
			byte item = (byte)GetCollectableType();
			byte impl = GetImplByte();

			UInt16 ret = (UInt16)(((UInt16)item << 8) | impl);

			return ret;
		}

		public override void OnEntityIntersect(Entity entity)
		{
			if (entity is Arnold)
			{
				if (mIsGhost == false)
				{
					CollectableManager.I.CollectPermanentItem(mTileMapIndex, GetItemType());
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