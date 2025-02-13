﻿namespace AridArnold
{
	/// <summary>
	/// A door that requires a number of keys
	/// </summary>
	internal class KeyDoor : Entity
	{
		#region rConstants

		const float INFO_DISTANCE = 29.0f;
		const float UNLOCK_DISTANCE = 20.0f;

		#endregion rConstants





		#region rMembers

		bool mHasKeysRequired;
		int[] mDisplayDigits;
		Texture2D[] mNumberTextures;
		Animator mUnlockAnim;
		Point mTileCoord;
		LevelLockInfoBubble mHelpBubble;

		bool mIsUnlocking;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Create door at position.
		/// </summary>
		public KeyDoor(Vector2 pos, int numRequired) : base(pos)
		{
			mHasKeysRequired = CollectableManager.I.GetNumCollected((UInt16)CollectableCategory.Key) >= numRequired;
			mDisplayDigits = MonoMath.GetDigits(numRequired);
			// Can't display more than 3 digits
			MonoDebug.Assert(mDisplayDigits.Length <= 3);

			mHelpBubble = new LevelLockInfoBubble(pos + new Vector2(8.0f, -4.0f), BubbleStyle.DefaultPrompt);

			mTileCoord = TileManager.I.GetTileMapCoord(mPosition);
			SetEnabled(!CollectableManager.I.HasSpecific(mTileCoord, GetCollectType()));
			mIsUnlocking = false;
		}

		/// <summary>
		/// Load content
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Shared/KeyDoor/LevelLock");

			mNumberTextures = new Texture2D[10];
			mNumberTextures[0] = MonoData.I.MonoGameLoad<Texture2D>("Shared/KeyDoor/LockZero");
			mNumberTextures[1] = MonoData.I.MonoGameLoad<Texture2D>("Shared/KeyDoor/LockOne");
			mNumberTextures[2] = MonoData.I.MonoGameLoad<Texture2D>("Shared/KeyDoor/LockTwo");
			mNumberTextures[3] = MonoData.I.MonoGameLoad<Texture2D>("Shared/KeyDoor/LockThree");
			mNumberTextures[4] = MonoData.I.MonoGameLoad<Texture2D>("Shared/KeyDoor/LockFour");
			mNumberTextures[5] = MonoData.I.MonoGameLoad<Texture2D>("Shared/KeyDoor/LockFive");
			mNumberTextures[6] = MonoData.I.MonoGameLoad<Texture2D>("Shared/KeyDoor/LockSix");
			mNumberTextures[7] = MonoData.I.MonoGameLoad<Texture2D>("Shared/KeyDoor/LockSeven");
			mNumberTextures[8] = MonoData.I.MonoGameLoad<Texture2D>("Shared/KeyDoor/LockEight");
			mNumberTextures[9] = MonoData.I.MonoGameLoad<Texture2D>("Shared/KeyDoor/LockNine");

			mUnlockAnim = new Animator(Animator.PlayType.OneShot,
																	("Shared/KeyDoor/LevelLockUnlock1", 0.14f),
																	("Shared/KeyDoor/LevelLockUnlock2", 0.14f),
																	("Shared/KeyDoor/LevelLockUnlock3", 0.14f),
																	("Shared/KeyDoor/LevelLockUnlock4", 0.14f),
																	("Shared/KeyDoor/LevelLockUnlock5", 0.14f));
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Upadte door.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			if (mIsUnlocking)
			{
				mUnlockAnim.Update(gameTime);

				if (!mUnlockAnim.IsPlaying())
				{
					SetEnabled(false);
				}

				return;
			}

			// Collision
			EntityManager.I.AddColliderSubmission(new EntityColliderSubmission(this));

			// Near check
			if (mHasKeysRequired)
			{
				if (EntityManager.I.AnyNearMe(UNLOCK_DISTANCE, this, typeof(Arnold), typeof(Androld)))
				{
					UnlockDoor();
				}
			}
			else
			{
				bool showInfo = EntityManager.I.AnyNearMe(INFO_DISTANCE, this, typeof(Arnold), typeof(Androld));
				mHelpBubble.Update(gameTime, showInfo);
			}


			base.Update(gameTime);
		}



		/// <summary>
		/// Collider bounds
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			return new Rect2f(mPosition, Tile.sTILE_SIZE, Tile.sTILE_SIZE);
		}



		/// <summary>
		/// Unlock the door
		/// </summary>
		void UnlockDoor()
		{
			if(mIsUnlocking)
			{
				return;
			}

			SFXManager.I.PlaySFX(AridArnoldSFX.Unlock, 0.4f);
			CollectableManager.I.CollectSpecificItem(GetCollectType(), mTileCoord);
			mUnlockAnim.Play();
			mIsUnlocking = true;
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the lock
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			if (mIsUnlocking)
			{
				MonoDraw.DrawTextureDepth(info, mUnlockAnim.GetCurrentTexture(), mPosition, DrawLayer.Tile);
			}
			else
			{
				MonoDraw.DrawTextureDepth(info, mTexture, mPosition, DrawLayer.Tile);

				// Draw digits
				Vector2 offset = new Vector2(0.0f, 2.0f);

				switch (mDisplayDigits.Length)
				{
					case 1: offset.X = 6.0f; break;
					case 2: offset.X = 3.0f; break;
					case 3: offset.X = 1.0f; break;
				}

				for (int i = 0; i < mDisplayDigits.Length; i++)
				{
					MonoDraw.DrawTextureDepth(info, mNumberTextures[mDisplayDigits[i]], mPosition + offset, DrawLayer.Tile);

					offset.X += 5.0f;
				}

				mHelpBubble.Draw(info);
			}
		}

		#endregion rDraw





		#region rCollect

		UInt16 GetCollectType()
		{
			return (UInt16)CollectableCategory.LevelLock;
		}

		#endregion rCollect

	}
}
