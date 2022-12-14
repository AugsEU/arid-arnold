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

		public AuxData(string fileName) : base(fileName)
		{
			mRailDatas = new List<LinearRailData>();
		}

		#endregion rInit





		#region rFile

		protected override string GetRelativeFolder()
		{
			return "Content\\Levels\\";
		}

		#endregion rFile





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

					node.mType = ParseNodeFlags(nodeFlags);

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
		RailNode.NodeType ParseNodeFlags(UInt32 flags)
		{
			RailNode.NodeType retVal = RailNode.NodeType.None;

			if ((flags & 0x1) != 0)
			{
				retVal |= RailNode.NodeType.HasPlatform;
			}

			return retVal;
		}

		#endregion rUtility


		#region rAccess

		List<LinearRailData> GetRailsData()
		{
			return mRailDatas;
		}

		#endregion rAccess
	}
}
