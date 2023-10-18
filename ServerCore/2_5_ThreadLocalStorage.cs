using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ServerCore
{
    internal class _2_5_ThreadLocalStorage
    {
        static ThreadLocal<string> ThreadName = new ThreadLocal<string>();
        // static으로 선언되었으므로 힙영역에서 공유되어야 함
        // 하지만 ThreadLocal로 선언해주었음 ==> ThreadName의 값을 변경하여도 '스레드 자신에게만 값이 해당값으로 변경됨'
        // ==> 다른 스레드들이 접근하면 변한값이 반영안됨!!

        static ThreadLocal<string> ThreadName2 = new ThreadLocal<string>(() => { return $"My name is {Thread.CurrentThread.ManagedThreadId}"; });
        // valueFactory버전 ThreadLocal ==> value가 세팅이 안된 상태면 넣는값으로 세팅하고 아니면 value세팅 안함
        // 위에 valueFactory 아닌 버전은 WhoAmI 실행할때마다 동일 스레드가 실행하더라도 새로 세팅하게 됨 이런 낭비를 방지

        static void WhoAmI()
        {
            ThreadName.Value = $"My name is {Thread.CurrentThread.ManagedThreadId}";

            Thread.Sleep(1000);

            Console.WriteLine(ThreadName.Value);
        }

        static void WhoAmI2()
        {
            bool repeat = ThreadName2.IsValueCreated;
            if(repeat)
                Console.WriteLine(ThreadName2.Value + $"{repeat}");
            else
                Console.WriteLine(ThreadName2.Value);
        }

        static void Main(string[] args)
        {                    
            // Parallel.Invoke(WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI);
            // Parallel.Invoke(method) : 각 메소드를 병렬적으로 각각의 task로 실행
            // 각 WhoAmI에서 sleep동안 ThreadName을 변경해주고 있지만 TLS 변수 이므로 자기가 설정한 값으로만 나옴!

            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(3, 3);
            Parallel.Invoke(WhoAmI2, WhoAmI2, WhoAmI2, WhoAmI2, WhoAmI2, WhoAmI2, WhoAmI2);
            // 스레드 풀 내에서 돌리므로 3개 돌리고 작업끝난걸로 돌리고 반복
            // valueFactory버전을 통해 각 스레드가 반복해서 ThreadName2를 정해도 반복해서 설정하지않고 한번만 할 수 있게 됨!

            ThreadName2.Dispose(); // 설정해둔 TLS 삭제
        }

    }
}
