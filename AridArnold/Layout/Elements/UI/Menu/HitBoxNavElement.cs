namespace AridArnold
{
	/// <summary>
	/// Layout element that we can navigate around with D-pad(for menus) and has a hitbox area.
	/// </summary>
	abstract class HitBoxNavElement : NavElement
	{
		// Number of pixels the mouse has to move to register as "unfrozen"
		const float FREEZE_UNLOCK_DIST = 3.0f;

		protected Vector2 mSize;

		// Keep track of this so that if the mouse isn't moving we don't still select things.
		Vector2 mMouseFreezePos = Vector2.Zero;

		// Has the mouse clicked us this frame?
		bool mHasMouseClick = false;

		protected HitBoxNavElement(XmlNode rootNode, Layout parent) : base(rootNode, parent)
		{
			mSize = new Vector2();
			mSize.X = MonoParse.GetFloat(rootNode["width"]);
			mSize.Y = MonoParse.GetFloat(rootNode["height"]);
		}

		protected HitBoxNavElement(string id, Vector2 pos, Vector2 size, Layout parent) : base(id, pos, parent)
		{
			mSize = size;
		}

		public override void Update(GameTime gameTime)
		{
			mHasMouseClick = false;

			Rect2f rect = new Rect2f(GetPosition(), mSize.X, mSize.Y);
			Vector2 mousePos = InputManager.I.GetMouseWorldPos(CameraManager.CameraInstance.ScreenCamera);
			bool mouseIntersect = Collision2D.BoxVsPoint(rect, mousePos);

			if(mouseIntersect)
			{
				mHasMouseClick = InputManager.I.KeyPressed(InputAction.SysLClick);
			}

			float distFromFreezePoint = (mousePos - mMouseFreezePos).Length();
			if (mMouseFreezePos == Vector2.Zero || distFromFreezePoint > FREEZE_UNLOCK_DIST)
			{
				if (mouseIntersect)
				{
					GetParent().SetSelectedElement(this);
				}

				mMouseFreezePos = mousePos;
			}

			base.Update(gameTime);
		}

		protected bool MouseClicked()
		{
			return mHasMouseClick;
		}

		protected void SetSize(Vector2 size)
		{
			mSize = size;
		}

		public Vector2 GetSize()
		{
			return mSize;
		}

		public Rect2f GetRect2f()
		{
			Vector2 min = GetPosition();
			Vector2 max = min + mSize;

			return new Rect2f(min, max);
		}
	}
}
