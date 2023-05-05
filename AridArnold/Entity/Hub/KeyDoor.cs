namespace AridArnold
{
	/// <summary>
	/// A door that requires a number of keys
	/// </summary>
	internal class KeyDoor : Entity
	{
		#region rMembers

		int mNumKeysRequired;
		int[] mDisplayDigits;
		Texture2D[] mNumberTextures;
		Point mTileCoord;

		#endregion rMembers





		#region rInitialisation
		
		/// <summary>
		/// Create door at position.
		/// </summary>
		public KeyDoor(Vector2 pos, int numRequired) : base(pos)
		{
			mNumKeysRequired = numRequired;
			mDisplayDigits = MonoMath.GetDigits(numRequired);

			// Can't display more than 3 digits
			MonoDebug.Assert(mDisplayDigits.Length <= 3);

			mTileCoord = TileManager.I.GetTileMapCoord(mPosition);

			MonoDebug.Assert(mTileCoord.X >= 0 && mTileCoord.Y >= 0);

			SetEnabled(!CollectableManager.I.HasSpecific(mTileCoord, GetCollectType()));
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
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Upadte door.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			// Collision
			EntityManager.I.AddColliderSubmission(new EntityColliderSubmission(this));

			base.Update(gameTime);
		}



		/// <summary>
		/// Collider bounds
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			return new Rect2f(mPosition, Tile.sTILE_SIZE, Tile.sTILE_SIZE);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the lock
		/// </summary>
		public override void Draw(DrawInfo info)
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
		}

		#endregion rDraw





		#region rCollect

		UInt16 GetCollectType()
		{
			return (UInt16)PermanentCollectable.LevelLock;
		}

		#endregion rCollect

	}
}
