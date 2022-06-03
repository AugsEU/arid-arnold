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
    internal class GhostFile
    {
        const int MAX_FRAMES = 60 * 60 * 4;//4 minutes of recording.
        readonly char[] FILE_MAGIC = { 'G', 'H', 'T' };

        Level mLevel;
        List<List<GhostInfo>> mGhostInfos;

        public GhostFile(Level level)
        {
            mGhostInfos = new List<List<GhostInfo>>(MAX_FRAMES);
            mLevel = level;
        }

        public void Save()
        {
            string filePath = GetFilename();

            if(mGhostInfos.Count == MAX_FRAMES)
            {

            }


            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }

            using (FileStream stream = File.OpenWrite(filePath))
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    //Write header.
                    bw.Write(FILE_MAGIC);
                    bw.Write((int)mGhostInfos.Count);

                    //Write data.
                    for(int i = 0; i < mGhostInfos.Count; i++)
                    {
                        bw.Write((int)mGhostInfos[i].Count);
                        foreach (GhostInfo info in mGhostInfos[i])
                        {
                            bw.Write(info.position.X); bw.Write(info.position.Y);
                            bw.Write(info.velocity.X); bw.Write(info.velocity.Y);
                            bw.Write(info.grounded);
                            bw.Write((int)info.gravity);
                        }
                    }
                }
            }
        }

        public void Load()
        {
            string filePath = GetFilename();

            if (!File.Exists(filePath))
            {
                return;
            }

            bool delFile = false;

            using (FileStream stream = File.OpenRead(filePath))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    try
                    {
                        //Read header.
                        br.ReadChars(FILE_MAGIC.Length);
                        int count = br.ReadInt32();

                        //Read data.
                        for (int i = 0; i < count; i++)
                        {
                            int ghostNumber = br.ReadInt32();

                            mGhostInfos.Add(new List<GhostInfo>());

                            for (int j = 0; j < ghostNumber; j++)
                            {
                                GhostInfo info;

                                info.position.X = br.ReadSingle(); info.position.Y = br.ReadSingle();
                                info.velocity.X = br.ReadSingle(); info.velocity.Y = br.ReadSingle();
                                info.grounded = br.ReadBoolean();
                                info.gravity = (CardinalDirection)br.ReadInt32();

                                mGhostInfos[i].Add(info);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        delFile = true;
                        mGhostInfos.Clear();
                        Util.Log("Exception: " + ex.ToString());
                    }
                }
            }

            if(delFile)
            {
                File.Delete(filePath);
            }
        }

        public void Close()
        {
            mGhostInfos.Clear();
            mLevel = null;
        }

        public void RecordFrame(PlatformingEntity entity, int frame)
        {
            if (frame < MAX_FRAMES)
            {
                if (mGhostInfos.Count-1 < frame)
                {
                    mGhostInfos.Add(new List<GhostInfo>());
                }

                GhostInfo info;
                info.position = entity.position;
                info.velocity = entity.velocity;
                info.grounded = entity.grounded;
                info.gravity = entity.GetGravityDir();

                mGhostInfos[frame].Add(info);
            }
        }

        public List<GhostInfo> ReadFrame(int frame)
        {
            frame = Math.Min(frame, mGhostInfos.Count - 1);
            return mGhostInfos[frame];
        }

        public bool IsEmpty()
        {
            return mGhostInfos.Count == 0;
        }

        public int GetFrameCount()
        {
            return mGhostInfos.Count;
        }

        private string GetFilename()
        {
            return Directory.GetCurrentDirectory() + "\\ghostData\\" + mLevel.Name + ".ght";
        }


    }
}
