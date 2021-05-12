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

            try
            {
                task1.Wait();
            }
            catch (AggregateException)
            {
                Console.WriteLine("Canceled");
            }

            var task2 = task1.ContinueWith(x => Console.WriteLine("Task executed regardless of the result of the parent task"));
            task2.Wait();

            //Task B
            Console.WriteLine("Task B");
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;

            var task3 = new Task(() => DoLongRunningTask(token), token);
            task3.Start();
            Thread.Sleep(1000);
            cancelTokenSource.Cancel();

            try
            {
                task3.Wait();
            }
            catch (AggregateException)
            {
                Console.WriteLine("Canceled");
            }

            var task4 = task3.ContinueWith(x => Console.WriteLine("Task executed when the parent task finished without success"), TaskContinuationOptions.NotOnRanToCompletion);
            task4.Wait();

            //Task C
            Console.WriteLine("Task C");
            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;

            var task5 = new Task(() => throw null, token);
            task5.Start();

            try
            {
                task5.Wait();
            }
            catch (AggregateException)
            {
                Console.WriteLine("Task 1 Faulted");
            }

            var task6 = task5.ContinueWith(x =>
            {
                Console.WriteLine(Thread.CurrentThread.Name);
                Console.WriteLine("Task executed when the parent task would be finished with fail and parent task thread should be reused for continuation");
            }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);
            task6.Wait();

            //Task D
            Console.WriteLine("Task D");
            cancelTokenSource = new CancellationTokenSource();
            token = cancelTokenSource.Token;

            var task7 = new Task(() => DoLongRunningTask(token), token);
            task7.Start();
            cancelTokenSource.Cancel();

            try 
            { 
                task7.Wait(); 
            }
            catch(AggregateException) 
            {
                Console.WriteLine("Canceled");
            }

            var task8 = task7.ContinueWith(x =>
            {
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                Console.WriteLine("Task executed when the parent task would be finished with fail and parent task thread should be reused for continuation");
            }, TaskContinuationOptions.HideScheduler | TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.LongRunning);
            task8.Wait();

            Console.ReadLine();
        }

        static void DoLongRunningTask(CancellationToken token)
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
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
