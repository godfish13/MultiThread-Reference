using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ServerCore
{
    internal class _6_InterLocked
    {
        ////////////////////////////////////////////////////////
        // 6 InterLocked
        static int number = 0;

        static void Thread1()
        {
            for (int i = 0; i < 100000; i++)
            {
                int aftervalue = Interlocked.Increment(ref number);
            }
        }

        static void Thread2()
        {
            for (int i = 0; i < 100000; i++)
            {
                Interlocked.Decrement(ref number);
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
