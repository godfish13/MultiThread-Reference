using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ServerCore
{
    internal class _2_3_ReaderWriterLock
    {
        static ReaderWriterLockSlim _rwlock = new ReaderWriterLockSlim();

        class Reward
        {

        }

        static Reward GetRewardByid(int id)
        {
            _rwlock.EnterReadLock();

            _rwlock.ExitReadLock();
            return null;
        }

        static Reward AddReward(Reward reward) 
        {
            _rwlock.EnterWriteLock();

            _rwlock.ExitWriteLock();
            return reward;
        }
    }
}
