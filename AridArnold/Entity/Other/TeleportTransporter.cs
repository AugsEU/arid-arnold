using Microsoft.Xna.Framework.Graphics;

namespace AridArnold
{
	// Entity that travels through teleportation pipes
	class TeleportTransporter : Entity
	{
		#region rTypes

		struct TraceInfo
		{
			public Texture2D mTexture;
			public Vector2 mPosition;
			public float mRotation;
			public SpriteEffects mSpriteEffects;
		}

		#endregion rTypes





		#region rConstants

		const int SEGMENT_DIVISIONS = 4;

		// How many "traces" do we leave behind? Includes the front one.
		const int TRACE_NUMBER = 3;

		static Color[] TRACE_COLORS =
		{
			new Color(0,81,72),
			new Color(0,61,54),
			new Color(7,33,30) 
		};

		const float SPEED_MULTIPLIER = 1.4025f;

		#endregion rConstants





		#region rMembers

		TraceInfo?[] mTraceHistory;
		Point mSegmentStart;
		Point mSegmentEnd;
		float mSegmentTimer;
		float mSegmentTotalTime;
		
		// The entity we are transporting.
		MovingEntity mTransportingEntity;
		float mInputSpeed;

		// Draw
		Texture2D mTraceTex1;
		Texture2D mTraceTex2;
		Texture2D mTraceTex3;

		#endregion rMembers





		#region rInitialistion

		/// <summary>
		/// Init teleport
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="tilePos"></param>
		public TeleportTransporter(Point tilePos, MovingEntity entityToTransport) : base(TileManager.I.GetTileTopLeft(tilePos))
		{
			mSegmentStart = tilePos;
			
			//Absorb entity
			mTransportingEntity = entityToTransport;
			mTransportingEntity.SetEnabled(false); // Hide entity as it travels through the pipe
			mInputSpeed = mTransportingEntity.GetPrevVelocity().Length();
			mSegmentTotalTime = Math.Clamp(10.0f / mInputSpeed, 0.5f, 2.0f);

			mSegmentTimer = -0.5f * mSegmentTotalTime;
			mTraceHistory = new TraceInfo?[TRACE_NUMBER];

			Point? nextPt = GetNextSegment(mSegmentStart, mSegmentStart);

			if(nextPt == null)
			{
				throw new Exception("Can't find next point to travel to.");
			}

			mSegmentEnd = nextPt.Value;
		}



		/// <summary>
		/// Load content for TeleportTransporter.
		/// </summary>
		public override void LoadContent()
		{
			mTraceTex1 = MonoData.I.MonoGameLoad<Texture2D>("Tiles/WW7/TeleportTrace1");
			mTraceTex2 = MonoData.I.MonoGameLoad<Texture2D>("Tiles/WW7/TeleportTrace2");
			mTraceTex3 = MonoData.I.MonoGameLoad<Texture2D>("Tiles/WW7/TeleportTrace3");

			mTraceHistory[0] = GenerateTraceAtHead(-2, mTraceTex3);
		}

		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Update teleport transporter.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);
			mSegmentTimer += dt;

			if (mSegmentTimer > mSegmentTotalTime)
			{
				Point? next = GetNextSegment(mSegmentStart, mSegmentEnd);

				if (next != null)
				{
					mSegmentTimer -= mSegmentTotalTime;
					mSegmentStart = mSegmentEnd;
					mSegmentEnd = next.Value;
				}
			}

			// Make sure to call this after updating the segment timer
			UpdateSegmentTraces(dt);

			base.Update(gameTime);
		}


		/// <summary>
		/// Segment traces for drawing.
		/// </summary>
		void UpdateSegmentTraces(float dt)
		{
			float oldSegTimer = mSegmentTimer - dt;
			int prevIdx = (int)MathF.Floor(SEGMENT_DIVISIONS * oldSegTimer / mSegmentTotalTime);
			int currIdx = (int)MathF.Floor(SEGMENT_DIVISIONS * mSegmentTimer / mSegmentTotalTime);

			if (prevIdx == currIdx)
			{
				return;
			}

			// Pop trace history back
			for (int i = mTraceHistory.Length - 1; i > 0; --i)
			{
				mTraceHistory[i] = mTraceHistory[i - 1];
			}

			Texture2D traceTex = mTraceTex1;

			if (currIdx == -1 || currIdx == SEGMENT_DIVISIONS)
			{
				traceTex = mTraceTex2;
			}
			else if(currIdx == SEGMENT_DIVISIONS + 1)
			{
				traceTex = mTraceTex3;
			}
			else if(currIdx > SEGMENT_DIVISIONS + 1)
			{
				mTraceHistory[0] = null;
				ShootOutEntity();
				return;
			}

			mTraceHistory[0] = GenerateTraceAtHead(currIdx, traceTex);
		}

		
		/// <summary>
		/// Shoot the entity out.
		/// </summary>
		void ShootOutEntity()
		{
			if (mTransportingEntity == null)
			{
				return;
			}

			Vector2 segStart = TileManager.I.GetTileCentre(mSegmentStart);
			Vector2 segEnd = TileManager.I.GetTileCentre(mSegmentEnd);
			Vector2 dir = segEnd - segStart;
			Vector2 output = segEnd + dir;

			mTransportingEntity.SetCentrePos(output);
			mTransportingEntity.OverrideVelocity(dir * ((mInputSpeed * SPEED_MULTIPLIER) / dir.Length()));

			if(mTransportingEntity is PlatformingEntity)
			{
				PlatformingEntity platformingEntity = (PlatformingEntity)mTransportingEntity;
				platformingEntity.SetGrounded(dir.Y < 0.0f && MathF.Abs(dir.X) < 0.001f);
			}

			// Release to the world.
			mTransportingEntity.SetEnabled(true);
			mTransportingEntity = null;
		}



		/// <summary>
		/// Get collider bounds
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			// Doesnt matter
			return new Rect2f(mPosition, mPosition);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw.
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			for(int i = 0; i < mTraceHistory.Length; ++i)
			{
				if (mTraceHistory[i] == null)
				{
					continue;
				}

				TraceInfo tracer = mTraceHistory[i].Value;

				MonoDraw.DrawTexture(info, tracer.mTexture, tracer.mPosition, null, TRACE_COLORS[i], tracer.mRotation, Vector2.Zero, 1.0f, tracer.mSpriteEffects, DrawLayer.TileEffects);
			}
		}

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Get next segment to go to.
		/// </summary>
		private Point? GetNextSegment(Point prev, Point start)
		{
			foreach(Point pt in MonoMath.GetAdjacentPoints(start))
			{
				if(pt == prev)
				{
					continue;
				}

				if(TileManager.I.GetTile(pt) is TeleportPipe)
				{
					return pt;
				}
			}

			return null;
		}



		/// <summary>
		/// Generate trace
		/// </summary>
		private TraceInfo GenerateTraceAtHead(int segIdx, Texture2D texture)
		{
			TraceInfo traceInfo = new TraceInfo();
			traceInfo.mTexture = texture;
			traceInfo.mSpriteEffects = SpriteEffects.None;

			Vector2 segStartPos = TileManager.I.GetTileCentre(mSegmentStart);
			Vector2 segEndPos = TileManager.I.GetTileCentre(mSegmentEnd);
			Vector2 mPosition = MonoMath.Lerp(segStartPos, segEndPos, (float)segIdx / (float)SEGMENT_DIVISIONS);

			bool horizontal = MathF.Abs(segStartPos.Y - segEndPos.Y) < 0.0001f;

			if(horizontal)
			{
				mPosition.Y -= Tile.sTILE_SIZE / 2.0f;
				traceInfo.mRotation = 0.0f;

				bool left = (segStartPos.X > segEndPos.X);
				if (left != (segIdx >= SEGMENT_DIVISIONS))
				{
					traceInfo.mSpriteEffects = SpriteEffects.FlipHorizontally;
				}

				if(left)
				{
					mPosition.X -= mTraceTex1.Width;
				}
			}
			else
			{
				bool up = (segStartPos.Y > segEndPos.Y);
				if (up != (segIdx >= SEGMENT_DIVISIONS))
				{
					traceInfo.mSpriteEffects = SpriteEffects.FlipHorizontally;
				}

				if (up)
				{
					mPosition.Y -= mTraceTex1.Width;
				}

				mPosition.X += Tile.sTILE_SIZE / 2.0f;
				traceInfo.mRotation = MathF.PI / 2.0f;
			}

			traceInfo.mPosition = mPosition;

			return traceInfo;
		}

		#endregion rUtility
	}
}
