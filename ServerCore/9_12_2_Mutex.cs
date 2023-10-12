using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    //////////////////////////////////////////
    // + Mutex
    internal class _9_12_2_Mutex
    {
        static int _num = 0;
        static Mutex _lock = new Mutex();   // ResetEvent와 비슷하나 단순히 bool로 작동하는 RE와 다르게 lock횟수 등 다양한 정보를 포함하고있음
                                            // lock이 여러번 걸렸을때 작동하는 기능 등 사용되나 당장 우리는 쓸일 없음
        static void thread1()
        {
            for (int i = 0; i < 100000; i++)     // 얘도 커널레벨에서 조작하느라 굉장히 느려짐
            {
                _lock.WaitOne();
                _num++;
                _lock.ReleaseMutex();
            }
        }

        static void thread2()
        {
            for (int i = 0; i < 100000; i++)
            {
                _lock.WaitOne();
                _num--;
                _lock.ReleaseMutex();
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
