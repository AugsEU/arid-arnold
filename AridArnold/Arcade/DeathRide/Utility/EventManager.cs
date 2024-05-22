namespace GMTK2023
{
	/// <summary>
	/// Callback type
	/// </summary>
	/// <param name="args">Sender args</param>
	delegate void EventCallback(EArgs args);





	/// <summary>
	/// Sender args. Might be pointless
	/// </summary>
	struct EArgs
	{
		public EArgs(object sender) { mSender = sender; }

		public object mSender;
	}





	/// <summary>
	/// Types of event. New ones have to be registered here.
	/// </summary>
	enum EventType
	{
		PlayerDead,
		KillPlayer,
		KeyCollect,
		ShopDoorOpen,
		RedKeyUsed
	}





	/// <summary>
	/// Send and receive events.
	/// </summary>
	internal class EventManager : Singleton<EventManager>
	{
		#region rMembers

		Dictionary<EventType, EventCallback> mEventListeners = new Dictionary<EventType, EventCallback>();

		#endregion rMembers





		#region rEvents

		/// <summary>
		/// Add a listener to an event type. The callback will be executed whenever that event type is triggered.
		/// </summary>
		/// <param name="type">Type of event to listen for</param>
		/// <param name="callback">Function to callback</param>
		public void AddListener(EventType type, EventCallback callback)
		{
			if (mEventListeners.ContainsKey(type))
			{
				mEventListeners[type] += callback;
			}
			else
			{
				mEventListeners.Add(type, callback);
			}
		}



		/// <summary>
		/// Trigger an event. Executes all callbacks in the listeners.
		/// </summary>
		/// <param name="type">Type of event to trigger</param>
		/// <param name="args">Arguements</param>
		public void SendEvent(EventType type, EArgs args)
		{
			if (mEventListeners.ContainsKey(type))
			{
				mEventListeners[type].Invoke(args);
			}
		}

		#endregion rEvents
	}
}
