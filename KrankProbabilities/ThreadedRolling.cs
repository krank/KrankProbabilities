using System;
using System.Collections.Concurrent;
using System.Threading;

namespace KrankProbabilities
{
  public class ThreadedRolling
  {
    static ConcurrentDictionary<string, int> threadedResults = new ConcurrentDictionary<string, int>();

    public static int numRolls;
    public static int numDice;
    public static int numThreads;

    public static TimeSpan delta;


    public static int StraightRolls()
    {
      int numSuccessful = 0;

      // Init threads
      Thread[] threads = new Thread[numThreads];

      for (int i = 0; i < numThreads; i++)
      {
        threads[i] = new Thread(ThreadedRolls);
        threads[i].Name = "Thread #" + i;
      }


      DateTime before = DateTime.Now;

      // Start all threads

      foreach(Thread t in threads) t.Start();

      // Wait for all threads to finish before continuing"
      foreach(Thread t in threads) t.Join();

      DateTime after = DateTime.Now;

      delta = after - before;

      // Add together all successes
      numSuccessful = 0;
      foreach (int results in threadedResults.Values) numSuccessful += results;

      return numSuccessful;
    }

    static void ThreadedRolls()
    {
      int r = Krank.ManyDieRoll(numRolls, numDice, new Random());
      
      threadedResults.TryAdd(Thread.CurrentThread.Name, r);
    }


  }
}
