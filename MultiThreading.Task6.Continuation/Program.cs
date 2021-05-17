/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task6.Continuation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create a Task and attach continuations to it according to the following criteria:");
            Console.WriteLine("a.    Continuation task should be executed regardless of the result of the parent task.");
            Console.WriteLine("b.    Continuation task should be executed when the parent task finished without success.");
            Console.WriteLine("c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");
            Console.WriteLine("d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");
            Console.WriteLine("Demonstrate the work of the each case with console utility.");
            Console.WriteLine();

            // feel free to add your code

            //Task A
            Console.WriteLine("Task A");
            var task1 = Task.Factory.StartNew(() => throw null);
            var task2 = task1.ContinueWith(x => Console.WriteLine("Task executed regardless of the result of the parent task. Task1 throw error"));
            task2.Wait();

            task1 = Task.Factory.StartNew(() => Console.WriteLine("Done"));
            task2 = task1.ContinueWith(x => Console.WriteLine("Task executed regardless of the result of the parent task. Task1 end with success result"));
            task2.Wait();

            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;

            task1 = new Task(() => DoLongRunningTask(token), token);
            task2 = task1.ContinueWith(x => Console.WriteLine("Task executed regardless of the result of the parent task. Task1 was canceled"));
            task1.Start();
            cancelTokenSource.Cancel();
            task2.Wait();
            Console.WriteLine();

            //Task B
            Console.WriteLine("Task B");
            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;

            var task3 = new Task(() => DoLongRunningTask(token), token);
            var task4 = task3.ContinueWith(x => Console.WriteLine("Task executed when the parent task finished without success. Task1 was canceled"), TaskContinuationOptions.NotOnRanToCompletion);
            task3.Start();

            Thread.Sleep(1000);
            cancelTokenSource.Cancel();

            task4.Wait();

            task3 = new Task(() => throw null);
            task4 = task3.ContinueWith(x => Console.WriteLine("Task executed when the parent task finished without success. Task1 throw error"), TaskContinuationOptions.NotOnRanToCompletion);
            task3.Start();
            Console.WriteLine();

            //Task C
            Console.WriteLine("Task C");
            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;

            var task5 = new Task(() => throw null, token);
            var task6 = task5.ContinueWith(x =>
            {
                Console.WriteLine("Task 2  id:" + Thread.CurrentThread.ManagedThreadId);
                Console.WriteLine("Task executed when the parent task would be finished with fail and parent task thread should be reused for continuation");
            }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);
            task5.Start();       

            task6.Wait();
            Console.WriteLine();

            //Task D
            Console.WriteLine("Task D");
            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;
            
            var task7 = new Task(() => DoLongRunningTask(token), token);
            var task8 = task7.ContinueWith(x =>
            {
                Console.WriteLine("Task 2  id:" + Thread.CurrentThread.ManagedThreadId); 
                Console.WriteLine("Task 2 is ThreadPoolThread? Answer:" + Thread.CurrentThread.IsThreadPoolThread);

                Console.WriteLine("Task executed when the parent task would be finished with fail and parent task thread should be reused for continuation");
            },new CancellationToken(), TaskContinuationOptions.OnlyOnCanceled, new CustomTaskScheduler());

            task7.Start();
            Thread.Sleep(2000); // For printing info about task1 thread
            cancelTokenSource.Cancel();
            task8.Wait();

            Console.ReadLine();
        }

        static void DoLongRunningTask(CancellationToken token)
        {
            Console.WriteLine("Task 1  id:" + Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("Task 2 is ThreadPoolThread? Answer:" + Thread.CurrentThread.IsThreadPoolThread);
            Console.WriteLine("Task 1 Do long running");
            token.ThrowIfCancellationRequested();

            for (int i = 0; i <= 10; i++)
            {
                Thread.Sleep(1000);
                token.ThrowIfCancellationRequested();
            }
        }
    }
}
