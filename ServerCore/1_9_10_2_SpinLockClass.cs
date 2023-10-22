using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    internal class _1_9_10_2_SpinLockClass        // C#에 있는 SpinLock 클래스 활용
    {
        static SpinLock _lock = new SpinLock(); // SpinLock은 기다리다가 답이없다 판단되면 양보했다가 다시 돌아오도록 복합적으로 구현되어 있음

        static void Main(string[] args)
        {
            bool lockTaken = false;

            try
            {
                _lock.Enter(ref lockTaken);
            }
            finally
            {
                if(lockTaken)
                    _lock.Exit();
            }
        }
    }
}
