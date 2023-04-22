namespace AridArnold
{
	class AuxData : MonoReadFile
	{
		#region rConstants

		const int FILE_VER = 3;
		const int NUM_ENTITY_PARAMS = 8;

		#endregion rConstants





		#region rMembers

		List<LinearRailData> mRailDatas;
		List<EntityData> mEntityDatas;

		#endregion rMembers




		#region rInit

		public AuxData(string fileName) : base(fileName + ".aux", false)
		{
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
			if(version != FILE_VER)
			{
				MonoDebug.Break();
				throw new Exception("File version doesn't match. Verify data integrity!");
			}

			//Rails
			ReadRails(br);

			if(br.BaseStream.Position == br.BaseStream.Length)
			{
				return;
			}

			// Entities
			ReadEntities(br);
		}



		/// <summary>
		/// Read linear rail data.
		/// </summary>
		private void ReadRails(BinaryReader br)
		{
			int railCount = br.ReadInt32();
			for(int i = 0; i < railCount; i++)
			{
				int size = br.ReadInt32();
				UInt32 flags = br.ReadUInt32();

				LinearRailData.RailType railType = ParseRailFlags(flags);

				LinearRailData linearRailData = new LinearRailData(size, railType);

				//Add nodes
				int nodeCount = br.ReadInt32();
				for(int j = 0; j < nodeCount; j++)
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

				entityData.mFloatParams = new float[NUM_ENTITY_PARAMS];
				entityData.mIntParams = new int[NUM_ENTITY_PARAMS];

				for(int j = 0; j < NUM_ENTITY_PARAMS; j++)
				{
					entityData.mFloatParams[j] = br.ReadSingle();
					entityData.mIntParams[j] = br.ReadInt32();
				}

				if (entityData.GetEntityType() == EntityData.EntityType.kSimpleNPC)
				{
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
			if(flags == 0x1)
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
			node.mType = RailNode.NodeType.None;

			if ((flags & 0x1) != 0)
			{
				node.mType |= RailNode.NodeType.HasPlatform;
			}

			//Get direction
			UInt32 direction = MonoAlg.IntSubString(flags, 8, 4);

			switch(direction)
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
