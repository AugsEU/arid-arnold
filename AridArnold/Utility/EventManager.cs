namespace AridArnold
{
	/// <summary>
	/// Types of event. New ones have to be registered here.
	/// </summary>
	enum EventType
	{
		PlayerDead,
		TimeChanged,
		KeyCollect,
		ShopDoorOpen,
		RedKeyUsed,
		DemonBossKilled,
	}


	struct EventFlag
	{
		public EventFlag()
		{
			mSignal = false;
			mTrigger = false;
		}

		public bool mTrigger;
		public bool mSignal;
	}


	/// <summary>
	/// Send and receive events.
	/// </summary>
	internal class EventManager : Singleton<EventManager>
	{
		EventFlag[] mEventFlags;

		public EventManager()
		{
			mEventFlags = new EventFlag[MonoEnum.EnumLength(typeof(EventType))];

			for (int i = 0; i < mEventFlags.Length; i++)
			{
				mEventFlags[i] = new EventFlag();
			}
		}

		public void TriggerEvent(EventType eventType)
		{
			mEventFlags[(int)eventType].mTrigger = true;
		}

		public bool IsSignaled(EventType eventType)
		{
			return mEventFlags[(int)eventType].mSignal;
		}

		public void Update(GameTime gameTime)
		{
			for (int i = 0; i < mEventFlags.Length; i++)
			{
				mEventFlags[i].mSignal = mEventFlags[i].mTrigger;
				mEventFlags[i].mTrigger = false;
			}
		}

		public void ResetAllEvents()
		{
			for (int i = 0; i < mEventFlags.Length; i++)
			{
				mEventFlags[i].mSignal = false;
				mEventFlags[i].mTrigger = false;
			}
		}

		public bool IsAnyEventHappening()
		{
			for (int i = 0; i < mEventFlags.Length; i++)
			{
				if(mEventFlags[i].mSignal || mEventFlags[i].mTrigger)
				{
					return true;
				}
			}

			return false;
		}
	}
}
