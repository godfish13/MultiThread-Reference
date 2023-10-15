using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ServerCore
{
    internal class _1_2_ThreadCreate
    {
        //////////////////////////////////////////////////////////////////////////////////
        static void MainThread(object State)          // 4_2-2 스레드 생성 실습
        {
            for(int i = 0; i < 5; i++)
                Console.WriteLine("Hello, Thread!");
        }

        static void Main(string[] args)
        {         
            ThreadPool.SetMinThreads(1, 1);             // 첫번쨰 인자는 스레드풀에 만들어둘 스레드의 최소 갯수
            ThreadPool.SetMaxThreads(5, 5);             // 첫번째 인자는 스레드풀에 만들어둘 스레드의 최대 갯수

            for(int i = 0; i < 4; i++)
            {
                ThreadPool.QueueUserWorkItem((obj) => { while (true) { }; });
            }

            Task t = new Task(() => { while (true) { }; }, TaskCreationOptions.LongRunning);    // Task 이용
            t.Start();

            ThreadPool.QueueUserWorkItem(MainThread);

            /*Thread t = new Thread(MainThread);    // Thread 생성 기초
            t.Name = "Test Thread";
            t.IsBackground = true;
            t.Start();
            Console.WriteLine("Waiting for thread...");
            t.Join();
            Console.WriteLine("Hello, World!");
            while(true)
            {

            }*/
        }
    }
}
