using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AridArnold.Screens;

namespace AridArnold
{
    class MonoTimer
    {
        bool mPlaying;
        double mElapsedTimeMs;

        public MonoTimer()
        {
            TimeManager.I.RegisterTimer(this);

            mElapsedTimeMs = 0.0;
            mPlaying = false;
        }

        ~MonoTimer()
        {
            TimeManager.I.RemoveTimer(this);
        }

        public double GetElapsedMs()
        {
            return mElapsedTimeMs;
        }

        public void Start()
        {
            mPlaying = true;
        }

        public void Update(GameTime gameTime)
        {
            if (mPlaying)
            {
                mElapsedTimeMs += gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }

        public void Stop()
        {
            mPlaying = false;
        }

        public void Reset()
        {
            mElapsedTimeMs = 0.0;
        }

        public void FullReset()
        {
            mPlaying = false;
            mElapsedTimeMs = 0.0;
        }

        public bool IsPlaying()
        {
            return mPlaying;
        }
    }

    internal class TimeManager : Singleton<TimeManager>
    {
        List<MonoTimer> mTimers = new List<MonoTimer>();

        public void RegisterTimer(MonoTimer timer)
        {
            mTimers.Add(timer);
        }

        public void RemoveTimer(MonoTimer timer)
        {
            mTimers.Remove(timer);
        }


        public void Update(GameTime gameTime)
        {
            foreach (MonoTimer timer in mTimers)
            {
                timer.Update(gameTime);
            }
        }
    }
}
