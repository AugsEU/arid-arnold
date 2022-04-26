using System;
using System.Collections.Generic;
using System.Text;

namespace AridArnold
{
    enum CollectibleType
    {
        WaterBottle
    }

    internal class CollectibleManager : Singleton<CollectibleManager>
    {
        Dictionary<CollectibleType, uint> mCurrentCollectibles = new Dictionary<CollectibleType, uint>();

        public void CollectItem(CollectibleType type, uint number = 1)
        {
            if (mCurrentCollectibles.ContainsKey(type))
            {
                mCurrentCollectibles[type] += number;
            }
            else
            {
                mCurrentCollectibles.Add(type, number);
            }
        }

        public uint GetCollected(CollectibleType type)
        {
            if(!mCurrentCollectibles.ContainsKey(type))
            {
                mCurrentCollectibles.Add(type, 0);
            }

            return mCurrentCollectibles[type];
        }

        public void ClearAllCollectibles()
        {
            mCurrentCollectibles.Clear();
        }

    }
}
