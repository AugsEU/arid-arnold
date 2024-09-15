namespace AridArnold
{
	using Microsoft.Xna.Framework.Input;
	using InputBindingMap = Dictionary<InputAction, InputBindSet>;
	using InputBindingPair = KeyValuePair<InputAction, InputBindSet>;
	using InputBindingGangList = Dictionary<BindingGang, InputAction[]>;
	using System.Linq;
	using Microsoft.Xna.Framework;

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

		SysFullScreen,

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
		UInt64 mInputUpdateIndex = 0;

		/// States used only for rebinding purposes. Not gameplay related.
		KeyboardState mKeyboardState;
		KeyboardState mPrevKeyboardState;

		GamePadState[] mGamePadStates;
		GamePadState[] mPrevGamePadStates;

		List<InputBindingType> mInputOrder;

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

			mKeyboardState = new KeyboardState();
			mPrevKeyboardState = new KeyboardState();

			mGamePadStates = new GamePadState[] { new GamePadState() , new GamePadState(), new GamePadState(), new GamePadState() };
			mPrevGamePadStates = new GamePadState[] { new GamePadState(), new GamePadState(), new GamePadState(), new GamePadState() };

			mInputOrder = new List<InputBindingType>
			{
				InputBindingType.kKeyboard,
				InputBindingType.kGamepad,
				InputBindingType.kMouse
			};
		}



		/// <summary>
		/// Set default bindings
		/// </summary>
		public void SetDefaultBindings()
		{
			mInputBindings.Clear();
			mBindingGangs.Clear();

			// System
			SetBinding(InputAction.SysConfirm, new KeyBinding(Keys.Enter), new PadBinding(Buttons.A), new PadBinding(Buttons.Start));
			SetBinding(InputAction.SysUp, new KeyBinding(Keys.Up), new PadBinding(Buttons.DPadUp));
			SetBinding(InputAction.SysDown, new KeyBinding(Keys.Down), new PadBinding(Buttons.DPadDown));
			SetBinding(InputAction.SysLeft, new KeyBinding(Keys.Left), new PadBinding(Buttons.DPadLeft));
			SetBinding(InputAction.SysRight, new KeyBinding(Keys.Right), new PadBinding(Buttons.DPadRight));

			SetBinding(InputAction.SysLClick, new MouseBtnBinding(MouseButton.Left));
			SetBinding(InputAction.SysRClick, new MouseBtnBinding(MouseButton.Right));

			SetBinding(InputAction.SysFullScreen, new KeyBinding(Keys.F11), new KeyBinding(Keys.F12));


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
			DefineBindingGang(BindingGang.SysConfirm, InputAction.SysConfirm, InputAction.ArnoldJump, InputAction.Confirm);
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

			mPrevKeyboardState = mKeyboardState;
			mKeyboardState = Keyboard.GetState();

			for(int i = 0; i < 4; i++)
			{
				mPrevGamePadStates[i] = mGamePadStates[i];
				mGamePadStates[i] = GamePad.GetState(i);
			}

			mMouseState = Mouse.GetState();

			if(GetFirstNewlyPressedKey() != Keys.None)
			{
				UpdateInputOrder(InputBindingType.kKeyboard);
			}
			else if(GetFirstNewlyPressedPadButton() != Buttons.None)
			{
				UpdateInputOrder(InputBindingType.kGamepad);
			}

			mInputUpdateIndex++;

			// If you leave the game open for more than this then maybe we should stop...
			if(mInputUpdateIndex == UInt64.MaxValue-32)
			{
				Main.ExitGame();
			}
		}



		/// <summary>
		/// Update the order of most recent key types
		/// </summary>
		void UpdateInputOrder(InputBindingType recentInput)
		{
			// Remove the input from the list if it exists
			if (mInputOrder.Contains(recentInput))
			{
				mInputOrder.Remove(recentInput);
			}

			// Insert the input at the front (most recent)
			mInputOrder.Insert(0, recentInput);
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
			float scaleFactor = (float)screenRect.Width / (float)Screen.SCREEN_WIDTH;

			Vector2 worldPosVec = new Vector2(mousePos.X, mousePos.Y) / scaleFactor;

			worldPosVec.X = Math.Clamp(worldPosVec.X, 0.0f, Screen.SCREEN_WIDTH);
			worldPosVec.Y = Math.Clamp(worldPosVec.Y, 0.0f, Screen.SCREEN_HEIGHT);

			if (relativeToCam is not null)
			{
				CameraSpec spec = relativeToCam.GetCurrentSpec();

				worldPosVec += spec.mPosition;
				worldPosVec *= spec.mZoom;
			}

			return worldPosVec;
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
		public UInt64 GetNumberOfInputFrames()
		{
			return mInputUpdateIndex;
		}


		/// <summary>
		/// Frames
		/// </summary>
		public void LoadInputFrames(UInt64 frames)
		{
			mInputUpdateIndex = frames;
		}

		#endregion rKeySense





		#region rRebind

		/// <summary>
		/// Find the first newly pressed key then add it to our input actions.
		/// Returns true if we did rebind this action
		/// </summary>
		public bool AttemptRebind(InputAction action)
		{
			MonoDebug.Assert(mInputBindings.ContainsKey(action), "Invalid action rebind.");

			Keys pressedKey = GetFirstNewlyPressedKey();
			Buttons pressedButton = GetFirstNewlyPressedPadButton();

			InputBinding newBinding = null;
			if(pressedKey != Keys.None)
			{
				newBinding = new KeyBinding(pressedKey);
			}
			else if(pressedButton != Buttons.None)
			{
				newBinding = new PadBinding(pressedButton);
			}

			if (newBinding is not null)
			{
				InputBindSet bindingSet = mInputBindings[action];

				bindingSet.AddRebinding(newBinding);
				return true;
			}

			return false;
		}



		/// <summary>
		/// Get first newly pressed key(for rebind detection)
		/// </summary>
		Keys GetFirstNewlyPressedKey()
		{
			Keys[] currPressedKeys = mKeyboardState.GetPressedKeys();
			Keys[] prevPressedKeys = mPrevKeyboardState.GetPressedKeys();

			return MonoAlg.GetFirstNewElement<Keys>(currPressedKeys, prevPressedKeys);
		}



		/// <summary>
		/// Get first newly pressed key(for rebind detection)
		/// </summary>
		Buttons GetFirstNewlyPressedPadButton()
		{
			for(int i = 0; i < 4; i++)
			{
				ref GamePadState currState = ref mGamePadStates[i];
				ref GamePadState prevState = ref mPrevGamePadStates[i];

				// Check each button to see if it was newly pressed
				foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
				{
					// Check if the button is pressed in the current state but not in the previous state
					if (currState.IsButtonDown(button) && !prevState.IsButtonDown(button))
					{
						return button;
					}
				}
			}

			return Buttons.None;
		}

		#endregion rRebind





		#region rUtility

		/// <summary>
		/// Get which are the most recent input types
		/// </summary>
		public List<InputBindingType> GetMostRecentInputTypes()
		{
			return mInputOrder;
		}

		#endregion rUtility





		#region rSerial

		/// <summary>
		/// Read binary segment
		/// </summary>
		public void ReadFromBinary(BinaryReader br)
		{
			mInputBindings = new InputBindingMap();
			int numBindings = br.ReadInt32();
			for(int i = 0; i < numBindings; i++)
			{
				InputAction action = (InputAction)br.ReadInt32();
				InputBindSet newSet = InputBindSet.ReadFromBinary(br);
				mInputBindings.Add(action, newSet);
			}
		}



		/// <summary>
		/// Write binary segment
		/// </summary>
		public void WriteFromBinary(BinaryWriter bw)
		{
			bw.Write(mInputBindings.Count());
			foreach (InputBindingPair keyBindPair in mInputBindings)
			{
				InputAction actionEnum = keyBindPair.Key;
				bw.Write((int)actionEnum);
				keyBindPair.Value.WriteFromBinary(bw);
			}
		}

		#endregion rSerial
	}
}
