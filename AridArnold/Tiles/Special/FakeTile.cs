namespace AridArnold
{
	class FakeTile : AirTile
	{
		#region rMembers

		Texture2D mApparentTexture;
		Texture2D mFadedTexture;

		#endregion rMembers



		#region rInitialisation

		/// <summary>
		/// Teleportation pipe
		/// </summary>
		public FakeTile(Vector2 position) : base(position)
		{
		}



		/// <summary>
		/// Load textures for teleportation pipe
		/// </summary>
		public override void LoadContent()
		{
			mApparentTexture = MonoData.I.MonoGameLoad<Texture2D>("ApparentWall");
			mFadedTexture = MonoData.I.MonoGameLoad<Texture2D>("FadedWall");
			UpdateTexture(false);
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update teleport tile
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			Rect2f fakeCollider = new Rect2f(mPosition, mPosition + new Vector2(sTILE_SIZE, sTILE_SIZE));
			bool faded = false;
			List<Entity> arnoldList = EntityManager.I.GetAllOfType(typeof(Arnold));
			foreach(Entity arnold in arnoldList)
			{
				if (Collision2D.BoxVsBox(fakeCollider, arnold.ColliderBounds()))
				{
					faded = true;
					break;
				}
			}

			UpdateTexture(faded);
		}



		/// <summary>
		/// Update texture to either faded or apparent.
		/// </summary>
		void UpdateTexture(bool isFaded)
		{
			mTexture = isFaded ? mFadedTexture : mApparentTexture;
		}

		#endregion rUpdate
	}
}
