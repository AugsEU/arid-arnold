namespace AridArnold
{
	class RailPlatform : Entity
	{
		#region rMembers

		RailTraveller mRail;
		int mSize;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Factory to create a rail platform from a rail node and register it.
		/// </summary>
		public static void TryCreateRailPlatformAtNode(LinearRailData railData, int idx, ContentManager content)
		{
			RailNode node = railData.GetNode(idx);

			if (node.mType != RailNode.NodeType.HasPlatform)
			{
				return;
			}

			RailTraveller railTraveller;

			switch (railData.GetRailType())
			{
				case LinearRailData.RailType.BackAndForth:
					railTraveller = new BackAndForthLinearRailTraveller(idx, railData);
					break;
				case LinearRailData.RailType.Cycle:
					railTraveller = new CycleLinearRailTraveller(idx, railData);
					break;
				default:
					throw new NotImplementedException();
			}

			EntityManager.I.RegisterEntity(new RailPlatform(railTraveller, railData.GetSize()), content);
		}
		

		
		/// <summary>
		/// Create rail platform that travels along a rail.
		/// </summary>
		public RailPlatform(RailTraveller rail, int size) : base(rail.GetPosition())
		{
			mRail = rail;
			mSize = size;
		}


		/// <summary>
		/// Load content texture.
		/// </summary>
		public override void LoadContent(ContentManager content)
		{
			// TO DO: Load appropriate texture.
			mTexture = content.Load<Texture2D>("Tiles/IronWorks/platform");
		}

		#endregion rInitialisation





		#region rUpdate

		public override void Update(GameTime gameTime)
		{
			mRail.Update(gameTime);

			//Util.DLog("Velocity: " + mRail.GetVelocity(gameTime).ToString() + " | Position: " + mPosition.ToString());

			EntityManager.I.AddColliderSubmission(new PlatformColliderSubmission(mRail.GetVelocity(gameTime), mPosition, mSize * Tile.sTILE_SIZE));

			mPosition = mRail.GetPosition();

			base.Update(gameTime);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the platform.
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			for(int i = 0; i < mSize; i++)
			{
				Vector2 pos = mPosition + new Vector2(i * Tile.sTILE_SIZE, 0.0f);

				MonoDraw.DrawTexture(info, mTexture, pos);
			}
		}



		/// <summary>
		/// Size of platform
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			return new Rect2f(mPosition, mPosition + new Vector2(mSize * Tile.sTILE_SIZE, 3.0f));
		}

		#endregion rDraw
	}
}


