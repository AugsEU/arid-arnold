using HorsesAndGun.Tiles;
using System;
using System.Collections.Generic;

namespace HorsesAndGun
{
	internal class TrackManager
	{
		public const int NUM_TRACKS = 5;
		public const int NUM_HORSES = 3;
		public const int NUM_TILES_PER_TRACK = 11;

		Vector2 TRACK_ORIGIN = new Vector2(83.0f, 28.0f);
		Vector2 TRACK_OFFSET = new Vector2(0.0f, 50.0f);
		Vector2 PADDING = new Vector2(4.0f, 0.0f);
		Vector2 TILE_OFFSET = new Vector2(50.0f, 0.0f);

		Vector2 mDynamicOffset = new Vector2(0.0f, 0.0f);

		const double START_FALL_TIME = 3500.0;
		const double END_FALL_TIME = 200.0;

		TrackTile[,] mTiles;
		ContentManager mContentManager;

		Horse[] mHorses;

		//Timings
		double mFallTime = START_FALL_TIME;
		MonoTimer mFallingTimer;

		double mShiftTime = END_FALL_TIME;
		MonoTimer mShiftTimer;

		MonoTimer mTotalTimer;

		public TrackManager(ContentManager _ContentManager)
		{
			mContentManager = _ContentManager;
		}

		public void Init()
		{
			mDynamicOffset = new Vector2(0.0f, 0.0f);
			mFallTime = START_FALL_TIME;
			mShiftTime = 1000.0;

			mTiles = new TrackTile[NUM_TRACKS, NUM_TILES_PER_TRACK];
			mHorses = new Horse[NUM_HORSES];

			//Timings
			mFallingTimer = new MonoTimer();
			mShiftTimer = new MonoTimer();
			mTotalTimer = new MonoTimer();

			mFallTime = START_FALL_TIME;
			mFallingTimer.FullReset();
			mShiftTimer.FullReset();

			for (int track = 0; track < NUM_TRACKS; track++)
			{
				for (int tile = 0; tile < NUM_TILES_PER_TRACK; tile++)
				{
					mTiles[track, tile] = CreateNextTrackTile(track);
				}
			}

			RemoveDoubleDeath();

			for (int horse = 0; horse < NUM_HORSES; horse++)
			{
				mHorses[horse] = new Horse(Vector2.Zero, 5, NUM_TRACKS - horse - 1);
				mTiles[NUM_TRACKS - horse - 1, 5] = new BasicTile(mContentManager);
				EntityManager.I.RegisterEntity(mHorses[horse], mContentManager);
			}

			SetHorsePositions();
		}

		private void RemoveDoubleDeath()
		{
			for (int track = 0; track < NUM_TRACKS; track++)
			{
				for (int tile = 1; tile < NUM_TILES_PER_TRACK; tile++)
				{
					if (mTiles[track, tile] is DeathTile && mTiles[track, tile - 1] is DeathTile)
					{
						mTiles[track, tile] = new BasicTile(mContentManager);
					}
				}
			}
		}

		public List<Vector2> GetGameOverPoints()
		{
			List<Vector2> retList = new List<Vector2>();

			foreach (Horse horse in mHorses)
			{
				if (horse.IsAlive() == false)
				{
					retList.Add(horse.GetEffectivePos());
				}
			}

			return retList;
		}

		public void Update(GameTime gameTime)
		{
			//Shift logic
			if (mTotalTimer.IsPlaying() == false)
			{
				mTotalTimer.Start();
			}

			mFallTime = START_FALL_TIME - mTotalTimer.GetElapsedMs() / 95.0;
			mFallTime = Math.Clamp(mFallTime, END_FALL_TIME, START_FALL_TIME);

			if (mFallingTimer.IsPlaying())
			{
				if (mFallingTimer.GetElapsedMs() > mFallTime)
				{
					BeginShift(gameTime);
				}
			}
			else if (!mShiftTimer.IsPlaying())
			{
				mFallingTimer.Start();
			}

			if (mShiftTimer.IsPlaying())
			{
				if (mShiftTimer.GetElapsedMs() > mShiftTime)
				{
					EndShift(gameTime);
				}
				else
				{
					UpdateShift(gameTime);
				}
			}

			//Update tiles
			for (int track = 0; track < NUM_TRACKS; track++)
			{
				for (int tile = 0; tile < NUM_TILES_PER_TRACK; tile++)
				{
					mTiles[track, tile].Update(gameTime);
				}
			}

			//Update horse positions
			SetHorsePositions();
			UpdateHorseOrders(gameTime);
			SetHorsePositions();

		}

		private void BeginShift(GameTime gameTime)
		{
			mFallingTimer.FullReset();
			mShiftTimer.FullReset();
			mShiftTimer.Start();

			for (int track = 0; track < NUM_TRACKS; track++)
			{
				for (int tile = 0; tile < NUM_TILES_PER_TRACK - 1; tile++)
				{
					mTiles[track, tile] = mTiles[track, tile + 1];
				}

				mTiles[track, NUM_TILES_PER_TRACK - 1] = CreateNextTrackTile(track);
			}

			for (int h = 0; h < NUM_HORSES; h++)
			{
				Horse horse = mHorses[h];
				horse.ShiftHorseBack();

				if (horse.GetReservedPoint().X < 0)
				{
					horse.Kill();
				}
			}

			RemoveDoubleDeath();

			mDynamicOffset = TILE_OFFSET;
		}

		private void UpdateShift(GameTime gameTime)
		{
			mDynamicOffset = Util.LerpVec(TILE_OFFSET, Vector2.Zero, (float)(mShiftTimer.GetElapsedMs() / mShiftTime));
		}

		private void EndShift(GameTime gameTime)
		{
			mDynamicOffset = Vector2.Zero;

			mFallingTimer.FullReset();
			mShiftTimer.FullReset();
			mFallingTimer.Start();
		}

		private void SetHorsePositions()
		{
			for (int h = 0; h < NUM_HORSES; h++)
			{
				Horse horse = mHorses[h];

				horse.position = GetTilePos(horse.TrackIndex, horse.TileIndex);
				horse.SetDestPosition(GetTilePos(horse.GetReservedPoint().Y, horse.GetReservedPoint().X));
			}
		}

		private void UpdateHorseOrders(GameTime gameTime)
		{
			for (int h = 0; h < NUM_HORSES; h++)
			{
				Horse horse = mHorses[h];

				if (horse.ReadyToMove())
				{
					HorseOrder orderToDo = horse.PopTopOrder();

					Point dest = MakeHorseOrderValid(horse, ref orderToDo);

					if (orderToDo.type != HorseOrderType.none)
					{
						horse.ExecuteOrder(orderToDo, dest);
						horse.ReserveTile(dest);
					}
				}
				else if (horse.FinishedMove())
				{
					horse.FinishOrder();

					TrackTile steppedOn = mTiles[horse.TrackIndex, horse.TileIndex];

					steppedOn.ApplyEffect(horse, this);

					mTiles[horse.TrackIndex, horse.TileIndex] = new BasicTile(mContentManager);
				}
			}
		}

		public Point MakeHorseOrderValid(Horse horse, ref HorseOrder order)
		{
			if (order.type == HorseOrderType.moveTile)
			{
				Point startPt = horse.GetCurrentPoint();
				Point destination = startPt + new Point(order.moveAmount, 0);
				destination.X = Math.Clamp(destination.X, 0, NUM_TILES_PER_TRACK - 1);

				if (destination.X == startPt.X)
				{
					order.type = HorseOrderType.none;
					return startPt;
				}

				while (IsPointReserved(destination))
				{
					destination.X -= Math.Sign(order.moveAmount);

					if (destination.X == startPt.X)
					{
						order.type = HorseOrderType.none;
						return startPt;
					}
				}

				order.moveAmount = destination.X - startPt.X;

				if (destination.X == startPt.X)
				{
					order.type = HorseOrderType.none;
					return startPt;
				}

				return destination;
			}
			else if (order.type == HorseOrderType.moveTrack)
			{
				Point startPt = horse.GetCurrentPoint();
				Point destination = startPt + new Point(0, order.moveAmount);
				destination.Y = Math.Clamp(destination.Y, 0, NUM_TRACKS - 1);

				if (destination.Y == startPt.Y)
				{
					order.type = HorseOrderType.none;
					return startPt;
				}

				while (IsPointReserved(destination))
				{
					destination.Y -= Math.Sign(order.moveAmount);

					if (destination.Y == startPt.Y)
					{
						order.type = HorseOrderType.none;
						return startPt;
					}
				}

				order.moveAmount = destination.Y - startPt.Y;

				if (destination.Y == startPt.Y)
				{
					order.type = HorseOrderType.none;
					return startPt;
				}

				return destination;
			}

			order.type = HorseOrderType.none;
			return horse.GetCurrentPoint();
		}

		private bool IsPointReserved(Point pt)
		{
			for (int h = 0; h < NUM_HORSES; h++)
			{
				Horse horse = mHorses[h];

				if (horse.GetReservedPoint() == pt)
				{
					return true;
				}
			}

			return false;
		}

		private TrackTile CreateNextTrackTile(int trackNum)
		{
			//Ensure baseline
			if (RandomManager.I.GetRandBool(15))
			{
				return new BasicTile(mContentManager);
			}

			//Plus tile
			if (RandomManager.I.GetRandBool(20))
			{
				int moveAmount = 1;
				switch (trackNum)
				{
					case 0: moveAmount = RandomManager.I.GetIntInRange(2, 3); break;
					case 1: moveAmount = RandomManager.I.GetIntInRange(2, 3); break;
					case 2: moveAmount = RandomManager.I.GetIntInRange(1, 3); break;
					case 3: moveAmount = RandomManager.I.GetIntInRange(1, 2); break;
					case 4: moveAmount = 1; break;
				}

				return new PlusTile(mContentManager, moveAmount);
			}

			//Up down
			if (RandomManager.I.GetRandBool(35))
			{
				bool goUp = true;
				switch (trackNum)
				{
					case 0: goUp = false; break;
					case 1: goUp = RandomManager.I.GetRandBool(20); break;
					case 2: goUp = RandomManager.I.GetRandBool(60); break;
					case 3: goUp = RandomManager.I.GetRandBool(90); break;
					case 4: goUp = true; break;
				}

				return new UpDownTile(mContentManager, goUp);
			}

			//Score
			if (RandomManager.I.GetRandBool(40))
			{
				bool forReal = true;
				switch (trackNum)
				{
					case 0: forReal = true; break;
					case 1: forReal = RandomManager.I.GetRandBool(95); break;
					case 2: forReal = RandomManager.I.GetRandBool(80); break;
					case 3: forReal = RandomManager.I.GetRandBool(70); break;
					case 4: forReal = RandomManager.I.GetRandBool(60); break;
				}

				if (forReal)
				{
					return new ScoreTile(mContentManager, RandomManager.I.GetRandBool(50));
				}
			}


			//Fast reload
			if (RandomManager.I.GetRandBool(40))
			{
				bool forReal = true;
				switch (trackNum)
				{
					case 0: forReal = true; break;
					case 1: forReal = RandomManager.I.GetRandBool(95); break;
					case 2: forReal = RandomManager.I.GetRandBool(80); break;
					case 3: forReal = RandomManager.I.GetRandBool(70); break;
					case 4: forReal = RandomManager.I.GetRandBool(60); break;
				}

				if (forReal)
				{
					return new FastReloadTile(mContentManager);
				}
			}

			//Death
			if (RandomManager.I.GetRandBool(50))
			{
				bool forReal = true;
				switch (trackNum)
				{
					case 0: forReal = RandomManager.I.GetRandBool(40); break;
					case 1: forReal = RandomManager.I.GetRandBool(50); break;
					case 2: forReal = RandomManager.I.GetRandBool(60); break;
					case 3: forReal = RandomManager.I.GetRandBool(90); break;
					case 4: forReal = true; break;
				}

				if (forReal)
				{
					return new DeathTile(mContentManager);
				}
			}

			return new BasicTile(mContentManager);
		}


		public void Draw(DrawInfo info)
		{
			for (int track = 0; track < NUM_TRACKS; track++)
			{
				for (int tile = 0; tile < NUM_TILES_PER_TRACK; tile++)
				{
					Texture2D tileTex = mTiles[track, tile].Draw(info);

					Vector2 tilePosition = GetTilePos(track, tile);

					info.spriteBatch.Draw(tileTex, tilePosition, Color.White);
				}
			}
		}

		public Vector2 GetTilePos(int track, int tile)
		{
			return mDynamicOffset + TRACK_ORIGIN + PADDING + track * TRACK_OFFSET + tile * TILE_OFFSET;
		}

		public Horse GetProjectedHorseHit(int track)
		{
			int closestHorseIdx = int.MaxValue;
			Horse bestHorse = null;

			for(int h = 0; h < mHorses.Length; h++)
			{
				Horse horse = mHorses[h];
				Point coord = horse.GetCurrentPoint();
			
				if(coord.Y == track)
				{
					if(bestHorse == null || coord.X < closestHorseIdx)
					{
						bestHorse = horse;
						closestHorseIdx = coord.X;
					}
				}
			}

			return bestHorse;
		}
	}
}
