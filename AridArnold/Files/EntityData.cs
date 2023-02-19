namespace AridArnold
{
	struct EntityData
	{
		public const int kClassSpacing = 2048;
		public const int kPlayerClassStart = 0 * kClassSpacing;
		public const int kEnemyClassStart = 1 * kClassSpacing;
		public const int kNPCClassStart = 2 * kClassSpacing;

		public enum EntityClass
		{
			// Player
			kArnold = kPlayerClassStart,
			kAndrold,
			kPlayerClassEnd,

			// Enemy
			kTrundle = kEnemyClassStart,
			kRoboto,
			kEnemyClassEnd,

			//NPC
			kBarbara = kNPCClassStart,
			kZippy,
			kDok,
			kBickDogel,
			kElectrent,
			kNPCClassEnd,
		}

		public enum EntityType
		{
			kBasic,
			kSimpleNPC
		}

		// Data
		public Point mPosition;
		public EntityClass mEntityClass;
		public WalkDirection mStartDirection;
		public CardinalDirection mGravityDirection;

		// Only NPC data
		public string mTalkText;
		public string mHeckleText;

		public EntityType GetEntityType()
		{
			if ((int)mEntityClass >= kNPCClassStart)
			{
				if(mEntityClass == EntityClass.kBickDogel)
				{
					//Special exception
					return EntityType.kBasic;
				}

				return EntityType.kSimpleNPC;
			}
			return EntityType.kBasic;
		}
	}
}
