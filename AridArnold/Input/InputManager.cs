namespace AridArnold
{
	using Microsoft.Xna.Framework.Input;
	using InputBindingMap = Dictionary<InputAction, InputBindSet>;
	using InputBindingPair = KeyValuePair<InputAction, InputBindSet>;
	using InputBindingGangList = Dictionary<BindingGang, InputAction[]>;
	using System.Linq;

	/// <summary>
	/// Represents the types of actions an input can be bound to.
	/// </summary>
	enum InputAction
	{
		//System - These are not rebindable keys which are used for core menu navigation.
		SysUp,
		SysDown,
		SysLeft,
		SysRight,
		SysConfirm,

		SysLClick,
		SysRClick,

		//Arnold
		ArnoldUp,
		ArnoldDown,
		ArnoldLeft,
		ArnoldRight,
		ArnoldJump,
		UseItem,

		//Game
		RestartLevel,
		Confirm,
		Pause,

#if DEBUG
		// Debug
		SkipLevel,
#endif // DEBUG
	}



	/// <summary>
	/// A binding gang is a set of multiple actions that represent vaguely similar things. The reason is that we want to accept either SysUp or ArnoldUp for navigating menus
	/// </summary>
	enum BindingGang
	{
		//System - These are not rebindable keys which are used for core menu navigation.
		SysUp,
		SysDown,
		SysLeft,
		SysRight,
		SysConfirm,
	}

	/// <summary>
	/// Singleton that manages inputs.
	/// </summary>
	internal class InputManager : Singleton<InputManager>
	{
		#region rMembers

		MouseState mMouseState;
		InputBindingMap mInputBindings = new InputBindingMap();
		InputBindingGangList mBindingGangs = new InputBindingGangList();
		int mInputUpdateIndex = 0;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Init input manager
		/// </summary>
		public void Init()
		{
			//Set default bindings for now. TO DO: Load from file.
			SetDefaultBindings();

			mMouseState = new MouseState();
			mInputUpdateIndex = 0;
		}



		/// <summary>
		/// Set default bindings
		/// </summary>
		public void SetDefaultBindings()
		{
			// System
			SetBinding(InputAction.SysConfirm, new KeyBinding(Keys.Enter), new PadBinding(Buttons.A), new PadBinding(Buttons.Start));
			SetBinding(InputAction.SysUp, new KeyBinding(Keys.Up), new PadBinding(Buttons.DPadUp));
			SetBinding(InputAction.SysDown, new KeyBinding(Keys.Down), new PadBinding(Buttons.DPadDown));
			SetBinding(InputAction.SysLeft, new KeyBinding(Keys.Left), new PadBinding(Buttons.DPadLeft));
			SetBinding(InputAction.SysRight, new KeyBinding(Keys.Right), new PadBinding(Buttons.DPadRight));

			SetBinding(InputAction.SysLClick, new MouseBtnBinding(MouseButton.Left));
			SetBinding(InputAction.SysRClick, new MouseBtnBinding(MouseButton.Right));


			// Arnold
			SetBinding(InputAction.ArnoldUp, new KeyBinding(Keys.Up), new PadBinding(Buttons.DPadUp));
			SetBinding(InputAction.ArnoldDown, new KeyBinding(Keys.Down), new PadBinding(Buttons.DPadDown));
			SetBinding(InputAction.ArnoldLeft, new KeyBinding(Keys.Left), new PadBinding(Buttons.DPadLeft));
			SetBinding(InputAction.ArnoldRight, new KeyBinding(Keys.Right), new PadBinding(Buttons.DPadRight));

			SetBinding(InputAction.ArnoldJump, new KeyBinding(Keys.Space), new PadBinding(Buttons.A));

			// Game
			SetBinding(InputAction.Confirm, new KeyBinding(Keys.Enter), new PadBinding(Buttons.Y));
			SetBinding(InputAction.Pause, new KeyBinding(Keys.Escape), new PadBinding(Buttons.Start));
			SetBinding(InputAction.RestartLevel, new KeyBinding(Keys.R), new PadBinding(Buttons.Back));
			SetBinding(InputAction.UseItem, new KeyBinding(Keys.LeftShift), new PadBinding(Buttons.B));

#if DEBUG
			// Debug
			mInputBindings.Add(InputAction.SkipLevel, new InputBindSet(new KeyBinding(Keys.P)));
#endif // DEBUG

			DefineBindingGang(BindingGang.SysUp, InputAction.SysUp, InputAction.ArnoldUp);
			DefineBindingGang(BindingGang.SysDown, InputAction.SysDown, InputAction.ArnoldDown);
			DefineBindingGang(BindingGang.SysLeft, InputAction.SysLeft, InputAction.ArnoldLeft);
			DefineBindingGang(BindingGang.SysRight, InputAction.SysRight, InputAction.ArnoldRight);
			DefineBindingGang(BindingGang.SysConfirm, InputAction.SysConfirm, InputAction.ArnoldJump);
		}


		/// <summary>
		/// Overwrite an input action with a new set of bindings
		/// </summary>
		public void SetBinding(InputAction key, params InputBinding[] bindings)
		{
			mInputBindings.Add(key, new InputBindSet(bindings));
		}


		/// <summary>
		/// Define a binding gang. Never set by user
		/// </summary>
		public void DefineBindingGang(BindingGang gang, params InputAction[] actions)
		{
			mBindingGangs.Add(gang, actions);
		}

#endregion rInitialisation





		#region rKeySense

		/// <summary>
		/// Update Inputs
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public void Update(GameTime gameTime)
		{
			foreach (InputBindingPair keyBindPair in mInputBindings)
			{
				keyBindPair.Value.Update(gameTime);
			}

			mMouseState = Mouse.GetState();
			mInputUpdateIndex++;
		}



		/// <summary>
		/// Was a button pressed recently?
		/// </summary>
		/// <param name="key">Key to check</param>
		/// <returns>True if it was pressed in the last update</returns>
		public bool KeyPressed(InputAction key)
		{
			return mInputBindings[key].AnyKeyPressed();
		}



		/// <summary>
		/// Was a button pressed recently?
		/// </summary>
		/// <param name="key">Key to check</param>
		/// <returns>True if it was pressed in the last update</returns>
		public bool KeyHeld(InputAction key)
		{
			return mInputBindings[key].AnyKeyHeld();
		}



		/// <summary>
		/// Any inputs in this gang have been pressed in the last frame?
		/// </summary>
		public bool AnyGangPressed(BindingGang gang)
		{
			InputAction[] gangMembers = null;
			if(!mBindingGangs.TryGetValue(gang, out gangMembers))
			{
				return false;
			}

			foreach(var action in gangMembers)
			{
				if(KeyPressed(action))
				{
					return true;
				}
			}

			return false;
		}



		/// <summary>
		/// Get set of input bindings for a certain action type
		/// </summary>
		public InputBindSet GetInputBindSet(InputAction key)
		{
			return mInputBindings[key];
		}



		/// <summary>
		/// Get absolute mouse position, not accounting for scale.
		/// </summary>
		public Point GetMousePos()
		{
			Point screenPoint;

			Rectangle screenRect = Main.GetGameDrawArea();
			screenPoint.X = (mMouseState.Position.X - screenRect.Location.X);
			screenPoint.Y = (mMouseState.Position.Y - screenRect.Location.Y);

			return screenPoint;
		}



		/// <summary>
		/// Get mouse position in "world" units. Accounting for scale. Use this one if in doubt.
		/// </summary>
		public Vector2 GetMouseWorldPos(Camera relativeToCam = null)
		{
			Point mousePos = GetMousePos();

			Rectangle screenRect = Main.GetGameDrawArea();
			float scaleFactor = screenRect.Width / Screen.SCREEN_WIDTH;

			Vector2 posVec = new Vector2(mousePos.X, mousePos.Y);

			if (relativeToCam is not null)
			{
				CameraSpec spec = relativeToCam.GetCurrentSpec();

				posVec += spec.mPosition;
				scaleFactor *= spec.mZoom;
			}

			return posVec / scaleFactor;
		}



		/// <summary>
		/// Get mouse position in "world" units. Accounting for scale. Use this one if in doubt.
		/// </summary>
		public Vector2 GetMouseWorldPos(CameraManager.CameraInstance instance)
		{
			return GetMouseWorldPos(CameraManager.I.GetCamera(instance));
		}



		/// <summary>
		/// Is the mouse in this rectangle?
		/// </summary>
		public bool MouseInRect(Rect2f rect, CameraManager.CameraInstance instance = CameraManager.CameraInstance.ScreenCamera)
		{
			Vector2 mousePos = GetMouseWorldPos(instance);
			return Collision2D.BoxVsPoint(rect, mousePos);
		}



		/// <summary>
		/// Get mouse position in "world" units. Accounting for scale. Use this one if in doubt.
		/// </summary>
		public Point GetMouseWorldPoint(CameraManager.CameraInstance instance = CameraManager.CameraInstance.ScreenCamera)
		{
			Vector2 mousePos = GetMouseWorldPos(instance);
			return new Point((int)mousePos.X, (int)mousePos.Y);
		}



		/// <summary>
		/// Over how many frames have we made inputs?
		/// </summary>
		public int GetNumberOfInputFrames()
		{
			return mInputUpdateIndex;
		}

		#endregion rKeySense
	}
}
