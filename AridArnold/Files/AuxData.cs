namespace AridArnold
{
	class AuxData : MonoReadFile
	{
		#region rTypes

		public enum LevelType
		{
			CollectWater,
			CollectKey,
			Shop,
			Hub,
			Empty,
			Fountain,
		}

		#endregion rTypes





		#region rConstants

		const int FILE_VER = 5;
		const int NUM_PARAMS = 8;

		#endregion rConstants





		#region rMembers

		// Meta
		string mName;
		string mRoot;
		LevelType mLevelType;
		string mThemePath;
		string mBGPath;
		int[] mIntParams;

		// Entity
		List<LinearRailData> mRailDatas;
		List<EntityData> mEntityDatas;

		#endregion rMembers




		#region rInit

		public AuxData(string fileName) : base(fileName + ".aux", false)
		{
			// Meta
			mName = "";
			mRoot = "";
			mLevelType = LevelType.CollectWater;
			mThemePath = "";
			mBGPath = "";
			mIntParams = new int[NUM_PARAMS];

			// Entity
			mRailDatas = new List<LinearRailData>();
			mEntityDatas = new List<EntityData>();
		}

		#endregion rInit





		#region rRead

		/// <summary>
		/// Read all data for aux file.
		/// </summary>
		protected override void ReadBinary(BinaryReader br)
		{
			//Clear all data.
			ClearAll();

			int version = br.ReadInt32();
			if (version != FILE_VER)
			{
				throw new Exception("File version doesn't match. Verify data integrity!");
			}

			//Meta
			ReadMetadata(br);

			//Rails
			ReadRails(br);

			if (br.BaseStream.Position == br.BaseStream.Length)
			{
				return;
			}

			// Entities
			ReadEntities(br);
		}



		/// <summary>
		/// Read the meta data
		/// </summary>
		private void ReadMetadata(BinaryReader br)
		{
			mName = br.ReadString();
			mRoot = br.ReadString();
			mLevelType = (LevelType)br.ReadUInt32();
			mThemePath = br.ReadString();
			mBGPath = br.ReadString();
			for (int i = 0; i < NUM_PARAMS; i++)
			{
				mIntParams[i] = br.ReadInt32();
			}
		}



		/// <summary>
		/// Read linear rail data.
		/// </summary>
		private void ReadRails(BinaryReader br)
		{
			int railCount = br.ReadInt32();
			for (int i = 0; i < railCount; i++)
			{
				int size = br.ReadInt32();
				UInt32 flags = br.ReadUInt32();

				LinearRailData.RailType railType = ParseRailFlags(flags);

				LinearRailData linearRailData = new LinearRailData(size, railType);

				//Add nodes
				int nodeCount = br.ReadInt32();
				for (int j = 0; j < nodeCount; j++)
				{
					RailNode node = new RailNode();
					int ptX = br.ReadInt32();
					int ptY = br.ReadInt32();

					node.mPoint = new Point(ptX, ptY);
					node.mSpeed = br.ReadSingle();
					node.mWaitTime = br.ReadSingle();

					UInt32 nodeFlags = br.ReadUInt32();

					ParseNodeFlags(ref node, nodeFlags);

					linearRailData.AddNode(node);
				}

				mRailDatas.Add(linearRailData);
			}
		}



		/// <summary>
		/// Read linear rail data.
		/// </summary>
		private void ReadEntities(BinaryReader br)
		{
			int entityCount = br.ReadInt32();
			for (int i = 0; i < entityCount; i++)
			{
				EntityData entityData = new EntityData();
				entityData.mPosition.X = br.ReadInt32();
				entityData.mPosition.Y = br.ReadInt32();
				entityData.mEntityClass = (EntityData.EntityClass)(br.ReadUInt32());
				entityData.mStartDirection = (WalkDirection)(br.ReadUInt32());
				entityData.mGravityDirection = (CardinalDirection)(br.ReadUInt32());

				entityData.mFloatParams = new float[NUM_PARAMS];
				entityData.mIntParams = new int[NUM_PARAMS];

				for (int j = 0; j < NUM_PARAMS; j++)
				{
					entityData.mFloatParams[j] = br.ReadSingle();
					entityData.mIntParams[j] = br.ReadInt32();
				}

				if (entityData.mEntityClass == EntityData.EntityClass.kSimpleNPC)
				{
					entityData.mNPCDataPath = br.ReadString();
					entityData.mTalkText = br.ReadString();
					entityData.mHeckleText = br.ReadString();
				}

				mEntityDatas.Add(entityData);
			}
		}

		#endregion rRead





		#region rUtility

		/// <summary>
		/// Clear all elements from data.
		/// </summary>
		void ClearAll()
		{
			mRailDatas.Clear();
		}



		/// <summary>
		/// Parse rail flags
		/// </summary>
		/// <returns>Rail type from flags.</returns>
		LinearRailData.RailType ParseRailFlags(UInt32 flags)
		{
			if (flags == 0x1)
			{
				return LinearRailData.RailType.Cycle;
			}

			//Default
			return LinearRailData.RailType.BackAndForth;
		}



		/// <summary>
		/// Parse node flags.
		/// </summary>
		/// <returns>Node type</returns>
		void ParseNodeFlags(ref RailNode node, UInt32 flags)
		{
			//ANATOMY OF THE FLAGS:
			// bbbb_bbbb_bbbb_bbbb_bbbb_bbbb_bbbb_bbbb
			// NNNN_NNNN_NNNN_NNNN_NNNN_RRRR_TTTT_TTTT
			//   NOT SET                ROT   TYPE


			//Get type
			node.mType = (RailNode.NodeType)(flags & 0xFF);

			//Get direction
			UInt32 direction = MonoAlg.IntSubString(flags, 8, 4);

			switch (direction)
			{
				case 0:
					node.mDirection = CardinalDirection.Up;
					break;
				case 1:
					node.mDirection = CardinalDirection.Right;
					break;
				case 2:
					node.mDirection = CardinalDirection.Down;
					break;
				case 3:
					node.mDirection = CardinalDirection.Left;
					break;
			}
		}

		#endregion rUtility





		#region rAccess

		/// <summary>
		/// Get the mName member.
		/// </summary>
		public string GetName()
		{
			return mName;
		}





		/// <summary>
		/// Get the mRoot member.
		/// </summary>
		public string GetRoot()
		{
			return mRoot;
		}





		/// <summary>
		/// Get the mLevelType member.
		/// </summary>
		public LevelType GetLevelType()
		{
			return mLevelType;
		}





		/// <summary>
		/// Get the mThemePath member.
		/// </summary>
		public string GetThemePath()
		{
			return mThemePath;
		}





		/// <summary>
		/// Get the mBGPath member.
		/// </summary>
		public string GetBGPath()
		{
			return mBGPath;
		}





		/// <summary>
		/// Get the mIntParams member.
		/// </summary>
		public int[] GetIntParams()
		{
			return mIntParams;
		}



		public List<LinearRailData> GetRailsData()
		{
			return mRailDatas;
		}


		public List<EntityData> GetEntityData()
		{
			return mEntityDatas;
		}

		#endregion rAccess
	}
}
