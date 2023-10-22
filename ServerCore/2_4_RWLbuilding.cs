using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ServerCore
{
    // 재귀적 락 허용할지 말지 결정해야함 여기선 YES => WriteLock->WriteLock ok / WriteLock->ReadLock ok / ReadLock->WriteLock no
    // 스핀락 정책(5000번 -> yield())
    class Lock
    {                                       
        const int EMPTY_FLAG = 0x00000000;  // 16진수는 각 0이 2^4의 값을 가짐, F는 4개 모두 1을 의미함
        const int WRITE_MASK = 0x7FFF0000;  // 32 비트 중 제일 앞의 unused 빼고 15개
        const int READ_MASK = 0x0000FFFF;   // 32 비트 중 뒤에서부터 16개
        const int MAX_SPIN_COUNT = 5000;    // 스핀락 5000번 돌림
                                            // 재귀적 락을 사용하기 위해 현재 관리되는 thread의 ID가 필요함으로 16진수로 추출 및 사용
        int _flag = EMPTY_FLAG;  // 총 32비트중 [unused(1) = 음수표현 제일앞자리] [WirteThreadID(15)] [ReadCount(16)] 로 나눠서 사용
                                 // 현재 플래그의 상태에 따라 EMPTH_FLAG면 락이 안걸려있는것, 다른 값이면 락이 걸려있는걸로 사용
        int _writeCount = 0;    // WriteLock 잡은 스레드만 사용가능함으로 그냥 int변수로 사용

        public void WriteLock()
        {
            // 동일 스레드(자기자신)가 WriteLock을 이미 획득하고 있는지 확인
            int LockThreadID = (_flag & WRITE_MASK) >> 16;      // 현재 락을 획득한 스레드의 ID
            if (Thread.CurrentThread.ManagedThreadId == LockThreadID) // 락을 가지고있는 스레드가 지금 여기 들어온 스레드랑 동일(재귀)하면
            {
                _writeCount++;
                return;
            }

            // 아무도 WriteLock or ReadLock을 획득하고 있지 않을 때, 경합해서 소유권 획득
            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;    // 현재 관리되는 스레드의 ID에 WriteMask를 씌울것이므로
            while (true)                                                                // 16자리 밀어서 ReadMask 파트 비우고 씌움
            {
                for (int i = 0; i< MAX_SPIN_COUNT; i++)
                {
                    //시도 성공하면 return
                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {                           // 아무도 WriteLock or ReadLock을 획득하고 있지 않을 때(EMPTY_FLAG 상태일 때), 경합해서 소유권 획득
                        _writeCount = 1;
                        return;
                    }
                }

                Thread.Yield(); //5000번 돌려보고 안되면 일단 yield
            }
        }

        public void WriteUnLock()
        {
            int LockCount = --_writeCount;  
            if (LockCount == 0)             // 재귀적으로 여러번 걸린 락이 모두 해제된 경우 UnLock
                Interlocked.Exchange(ref _flag, EMPTY_FLAG);    // _flag 깔끔하게 0으로 밀어줌
        }

        public void ReadLock() 
        {
            // 동일 스레드(자기자신)가 WriteLock을 이미 획득하고 있는지 확인
            int LockThreadID = (_flag & WRITE_MASK) >> 16;      // 현재 락을 획득한 스레드의 ID
            if (Thread.CurrentThread.ManagedThreadId == LockThreadID) // 락을 가지고있는 스레드가 지금 여기 들어온 스레드랑 동일(재귀)하면
            {
                Interlocked.Increment(ref _flag);   // ReadLock이므로 _flag의 값 +1
                return;
            }

            // 아무도 WriteLock을 획득하고 있지 않으면 ReadCount를 1 늘린다
            // == WRITE_MASK 파트가 0000이면 READ_MASK 파트를 1 늘림
            while (true) 
            {
                for (int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    int expected = (_flag & READ_MASK);
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
                        return;             // READ_MASK 파트만 존재 ==> WriteLock이 안걸려 있다는 뜻 
                }                           // 그러므로 WriteLock파트가 없으면 Read파트 1 늘림
                // 동시에 들어온 스레드중 경합에 이긴 스레드는 자연스레 통과하고 사용, 진애 _flag값이 expected랑 변화하였으므로 한턴 대기 후 사용 
                Thread.Yield();
            }
        }

        public void ReadUnLock()
        {
            Interlocked.Decrement(ref _flag);   // _flag - 1 
        }
    }

    internal class _2_4_RWLbuilding
    {
        static volatile int count = 0;
        static Lock _lock = new Lock();

        static void Main(string[] args)
        {
            Task t1 = new Task(delegate ()
            {
                for(int i = 0; i < 100000; i++)
                {
                    _lock.WriteLock();
                    count++;
                    _lock.WriteUnLock();
                }
            });

            Task t2 = new Task(delegate ()
            {
                for (int i = 0; i < 100000; i++)
                {
                    _lock.WriteLock();
                    count--;
                    _lock.WriteUnLock();
                }
            });

            t1.Start();
            t2.Start();
            Task.WaitAll(t1, t2);
            Console.WriteLine(count);
        }
    }
}
