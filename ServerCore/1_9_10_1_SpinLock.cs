using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    ////////////////////////////////////////////////////////
    // (9 Lock 구현 이론) 10 ***** SpinLock *****
    class SpinLock
    {
        /*volatile bool _locked = false;
        public void Acquire()
        {
            while (_locked)
            {
                // 잠김 풀릴때까지 대기
            }

            _locked = true;  // 자신이 획득했으므로 자기 작업 진행하면서 lock걸어버리기
            // 위 형태는 대기와 잠금이 원자성을 지니지 못하고 따로발생하고있음 -> 원자성이 보장되지 못함
            // 따라서 두 스레드가 동시에 _locked == false일때 접근해버려 작업이 꼬일 가능성이 존재한다         
        }*/

        volatile int _locked = 0;   // false == 0, true == 1

        public void Acquire1()  // Exchange 사용
        {
            while (true)
            {
                int original = Interlocked.Exchange(ref _locked, 1);    //!!!! 매개변수2번째로 지정한 값으로 설정하고 ***원래***값을 반환 !!!!
                if (original == 0)      // original == 1 이면 '원래'값이 1이었던 것이므로 이미 잠겨있었다는 뜻                                    
                    break;              // 그러므로 original == 0 인 경우가 원하던 잠겨있지 않던 경우
            }                           // +) _locked는 volatile 키워드이므로 맘대로 값 할당하고 사용하면 안된다!!!
        }                               // 하지만 original은 각각 스레드의 스택에서 사용되는 단순변수이므로 이 값은 사용해도 됨 이런거 주의해야함

        public void Acquire2()   // CompareExchange 사용, 일반적으로 Exchange보다 많이 사용함
        {
            while (true)
            {
                int expected = 0;   // CompareExchange를 그대로 쓰면 헷갈리기 쉬우므로 기대값, 기대값과 일치하면 변동시킬 값을 정해놓고 이름으로 쓰면 더 가독성이 좋음
                int desired = 1;
                if (Interlocked.CompareExchange(ref _locked, desired, expected) == expected)
                    break;                       // _locked를 expected와 비교하여 같으면 _locked를 desired로 바꾸고 ***원래***값을 반환
            }                                    // 원래 값이 기대값과 일치했으므로 대기해제하고 lock걸어줌
        }

        public void Release()
        {
            _locked = 0;
        }
    }


    internal class _1_9_10_1_SpinLock
    {
        ////////////////////////////////////////////////////////
        // (9 Lock 구현 이론) 10 SpinLock
        static int _num = 0;
        static SpinLock _lock = new SpinLock();

        static void thread1()
        {
            for (int i = 0; i < 100000; i++)
            {
                _lock.Acquire2();
                _num++;
                _lock.Release();
            }
        }

        static void thread2()
        {
            for (int i = 0; i < 100000; i++)
            {
                _lock.Acquire2();
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
