using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ServerCore
{
    internal class _1_3_CompilerOptimize
    {
        //////////////////////////////////////////////////////////////////////////////////
        // 3 컴파일러 최적화 실습
        static bool stop = false;       // static, 전역(heap)메모리에 할당했으므로 모든 스레드에서 접근가능

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
        }

        
    }
}
