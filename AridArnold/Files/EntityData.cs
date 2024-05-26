namespace AridArnold
{
	struct EntityData
	{
		public const int kClassSpacing = 2048;
		public const int kPlayerClassStart = 0 * kClassSpacing;
		public const int kEnemyClassStart = 1 * kClassSpacing;
		public const int kNPCClassStart = 2 * kClassSpacing;
		public const int kUtilityClassStart = 3 * kClassSpacing;

		public enum EntityClass
		{
			// Player
			kArnold = kPlayerClassStart,
			kAndrold,
			kPlayerClassEnd,

			// Enemy
			kTrundle = kEnemyClassStart,
			kRoboto,
			kFutronGun,
			kFutronRocket,
			kFarry,
			kMamal,
			kPapyras,
			kRanger,
			kEnemyClassEnd,

			//NPC
			kSimpleNPC = kNPCClassStart,
			kBickDogel,
			kFireBarrel,
			kNPCClassEnd,

			// Utility
			kArnoldSpawner = kUtilityClassStart,
			kSequenceDoor,
			kLevelLock,
			kShopDoor,
			kItemStand,
			kGravityOrb,
			kGravityTile,
			kTimeMachine,
			kPlantPot,
			kPillarPot,
			kWorldCabinet,
			kUtilityClassEnd
		}

		// Data
		public Point mPosition;
		public EntityClass mEntityClass;
		public WalkDirection mStartDirection;
		public CardinalDirection mGravityDirection;
		public float[] mFloatParams;
		public int[] mIntParams;

		// Only NPC data
		public string mTalkText;
		public string mHeckleText;
		public string mNPCDataPath;
	}
}
