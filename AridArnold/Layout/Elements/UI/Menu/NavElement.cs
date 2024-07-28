

using static AridArnold.EMField;

namespace AridArnold
{
	/// <summary>
	/// Layout element that we can navigate around with D-pad(for menus)
	/// </summary>
	abstract class NavElement : LayElement
	{
		public enum NavDir
		{
			kUp = 0,
			kDown,
			kLeft,
			kRight
		}

		// Elements we can navigate to
		protected string[] mNavIDs;

		protected bool mBlockedOut;

		public NavElement(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			mNavIDs = new string[4];

			mNavIDs[(int)NavDir.kUp] = MonoParse.GetString(rootNode["up"]);
			mNavIDs[(int)NavDir.kDown] = MonoParse.GetString(rootNode["down"]);
			mNavIDs[(int)NavDir.kLeft] = MonoParse.GetString(rootNode["left"]);
			mNavIDs[(int)NavDir.kRight] = MonoParse.GetString(rootNode["right"]);

			// Select me if none selected.
			NavElement selectedElement = parent.GetSelectedElement();
			if(selectedElement is null)
			{
				parent.SetSelectedElement(this);
			}
		}

		public NavElement(string id, Vector2 pos, Layout parent) : base(id, pos, parent)
		{
			mNavIDs = new string[4];
			for (int i = 0; i < mNavIDs.Length; i++)
			{
				mNavIDs[i] = "";
			}
		}


		public override void Update(GameTime gameTime)
		{
			CheckNavigation(BindingGang.SysUp, NavDir.kUp);
			CheckNavigation(BindingGang.SysDown, NavDir.kDown);
			CheckNavigation(BindingGang.SysLeft, NavDir.kLeft);
			CheckNavigation(BindingGang.SysRight, NavDir.kRight);

			base.Update(gameTime);
		}

		void CheckNavigation(BindingGang gang, NavDir dir)
		{
			string navElementID = mNavIDs[(int)dir];
			if (!IsSelected() || navElementID.Length == 0)
			{
				// Not selected, don't do nav
				return;
			}

			if (InputManager.I.AnyGangPressed(gang))
			{
				NavElement nextElement = (NavElement)GetParent().GetElementByID(navElementID);
				
				while (nextElement is not null && nextElement.mBlockedOut)
				{
					nextElement = nextElement.GetLinkedElement(dir);
				}

				if (nextElement is not null)
				{
					GetParent().SetSelectedElement(nextElement);
				}
			}
		}

		public NavElement GetLinkedElement(NavDir dir)
		{
			string strID = mNavIDs[(int)dir];
			if(strID.Length == 0)
			{
				return null;
			}

			return (NavElement)GetParent().GetElementByID(strID);
		}

		public bool IsSelected()
		{
			return object.ReferenceEquals(GetParent().GetSelectedElement(), this);
		}

		public bool IsBlockedOut()
		{
			return mBlockedOut;
		}

		public void Link(NavDir navDir, string id)
		{
			mNavIDs[(int)navDir] = id;
		}

		public void LinkUpDown(NavElement up, NavElement down)
		{
			mNavIDs[(int)NavDir.kUp] = up is not null ? up.GetID() : "";
			mNavIDs[(int)NavDir.kDown] = down is not null ? down.GetID() : "";
		}
	}
}
