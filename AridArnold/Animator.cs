using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold
{
    struct AnimationFrame
    {
        public AnimationFrame(Texture2D img, float t)
        {
            mImage = img;
            mDuration = t;
        }

        public Texture2D mImage;
        public float mDuration;
    }

    internal class Animator
    {
        public enum PlayType
        {
            OneShot,
            Repeat
        };

        List<AnimationFrame> mFrames = new List<AnimationFrame>();
        PlayType mPlayType;
        float mTotalDuration;
        float mPlayHead;
        bool mPlaying;

        public Animator(PlayType playType = PlayType.Repeat)
        {
            mPlaying = false;
            mTotalDuration = 0.0f;
            mPlayHead = 0.0f;
            mPlayType = playType;
        }

        public void Play()
        {
            mPlaying = true;
            mPlayHead = 0.0f;
        }

        public void Stop()
        {
            mPlaying = false;
        }

        public bool IsPlaying()
        {
            return mPlaying;
        }

        //Frames can only be added, not removed(yet).
        public void LoadFrame(ContentManager content, string textureName, float duration)
        {
            mTotalDuration += duration;
            mFrames.Add(new AnimationFrame(content.Load<Texture2D>(textureName), duration));
        }

        public void Update(GameTime gameTime)
        {
            if (mPlaying)
            {
                mPlayHead += (float)gameTime.ElapsedGameTime.TotalSeconds;

                switch (mPlayType)
                {
                    case PlayType.OneShot:
                        if (mPlayHead > mTotalDuration)
                        {
                            Stop();
                        }
                        break;
                    case PlayType.Repeat:
                        while (mPlayHead > mTotalDuration)
                        {
                            mPlayHead -= mTotalDuration;
                        }
                        break;
                    default:
                        break;
                }

            }
        }

        public Texture2D GetCurrentTexture()
        {
            float timeLeft = mPlayHead;
            int i = 0;
            for ( ; i < mFrames.Count; i++)
            {
                timeLeft -= mFrames[i].mDuration;

                if(timeLeft < 0.0f)
                {
                    break;
                }
            }

            return mFrames[i].mImage;
        }

        public Texture2D GetTexture(int index)
        {
            return mFrames[index].mImage;
        }
    
    }
}
