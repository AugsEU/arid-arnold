
using Microsoft.Xna.Framework;

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

		protected Vector2 mSize;

		List<HitBoxNavElement> mChildren;
		ScrollArrow mUpArrow;
		ScrollArrow mDownArrow;

		int mTopItemIndex = 0;
		int mNumChildDisplay = 0;

		string mUpNav;
		string mDownNav;

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
			mUpArrow.LinkDown(GenerateChildId(0, 2));
			mDownArrow.LinkUp(GenerateChildId(1, 2));
			GetParent().AddChildDirect(mUpArrow);
			GetParent().AddChildDirect(mDownArrow);

			Vector2 arrowPos = GetPosition();
			arrowPos.X += mSize.X * 0.5f;
			arrowPos.X -= mUpArrow.GetSize().X * 0.5f;
			arrowPos.Y -= ARROW_PADDING;
			
			mUpArrow.SetPos(new Vector2(arrowPos.X, arrowPos.Y - mUpArrow.GetSize().Y));

			arrowPos.Y += mSize.Y + ARROW_PADDING * 2.0f;
			mDownArrow.SetPos(arrowPos);

			mUpNav = MonoParse.GetString(rootNode["up"]);
			mDownNav = MonoParse.GetString(rootNode["down"]);
		}



		/// <summary>
		/// Link all children together. Call after adding all your elements
		/// </summary>
		public void LinkAllElements()
		{
			if(mChildren.Count >= 1)
			{
				mChildren[0].LinkUp(mUpNav);
				mChildren[mChildren.Count - 1].LinkDown(mDownNav);
			}

			for(int i = 1; i < mChildren.Count - 1; i++)
			{
				HitBoxNavElement prevElement = mChildren[i - 1];
				HitBoxNavElement nextElement = mChildren[i + 1];

				mChildren[i].Link(prevElement, nextElement);
			}
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update the scroll list
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			CalculateTopIndex();
			PositionElements();

			for (int i = 0; i < mChildren.Count; i++)
			{
				mChildren[i].SetEnabled(mTopItemIndex <= i && i < mTopItemIndex + mNumChildDisplay);
			}

			mUpArrow.Update(gameTime);
			mDownArrow.Update(gameTime);

			base.Update(gameTime);
		}



		/// <summary>
		/// Calculate the indicies of the top/bottom most visible elements
		/// </summary>
		void CalculateTopIndex()
		{
			if(mChildren.Count == 0)
			{
				mDownArrow.SetVisible(false);
				mUpArrow.SetVisible(false);
				return;
			}

			if(mNumChildDisplay == 0)
			{
				RecalculateNumShown();
			}

			int selectedItem = GetSelectedChildIdx();

			if(selectedItem != -1)
			{
				if(selectedItem < mTopItemIndex)
				{
					mTopItemIndex = selectedItem;
					RecalculateNumShown();
				}
				else
				{
					while(selectedItem >= mTopItemIndex + mNumChildDisplay)
					{
						mTopItemIndex++;
						RecalculateNumShown();
					}
				}
			}

			mDownArrow.SetEnabled(mTopItemIndex + mNumChildDisplay < mChildren.Count);
			mUpArrow.SetEnabled(mTopItemIndex > 0);
		}



		/// <summary>
		/// Based off the top index, how many should we be showing?
		/// </summary>
		void RecalculateNumShown()
		{
			float yCursor = 0.0f;
			int numShown = 0;
			for(int i = mTopItemIndex; i < mChildren.Count; i++)
			{
				Vector2 childSize = mChildren[i].GetSize();
				yCursor += childSize.Y;
				if(yCursor > mSize.Y)
				{
					break;
				}
				numShown++;
			}

			mNumChildDisplay = numShown;
		}



		/// <summary>
		/// Get selected child
		/// </summary>
		int GetSelectedChildIdx()
		{
			for(int i = 0; i < mChildren.Count; i++)
			{
				if (mChildren[i].IsSelected())
				{
					return i;
				}
			}

			return -1;
		}



		/// <summary>
		/// Position the elements we can see.
		/// </summary>
		void PositionElements()
		{
			if(mChildren.Count == 0)
			{
				return;
			}

			float centreX = mSize.X * 0.5f;

			float totalHeight = 0.0f;
			for (int i = mTopItemIndex; i < mTopItemIndex + mNumChildDisplay; i++)
			{
				totalHeight += mChildren[i].GetSize().Y;
			}

			float spacing = (mSize.Y - totalHeight) / (mNumChildDisplay + 1);

			float yCursor = spacing;
			for (int i = mTopItemIndex; i < mTopItemIndex + mNumChildDisplay; i++)
			{
				Vector2 elementSize = mChildren[i].GetSize();
				Vector2 relPos = new Vector2(centreX - elementSize.X * 0.5f, yCursor);

				mChildren[i].SetPos(relPos + GetPosition());

				yCursor += elementSize.Y;
				yCursor += spacing;
			}
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the elements
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			for (int i = mTopItemIndex; i < mTopItemIndex + mNumChildDisplay; i++)
			{
				DrawChildElementBox(info, mChildren[i]);
			}

			//MonoDebug.AddDebugRect(new Rect2f(GetPosition(), mSize.X, mSize.Y), new Color(255, 0, 0, 100));

			base.Draw(info);
		}



		/// <summary>
		/// Draw a child element with a border
		/// </summary>
		void DrawChildElementBox(DrawInfo info, HitBoxNavElement element)
		{
			Color shadowColor = GetColor() * 0.2f;
			Vector2 shadowDisp = new Vector2(2.0f, 2.0f);
			MonoDraw.DrawRectHollow(info, element.GetRect2f(), 2.0f, GetColor(), shadowColor, shadowDisp, GetDepth());
		}

		#endregion rDraw





		#region rUtil

		/// <summary>
		/// Add a child to our list
		/// </summary>
		protected void AddChild(HitBoxNavElement child)
		{
			child.SetEnabled(false);
			mChildren.Add(child);
			GetParent().AddChildDirect(child);
		}


		/// <summary>
		/// Scroll this list.
		/// </summary>
		public void ScrollList(int delta)
		{
			mTopItemIndex += delta;
			mTopItemIndex = Math.Clamp(mTopItemIndex, 0, mChildren.Count);
		}



		/// <summary>
		/// Generate a standard child ID
		/// </summary>
		protected string GenerateChildId(int index, int count)
		{
			string baseId = GetID();

			if (index == 0)
			{
				return string.Format("{0}Top", baseId);
			}
			else if(index == count - 1)
			{
				return string.Format("{0}Bottom", baseId);
			}

			return string.Format("{0}{1}", baseId, index);
		}

		#endregion rUtil
	}
}
