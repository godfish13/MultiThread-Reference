using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    internal class _7_8_DeadLock
    {
        ////////////////////////////////////////////////////////
        // 7, 8 Lock 기초와 DeadLock
        static int number = 0;
        static object obj = new object();

        static void Thread1()
        {
            for (int i = 0; i < 100000; i++)
            {
                //Monitor.Enter(obj); // 상호배제 Mutual Exclusive // Enter ~ Exit 사이의 코드는 원자성을 지니게 됨
                //number++;
                //Monitor.Exit(obj);  // 그러나 오류가 생기기 쉬움
                lock (obj)          // 위와 같은 내용이나 Exit빼먹을 위험성이 훨씬 적음
                {
                    number++;
                }
            }
        }

        static void Thread2()
        {
            for (int i = 0; i < 100000; i++)
            {
                lock(obj) 
                {
                    number--;
                }
            }
        }

        static void Main(string[] args)
        {
            Task t1 = new Task(Thread1);
            Task t2 = new Task(Thread2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }
    }
}
