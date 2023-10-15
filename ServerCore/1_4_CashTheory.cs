using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    internal class _1_4_CashTheory
    {
        //////////////////////////////////////////////////////////////////////////////////
        // 4 캐시 이론
        static void Main(string[] args) 
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
        }
    }
}
