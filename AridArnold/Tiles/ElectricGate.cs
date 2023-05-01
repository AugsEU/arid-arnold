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
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/GateClosed");
			mMidTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/GateMid");
			mOpenTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/GateOpen");
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update electric gate.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			//Check surrounding tiles.
			EMField.ScanResults scan = TileManager.I.GetEMField().ScanAdjacent(mTileMapIndex);

			if(scan.mTotalPositiveElectric > 0.75f)
			{
				mStatus = GateStatus.Open;
			}
			else if(scan.mTotalPositiveElectric > 0.25f)
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


		public override bool IsSolid()
		{
			if(mStatus == GateStatus.Open)
			{
				return false;
			}

			return true;
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
