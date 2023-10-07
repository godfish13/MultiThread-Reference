using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace ServerCore
{
    internal class _5_MemoryBarrier
    {
        //////////////////////////////////////////////////////////////////////////////////
        // 5 메모리 배리어

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
    }
}
