using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ServerCore
{
    internal class Program
    {
        //////////////////////////////////////////////////////////////////////////////////
        // 4_2-5 메모리 배리어
        // 1. 코드 재배치 방지(메모 참조)
        // 2. 가시성 (단, 사람이 보기 편하다는 가시성이 아니라 CPU가 메모리를 참조하기 편하단 의미의 가시성임)

        // 1) Full Memory Barrier (Thread.MemoryBarrier()) : Store/Load 둘 다 막음
        // 2) Store Memory Barrier : Store만 막음  // 단, 이하 둘은 어셈블리 영역정도에서나 사용 위에 Full만 제대로 확인하기
        // 2) Load Memory Barrier : Load만 막음  

        static int x = 0;
        static int y = 0;
        static int result1 = 0;
        static int result2 = 0;

        static void Thread1()
        {
            y = 1;
            
            //----------------------
            Thread.MemoryBarrier();
            
            result1 = x;
        }

        static void Thread2()
        {
            x = 1;

            //----------------------
            Thread.MemoryBarrier();
            
            result2 = y;
        }

        static void Main(string[] args)
        {
            int count = 0;
            while (true)
            {
                count++;
                x = y = result1 = result2 = 0;

                Task t1 = new Task(Thread1);
                Task t2 = new Task(Thread2);
                t1.Start();
                t2.Start();

                Task.WaitAll(t1, t2);

                if (result1 == 0 && result2 == 0) break;
            }

            Console.WriteLine($"{count}번만에 빠져나옴");
        }


        //////////////////////////////////////////////////////////////////////////////////
        // 4_2-4 캐시 이론
        /*static void Main(string[] args) 
        {
            int[,] arr = new int[10000, 10000];
            
            {
                long now = DateTime.Now.Ticks;              // 위아래 시간이 다름, 캐시이론에 따라 다르다
                for (int y = 0; y < 10000; y++)
                    for (int x = 0; x < 10000; x++) arr[y, x] = 1;

                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(y, x) 순서 걸린 시간 : {end - now}");
            }

            {
                long now = DateTime.Now.Ticks;
                for (int y = 0; y < 10000; y++)
                    for (int x = 0; x < 10000; x++) arr[x, y] = 1;

                long end = DateTime.Now.Ticks;
                Console.WriteLine($"(x, y) 순서 걸린 시간 : {end - now}");
            }
        }*/


        //////////////////////////////////////////////////////////////////////////////////
        // 4_2-3 컴파일러 최적화 실습
        /*static bool stop = false;       // static, 전역(heap)메모리에 할당했으므로 모든 스레드에서 접근가능

        static void ThreadMain()
        {
            Console.WriteLine("스레드 시작");

            while (stop == false)
            {
                // 누군가 stop하기전까지 무한반복
            }

            Console.WriteLine("스레드 끝");
        }

        static void Main(string[] args) 
        {
            Task T = new Task(ThreadMain);
            T.Start();

            Thread.Sleep(1000);     // 밀리세컨드 단위, 스레드 n초간 정지

            stop = true;

            Console.WriteLine("Stop 호출");
            Console.WriteLine("스레드 종료 대기");

            T.Wait();                               // thread의 join과 같은 역할, task가 끝날때까지 기다려줌

            Console.WriteLine("스레드 종료 완료!");
        }*/

        //////////////////////////////////////////////////////////////////////////////////
        /*static void MainThread(object State)          // 4_2-2 스레드 생성 실습
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

            Task t = new Task(() => { while (true) { }; }, TaskCreationOptions.LongRunning);
            t.Start();

            ThreadPool.QueueUserWorkItem(MainThread);

            Thread t = new Thread(MainThread);
            t.Name = "Test Thread";
            t.IsBackground = true;
            t.Start();
            Console.WriteLine("Waiting for thread...");
            t.Join();
            Console.WriteLine("Hello, World!");
            while(true)
            {

            }
        }*/
    }
}
