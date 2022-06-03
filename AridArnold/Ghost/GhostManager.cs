using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using AridArnold.Levels;

namespace AridArnold
{
    struct GhostInfo
    {
        public Vector2 position;
        public Vector2 velocity;
        public bool grounded;
        public CardinalDirection gravity;
    }

    /// <summary>
    /// Controls and manages replay ghosts
    /// </summary>
    internal class GhostManager : Singleton<GhostManager>
    {
        //Only record every 4th frame.
        const int FRAME_SUB = 4;
        
        Level mCurrentLevel;

        int mCurrentFrame;
        int mRecordFrame;

        GhostArnold mGhostArnold;


        GhostFile mInputFile;
        GhostFile mOutputFile;

        public void Load(ContentManager content)
        {
            mGhostArnold = new GhostArnold(Vector2.Zero);
            mGhostArnold.LoadContent(content);
        }

        //Update
        public void StartLevel(Level level)
        {
            mCurrentLevel = level;
            mCurrentFrame = 0;
            mRecordFrame = 0;

            mInputFile = new GhostFile(level);
            mOutputFile = new GhostFile(level);

            mGhostArnold.StartLevel();

            mInputFile.Load();
        }

        public void EndLevel(bool levelWin)
        {
            if (levelWin)
            {
                if(mInputFile.IsEmpty() || mOutputFile.GetFrameCount() < mInputFile.GetFrameCount())
                {
                    mOutputFile.Save();
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            mCurrentFrame++;

            if(mRecordFrame < mInputFile.GetFrameCount())
            {
                mGhostArnold.Update(gameTime);
            }

            if (mCurrentFrame % FRAME_SUB == 0)
            {
                RecordFrame();
                mRecordFrame++;
            }
        }

        //Recording
        public void RecordFrame()
        {
            for(int i = 0; i < EntityManager.I.GetEntityNum(); i++)
            {
                Entity entity = EntityManager.I.GetEntity(i);

                if(entity is Arnold)
                {
                    Arnold arnold = (Arnold)entity;
                    mOutputFile.RecordFrame(arnold, mRecordFrame);
                }
            }
        }

        //Draw
        public void Draw(DrawInfo info)
        {
            if (mInputFile.IsEmpty() == false)
            {
                List<GhostInfo> ghosts = mInputFile.ReadFrame(mRecordFrame);

                foreach (GhostInfo ghost in ghosts)
                {
                    mGhostArnold.SetGhostInfo(ghost);
                    mGhostArnold.Draw(info);
                }
            }
        }

        //Interface
        public string GetTime()
        {
            return FrameTimeToString(mOutputFile.GetFrameCount());
        }

        public string GetTimeToBeat()
        {
            if(mInputFile.IsEmpty())
            {
                return "";
            }
            
            return FrameTimeToString(mInputFile.GetFrameCount());
        }

        //Util
        private string FrameTimeToString(int frame)
        {
            int ms = (int)(frame * (1000.0f / 60.0f));
            int cs = (int)(ms / 10);
            int s = (int)(cs / 100);
            int m = (int)(s / 60);


            if (m == 0)
            {
                return String.Format("{0:D2} : {1:D2}", s, cs % 100);
            }
            else
            {
                return String.Format("{0:D} : {1:D2} : {2:D2}", m, s % 60, cs % 100);
            }
        }
    }
}
