/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
        public static int[] array = new int[10];
        private static AutoResetEvent addEvent = new AutoResetEvent(true);
        private static AutoResetEvent printEvent = new AutoResetEvent(false);
        public static Thread addTread;
        public static Thread printTread;

        static void Main(string[] args)
        {
            Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
            Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
            Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
            Console.WriteLine();

            var th1 = new Thread(AddNumberInArray);
            var th2 = new Thread(PrintNumberInArray);
            th1.Start();
            th2.Start();

            th1.Join();
            th2.Join();

            Console.ReadLine();
        }

        public static void AddNumberInArray()
        {
            for (int i = 1; i < 11; i++)
            {
                array[i - 1] = i;
                printEvent.Set();
                addEvent.WaitOne();
            }
        }

        public static void PrintNumberInArray()
        {
            for (int i = 1; i < 11; i++)
            {
                printEvent.WaitOne();
                Console.WriteLine("[{0}]", string.Join(", ", array));
                addEvent.Set();
            }
        }
    }
}
