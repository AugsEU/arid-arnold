namespace AridArnold
{
	internal class ElectricGate : SquareTile
	{
		#region rTypes

		enum GateStatus
		{
			Closed,
			Mid,
			Open
		}

		#endregion rTypes





		#region rMembers

		Texture2D mOpenTexture;
		Texture2D mMidTexture;

		GateStatus mStatus;

		#endregion rMembers



		#region rInitialisation

		/// <summary>
		/// Init electric gate at position.
		/// </summary>
		public ElectricGate(Vector2 pos) : base(pos)
		{
			mStatus = GateStatus.Closed;
		}



		/// <summary>
		/// Load content for electric gate.
		/// </summary>
		public override void LoadContent(ContentManager content)
		{
			mTexture = content.Load<Texture2D>("Tiles/Lab/gate-closed");
			mMidTexture = content.Load<Texture2D>("Tiles/Lab/gate-mid");
			mOpenTexture = content.Load<Texture2D>("Tiles/Lab/gate-open");
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update electric gate.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			//Electricity
			EMField emField = TileManager.I.GetEMField();

			//Check surrounding tiles.
			float totalElectric = 0.0f;

			for (int i = 0; i < ADJACENT_COORDS.Length; i++)
			{
				totalElectric += emField.GetValue(mTileMapIndex, ADJACENT_COORDS[i]).mElectric;
			}

			if(totalElectric > 0.75f)
			{
				mStatus = GateStatus.Open;
			}
			else if(totalElectric > 0.25f)
			{
				mStatus = GateStatus.Mid;
			}
			else
			{
				mStatus = GateStatus.Closed;
			}
		}



		/// <summary>
		/// Collide with entities except when the gate is open.
		/// </summary>
		public override CollisionResults Collide(MovingEntity entity, GameTime gameTime)
		{
			if(mStatus == GateStatus.Open)
			{
				return CollisionResults.None;
			}

			return base.Collide(entity, gameTime);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get appropriate gate texture.
		/// </summary>
		public override Texture2D GetTexture()
		{
			switch (mStatus)
			{
				case GateStatus.Closed:
					return mTexture;
				case GateStatus.Mid:
					return mMidTexture;
				case GateStatus.Open:
					return mOpenTexture;
				default:
					break;
			}

			throw new NotImplementedException();
		}

		#endregion rDraw
	}
}
