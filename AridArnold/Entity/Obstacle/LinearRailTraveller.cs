namespace AridArnold
{
	/// <summary>
	/// Represents a node used by LinearRail
	/// </summary>
	struct RailNode
	{
		public enum NodeType
		{
			None = 0b0000_0000,
			HasPlatform = 0b0000_0001,
			HasSpikeBlock = 0b0000_0010,
		}

		public Point mPoint;
		public float mSpeed;
		public float mWaitTime;
		public NodeType mType;
		public CardinalDirection mDirection;
	}

	/// <summary>
	/// A rail that goes linearly from node to node.
	/// </summary>
	class LinearRailData
	{
		#region rTypes

		public enum RailType
		{
			BackAndForth,
			Cycle
		}

		#endregion rTypes





		#region rMembers

		//Rail specs
		int mSize;
		RailType mType;
		List<RailNode> mRailNodes;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Init Linear Rail
		/// </summary>
		public LinearRailData(int size, RailType type)
		{
			mSize = size;
			mType = type;
			mRailNodes = new List<RailNode>();
		}


		/// <summary>
		/// Add node to rail.
		/// </summary>
		public void AddNode(RailNode railNode)
		{
			mRailNodes.Add(railNode);
		}

		#endregion rInitialisation





		#region rUtil

		//Getters
		public int GetSize() { return mSize; }
		public RailType GetRailType() { return mType; }
		public int GetCount() { return mRailNodes.Count; }
		public RailNode GetNode(int i) { return mRailNodes[i]; }

		#endregion rUtil





		#region rFactory

		/// <summary>
		/// Loop through rail nodes and spawn appropriate entities on them
		/// </summary>
		public void ParseAllNodes()
		{
			for (int i = 0; i < mRailNodes.Count; i++)
			{
				ParseRailNode(i);
			}
		}



		/// <summary>
		/// Parse a single rail node and spawn the appropriate entities on this.
		/// </summary>
		private void ParseRailNode(int idx)
		{
			RailNode node = GetNode(idx);

			if (node.mType == RailNode.NodeType.None)
			{
				return;
			}

			RailTraveller railTraveller;

			switch (GetRailType())
			{
				case LinearRailData.RailType.BackAndForth:
					railTraveller = new BackAndForthLinearRailTraveller(idx, this);
					break;
				case LinearRailData.RailType.Cycle:
					railTraveller = new CycleLinearRailTraveller(idx, this);
					break;
				default:
					throw new NotImplementedException();
			}

			switch (node.mType)
			{
				case RailNode.NodeType.HasPlatform:
					EntityManager.I.RegisterEntity(new RailPlatform(railTraveller, node.mDirection, GetSize()));
					break;
				case RailNode.NodeType.HasSpikeBlock:
					EntityManager.I.RegisterEntity(new SpikeBlock(railTraveller));
					break;
				default:
					throw new NotImplementedException("Invalid node flags.");
			}


		}

		#endregion rFactory
	}



	/// <summary>
	/// A rail that goes linearly from node to node.
	/// </summary>
	abstract class LinearRailTraveller : RailTraveller
	{
		#region rMembers

		protected LinearRailData mData;

		protected int mPrevNode; //Which leg of the journey are we on?
		float mCurrentWaitTime;

		#endregion rMembers





		#region rInitialise

		/// <summary>
		/// Init Linear Rail
		/// </summary>
		public LinearRailTraveller(int startNode, LinearRailData data)
		{
			mPrevNode = startNode;
			mCurrentWaitTime = 0.0f;
			mPosition = GetNodePosition(data.GetNode(startNode));

			mData = data;
		}

		#endregion rInitialise





		#region rUpdate


		protected override void MoveRail(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);

			//Still waiting...
			if (mCurrentWaitTime > 0.0f)
			{
				mCurrentWaitTime -= dt;
				return;
			}

			int currentNodeIdx = GetCurrentNodeIdx();
			int nextNodeIdx = GetNextNodeIdx(currentNodeIdx);

			RailNode currentNode = mData.GetNode(currentNodeIdx);
			RailNode nextNode = mData.GetNode(nextNodeIdx);

			Vector2 startPos = GetNodePosition(currentNode);
			Vector2 endPos = GetNodePosition(nextNode);

			Vector2 legVector = endPos - startPos;
			Vector2 directionVector = Vector2.Normalize(legVector);

			mPosition += directionVector * currentNode.mSpeed * dt;

			float distanceFromStartSq = (mPosition - startPos).LengthSquared();

			// Leg complete
			if (distanceFromStartSq > legVector.LengthSquared())
			{
				ArriveAtNode(nextNodeIdx);
			}
		}





		/// <summary>
		/// Arrive at a node.
		/// </summary>
		/// <param name="index"></param>
		virtual protected void ArriveAtNode(int index)
		{
			mPrevNode = index;
			RailNode node = mData.GetNode(index);

			mPosition = GetNodePosition(node);
			mCurrentWaitTime = node.mWaitTime;
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw rail
		/// </summary>
		public override void Draw(DrawInfo info, Vector2 offset)
		{
			int curr = 0;

			while(curr < mData.GetCount() - 1)
			{
				int next = curr + 1;

				while(next < mData.GetCount() - 1 && mData.GetNode(curr).mWaitTime == 0)
				{
					next++;
				}

				Vector2 startPos = GetNodePosition(mData.GetNode(curr)) + offset;
				Vector2 endPos = GetNodePosition(mData.GetNode(next)) + offset;

				DrawSection(info, startPos, endPos);

				curr = next;
			}
		}



		/// <summary>
		/// Draw a section of the rail.
		/// </summary>
		public void DrawSection(DrawInfo info, Vector2 start, Vector2 end)
		{
			Color highlightCol = new Color(150, 150, 150);
			Color shawdowCol = new Color(50, 50, 50);
			const float DOT_SIZE = 1.0f;
			const float DOT_DIST = 6.0f;
			const float END_SIZE = 2.0f;


			// Draw end caps
			Rectangle startRect = MonoMath.SquareCenteredAt(start, END_SIZE);
			Rectangle endRect = MonoMath.SquareCenteredAt(end, END_SIZE);
			MonoDraw.DrawRectShadow(info, startRect, highlightCol, shawdowCol, 1.0f, DrawLayer.SubEntity);
			MonoDraw.DrawRectShadow(info, endRect, highlightCol, shawdowCol, 1.0f, DrawLayer.SubEntity);

			float sectionDist = (start - end).Length();
			int numDots = (int)MathF.Round(sectionDist / DOT_DIST);
			for(int i = 0; i < numDots; i++)
			{
				Vector2 dotPos = MonoMath.Lerp(start, end, (float)(i + 1) / (numDots + 1));
				Rectangle dotRect = MonoMath.SquareCenteredAt(dotPos, DOT_SIZE);
				MonoDraw.DrawRectShadow(info, dotRect, highlightCol, shawdowCol, 1.0f, DrawLayer.SubEntity);
			}
		}

		#endregion rDraw





		#region rUtil

		/// <summary>
		/// Get real world position of node.
		/// </summary>
		Vector2 GetNodePosition(RailNode node)
		{
			return TileManager.I.GetTileTopLeft(node.mPoint);
		}



		/// <summary>
		/// Get node at the start of our current leg.
		/// </summary>
		protected abstract int GetCurrentNodeIdx();



		/// <summary>
		/// Get node at the end of our current leg.
		/// </summary>
		protected abstract int GetNextNodeIdx(int index);

		#endregion rUtil
	}



	/// <summary>
	/// A linear rail that travels in a cycle.
	/// </summary>
	class CycleLinearRailTraveller : LinearRailTraveller
	{
		public CycleLinearRailTraveller(int startNode, LinearRailData data) : base(startNode, data)
		{
		}

		protected override int GetCurrentNodeIdx()
		{
			return mPrevNode;
		}

		protected override int GetNextNodeIdx(int index)
		{
			if (index < mData.GetCount() - 1)
			{
				return index + 1;
			}

			return 0;
		}
	}


	/// <summary>
	/// A linear rail that travels back and forth.
	/// </summary>
	class BackAndForthLinearRailTraveller : LinearRailTraveller
	{
		bool mGoingForwards;

		public BackAndForthLinearRailTraveller(int startNode, LinearRailData data) : base(startNode, data)
		{
			mGoingForwards = startNode != (data.GetCount() - 1);
		}

		protected override int GetCurrentNodeIdx()
		{
			return mPrevNode;
		}

		protected override int GetNextNodeIdx(int index)
		{
			int retIdx = index + (mGoingForwards ? 1 : -1);
			return retIdx;
		}

		protected override void ArriveAtNode(int index)
		{
			//When we reach 0, we go forwards. When we reach the end we go backwards.
			if (index == 0)
			{
				mGoingForwards = true;
			}
			else if (index == mData.GetCount() - 1)
			{
				mGoingForwards = false;
			}

			base.ArriveAtNode(index);
		}
	}
}
