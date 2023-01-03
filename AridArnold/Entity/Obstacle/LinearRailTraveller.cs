namespace AridArnold
{
	/// <summary>
	/// Represents a node used by LinearRail
	/// </summary>
	struct RailNode
	{
		public enum NodeType
		{
			None,
			HasPlatform = 0b0000_0001
		}

		public Point mPoint;
		public float mSpeed;
		public float mWaitTime;
		public NodeType mType;
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
		public RailNode GetNode(int i) {  return mRailNodes[i]; }

		#endregion rUtil
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
			int nextNodeIdx = GetNextNodeIdx();

			RailNode currentNode = mData.GetNode(currentNodeIdx);
			RailNode nextNode = mData.GetNode(nextNodeIdx);

			Vector2 startPos = GetNodePosition(currentNode);
			Vector2 endPos = GetNodePosition(nextNode);

			Vector2 legVector = endPos - startPos;
			Vector2 directionVector = Vector2.Normalize(legVector);

			mPosition += directionVector * currentNode.mSpeed * dt;

			float distanceFromStartSq = (mPosition - startPos).LengthSquared();

			// Leg complete
			if(distanceFromStartSq > legVector.LengthSquared())
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
		protected abstract int GetNextNodeIdx();

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

		protected override int GetNextNodeIdx()
		{
			if(mPrevNode < mData.GetCount() - 1)
			{
				return mPrevNode + 1;
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

		protected override int GetNextNodeIdx()
		{
			if (mGoingForwards)
			{
				return mPrevNode + 1;
			}

			return mPrevNode - 1;
		}

		protected override void ArriveAtNode(int index)
		{
			//When we reach 0, we go forwards. When we reach the end we go backwards.
			if(index == 0)
			{
				mGoingForwards = true;
			}
			else if(index == mData.GetCount() - 1)
			{
				mGoingForwards = false;
			}

			base.ArriveAtNode(index);
		}
	}
}
