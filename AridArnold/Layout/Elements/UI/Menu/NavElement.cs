

namespace AridArnold
{
	/// <summary>
	/// Layout element that we can navigate around with D-pad(for menus)
	/// </summary>
	abstract class NavElement : LayElement
	{
		// Elements we can navigate to
		string mUpNavID;
		string mDownNavID;
		string mLeftNavID;
		string mRightNavID;

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
			NavElement navElement = FindNavElement(navElementID);
			if (InputManager.I.AnyGangPressed(gang) && navElement is not null)
			{
				SetSelected(navElement);
			}
		}

		public bool IsSelected()
		{
			return object.ReferenceEquals(GetParent().GetSelectedElement(), this);
		}

		NavElement FindNavElement(string navID)
		{
			if(navID is null || navID.Length == 0)
			{
				return null;
			}

			LayElement elementByID = GetParent().GetElementByID(navID);

			return (NavElement)elementByID; // Note: Unsafe cast because we want to crash if the type is wrong.
		}

		protected void SetSelected(NavElement navElement)
		{
			GetParent().SetSelectedElement(navElement);
		}
	}
}
