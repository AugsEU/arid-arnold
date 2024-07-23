
namespace AridArnold
{
	class MenuScrollList : LayElement
	{
		#region rConstants

		const float DEFAULT_WIDTH  = 800.0f;
		const float DEFAULT_HEIGHT = 400.0f;
		const float ARROW_PADDING = 5.0f;

		#endregion rConstants


		#region rMembers

		Vector2 mSize;

		List<HitBoxNavElement> mChildren;
		ScrollArrow mUpArrow;
		ScrollArrow mDownArrow;

		int mSelectedIndex = 0;

		int mTopItemIndex = 0;
		int mBottomItemIndex = 0;
		float mItemSpacing = -1.0f;

		#endregion rMembers


		#region rInit

		/// <summary>
		/// Create menu scroll list
		/// </summary>
		public MenuScrollList(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			mSize.X = MonoParse.GetFloat(rootNode["width"], DEFAULT_WIDTH);
			mSize.Y = MonoParse.GetFloat(rootNode["height"], DEFAULT_HEIGHT);

			mChildren = new List<HitBoxNavElement>();

			mUpArrow = new ScrollArrow(rootNode, parent, this, true);
			mDownArrow = new ScrollArrow(rootNode, parent, this, false);

			Vector2 arrowPos = GetPosition();
			arrowPos.X += mSize.X * 0.5f;
			arrowPos.Y -= ARROW_PADDING;
			mUpArrow.SetPos(arrowPos);

			arrowPos.Y += mSize.Y + ARROW_PADDING * 2.0f;
			mDownArrow.SetPos(arrowPos);
		}

		#endregion rInit



		#region rUpdate

		/// <summary>
		/// Update the scroll list
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			CalculateTopAndBottomIdx();
			PositionElements();

			for (int i = mTopItemIndex; i <= mBottomItemIndex; i++)
			{
				mChildren[i].Update(gameTime);
			}

			base.Update(gameTime);
		}



		/// <summary>
		/// Calculate the indicies of the top/bottom most visible elements
		/// </summary>
		void CalculateTopAndBottomIdx()
		{
			if(mSelectedIndex < mTopItemIndex)
			{
				// Bounded by top
				mTopItemIndex = mSelectedIndex;

				// Work our way down to figure this out.
				mBottomItemIndex = IterateUntilFull(mTopItemIndex, 1);
			}
			else if(mSelectedIndex > mBottomItemIndex)
			{
				// Bounded by bottom
				mBottomItemIndex = mSelectedIndex;

				// Work our way up
				mTopItemIndex = IterateUntilFull(mBottomItemIndex, -1);
			}
		}



		/// <summary>
		/// Utility method to calculate indices
		/// </summary>
		int IterateUntilFull(int startIdx, int step)
		{
			float yCursor = mChildren[startIdx].GetSize().Y;
			int returnIdx = startIdx;

			int numIter = 0;
			while (numIter++ < 500)
			{
				int nextIdx = returnIdx + step;
				if (nextIdx < 0 || nextIdx >= mChildren.Count)
				{
					break;
				}

				yCursor += mChildren[nextIdx].GetSize().Y;
				if(yCursor > mSize.Y)
				{
					break;
				}

				returnIdx = nextIdx;
			}
			MonoDebug.Assert(numIter < 500, "Scroll area logic error");

			return returnIdx;
		}



		/// <summary>
		/// Position the elements we can see.
		/// </summary>
		void PositionElements()
		{
			float centreX = mPos.X + mSize.X * 0.5f;

			int elementsToDisplay = mBottomItemIndex - mTopItemIndex + 1;
			float totalHeight = 0.0f;
			for (int i = mTopItemIndex; i <= mBottomItemIndex; i++)
			{
				totalHeight += mChildren[i].GetSize().Y;
			}

			float spacing = (mSize.Y - totalHeight) / (elementsToDisplay + 1);

			float yCursor = spacing;
			for (int i = mTopItemIndex; i <= mBottomItemIndex; i++)
			{
				Vector2 elementSize = mChildren[i].GetSize();
				Vector2 relPos = new Vector2(centreX - elementSize.X, yCursor);

				mChildren[i].SetPos(relPos + GetPosition());

				yCursor += elementSize.Y;
				yCursor += spacing;
			}
		}

		#endregion rUpdate


		#region rUtil

		/// <summary>
		/// Increment selected element
		/// </summary>
		public void IncrementSelected(int increment)
		{
			if(GetParent().IsSelectionBlocked())
			{
				return;
			}
			mSelectedIndex += increment;
			mSelectedIndex = Math.Clamp(mSelectedIndex, 0, mChildren.Count - 1);
		}



		/// <summary>
		/// Add a child to our list
		/// </summary>
		protected void AddChild(HitBoxNavElement child)
		{
			mChildren.Add(child);
		}

		#endregion rUtil
	}
}
