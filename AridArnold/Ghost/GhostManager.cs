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

            mGhostArnold.Update(gameTime);

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
    }
}
