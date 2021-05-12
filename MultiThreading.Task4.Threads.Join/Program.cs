/*
 * 4.	Write a program which recursively creates 10 threads.
 * Each thread should be with the same body and receive a state with integer number, decrement it,
 * print and pass as a state into the newly created thread.
 * Use Thread class for this task and Join for waiting threads.
 * 
 * Implement all of the following options:
 * - a) Use Thread class for this task and Join for waiting threads.
 * - b) ThreadPool class for this task and Semaphore for waiting threads.
 */

using System;
using System.Threading;

namespace MultiThreading.Task4.Threads.Join
{
    class Program
    {
        static int count = 10;
        static Semaphore semaphore = new Semaphore(1, 1);
        static void Main(string[] args)
        {
            Console.WriteLine("4.	Write a program which recursively creates 10 threads.");
            Console.WriteLine("Each thread should be with the same body and receive a state with integer number, decrement it, print and pass as a state into the newly created thread.");
            Console.WriteLine("Implement all of the following options:");
            Console.WriteLine();
            Console.WriteLine("- a) Use Thread class for this task and Join for waiting threads.");
            Console.WriteLine("- b) ThreadPool class for this task and Semaphore for waiting threads.");

            Console.WriteLine();

            //ThreadDecrement(count);

            var info = new Info
            {
                CurrentSemaphore = semaphore,
                State = count
            };
            ThreadPoolDecrement(info);

            Console.ReadLine();
        }

        static void ThreadDecrement(object obj)
        {
            var state = (int)obj;
            Thread thread = null;

            if(state > 1)
            {
                thread = new Thread(new ParameterizedThreadStart(ThreadDecrement));
                
                thread.Start(state - 1);
                thread?.Join();
            }

            Thread.CurrentThread.Name = state.ToString();
            Console.WriteLine($"Thred {Thread.CurrentThread.Name}# with state {state}");
        }

        static void ThreadPoolDecrement(object obj)
        {
            var info = obj as Info;

            if (info.State > 1)
            {
                info.CurrentSemaphore.WaitOne();
                var nextInfo = new Info
                {
                    PreviosSemaphore = info.CurrentSemaphore,
                    CurrentSemaphore = new Semaphore(1, 1),
                    State = info.State - 1
                };
                ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolDecrement), nextInfo);
                info.CurrentSemaphore.WaitOne();
            }

            Thread.CurrentThread.Name = info.State.ToString();
            Console.WriteLine($"Thred {Thread.CurrentThread.Name}# with state {info.State}");
            info.PreviosSemaphore?.Release();
        }

        public class Info
        {
            public int State { get; set; }

            public Semaphore PreviosSemaphore { get; set; }

            public Semaphore CurrentSemaphore { get; set; }
        }
    }
}
