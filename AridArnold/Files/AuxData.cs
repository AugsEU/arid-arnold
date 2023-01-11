namespace AridArnold
{
	class AuxData : MonoReadFile
	{
		#region rConstants

		const int FILE_VER = 1;

		#endregion rConstants





		#region rMembers

		List<LinearRailData> mRailDatas;

		#endregion rMembers




		#region rInit

		public AuxData(string fileName) : base(fileName + ".aux")
		{
			mRailDatas = new List<LinearRailData>();
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
				throw new Exception("File version doesn't match. Verify data integrity!");
			}

			//Rails
			ReadRails(br);
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
			UInt32 direction = Util.IntSubString(flags, 8, 4);

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

		#endregion rAccess
	}
}
