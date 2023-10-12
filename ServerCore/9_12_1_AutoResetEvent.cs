using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    ///////////////////////////////////////////////
    // 12. AutoReset Event

    class Lock
    {
        AutoResetEvent _available = new AutoResetEvent(true);  // 커널(관리자)레벨에서 사용되는 일종의 bool

        public void Acquire()
        {
            _available.WaitOne();   // 입장 시도, 입장 성공하면 _available = false
            //_available.Reset();     // 락 걸어줌 _available = false로 바꿔줌 // 단, WaitOne()이 자동으로 바꿔줬으므로 생략 
        }

        public void Release()
        {
            _available.Set();   // 락 해제, _available = true
        }

        //+추가 ManualReset Event
        /*ManualResetEvent _available = new ManualResetEvent(true);
        public void Acquire()
        {
            _available.WaitOne();   // 입장 시도, Manual은 입장 성공해도 문 안닫아줌
            _available.Reset();     // 락 걸어줌 _available2 = false로 바꿔줌
        }   // 이 코드는 원자성이 보장되지 않고 WaitOne과 Reset을 따로 실행하므로 오류 발생함
            // 나중에 한번에 여러번 통과시키고 락을 걸고싶은 경우 사용 // 당장 배우려는 부분은 아니므로 auto사용함
        public void Release()
        {
            _available.Set();   // 락 해제, _available2 = true
        }*/

    }

    internal class _9_12_1_AutoResetEvent
    {
        static int _num = 0;
        static Lock _lock = new Lock();

        static void thread1()
        {
            for (int i = 0; i < 10000; i++)     // 커널레벨에서 조작하느라 굉장히 느려지므로 100000정도만 되어도 한참걸림
            {
                _lock.Acquire();
                _num++;
                _lock.Release();
            }
        }

        static void thread2()
        {
            for (int i = 0; i < 10000; i++)
            {
                _lock.Acquire();
                _num--;
                _lock.Release();
            }
        }

        static void Main(string[] args)
        {
            Task t1 = new Task(thread1);
            Task t2 = new Task(thread2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(_num);
        }
    }
}
