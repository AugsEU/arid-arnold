

namespace AridArnold
{
	/// <summary>
	/// Layout element that we can navigate around with D-pad(for menus)
	/// </summary>
	abstract class NavElement : LayElement
	{
		// Elements we can navigate to
		protected string mUpNavID;
		protected string mDownNavID;
		protected string mLeftNavID;
		protected string mRightNavID;

		public NavElement(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			mUpNavID = MonoParse.GetString(rootNode["up"]);
			mDownNavID = MonoParse.GetString(rootNode["down"]);
			mLeftNavID = MonoParse.GetString(rootNode["left"]);
			mRightNavID = MonoParse.GetString(rootNode["right"]);

			// Select me if none selected.
			NavElement selectedElement = parent.GetSelectedElement();
			if(selectedElement is null)
			{
				parent.SetSelectedElement(this);
			}
		}

		public NavElement(string id, Vector2 pos, Layout parent) : base(id, pos, parent)
		{
			mUpNavID = "";
			mDownNavID = "";
			mLeftNavID = "";
			mRightNavID = "";
		}


		public override void Update(GameTime gameTime)
		{
			CheckNavigation(BindingGang.SysUp, mUpNavID);
			CheckNavigation(BindingGang.SysDown, mDownNavID);
			CheckNavigation(BindingGang.SysLeft, mLeftNavID);
			CheckNavigation(BindingGang.SysRight, mRightNavID);

			base.Update(gameTime);
		}

		void CheckNavigation(BindingGang gang, string navElementID)
		{
			if(!IsSelected() || navElementID.Length == 0)
			{
				// Not selected, don't do nav
				return;
			}

			if (InputManager.I.AnyGangPressed(gang))
			{
				GetParent().SetSelectedElement(navElementID);
			}
		}

		public bool IsSelected()
		{
			return object.ReferenceEquals(GetParent().GetSelectedElement(), this);
		}

		public void LinkUp(string upId)
		{
			mUpNavID = upId;
		}

		public void LinkDown(string downId)
		{
			mDownNavID = downId;
		}

		public void LinkLeft(string leftId)
		{
			mLeftNavID = leftId;
		}

		public void LinkRight(string rightId)
		{
			mRightNavID = rightId;
		}

		public void Link(NavElement up, NavElement down)
		{
			up.LinkDown(GetID());
			mUpNavID = up.GetID();

			mDownNavID = down.GetID();
			down.LinkUp(GetID());
		}
	}
}
