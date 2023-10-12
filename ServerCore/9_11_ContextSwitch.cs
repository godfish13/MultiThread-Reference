using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    ////////////////////////////////////////////////////////
    // 11 Context Switching

    class SpinLock
    {
        volatile int _locked = 0;   // false == 0, true == 1                          

        public void Acquire()
        {
            while (true)
            {
                int expected = 0;
                int desired = 1;
                if (Interlocked.CompareExchange(ref _locked, desired, expected) == expected)
                    break;

                // lock이 사용중일시 휴식하고 올게!를 구현하는 3가지 방식
                //Thread.Sleep(1);    // 무조건 휴식 ==> 1ms 정도 쉬고 싶어용, 단 운영체제가 스케쥴러에 따라 Sleep시켜서 정확히 1ms만큼 대기 안함 랜덤함
                //Thread.Sleep(0);    // 조건부 양보 ==> 나보다 우선순위가 낮은 스레드에 양보안함 ==> 우선순위가 나보다 높거나 같은 스레드가 없으면 본인 실행
                Thread.Yield();     // 관대한 양보 ==> 조건없이 양보, 지금 실행 가능한 스레드가 있으면 실행하도록 함 ==> 실행가능한 스레드 없으면 남은 시간 소진    
            }
        }

        public void Release()
        {
            _locked = 0;
        }
    }

    internal class _11_ContextSwitch
    {       
        static int _num = 0;
        static SpinLock _lock = new SpinLock();

        static void thread1()
        {
            for (int i = 0; i < 100000; i++)
            {
                _lock.Acquire();
                _num++;
                _lock.Release();
            }
        }

        static void thread2()
        {
            for (int i = 0; i < 100000; i++)
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
