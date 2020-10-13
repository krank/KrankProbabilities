using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace KrankProbabilities
{
  public class ThreadedRolling
  {
    static ConcurrentDictionary<string, int> threadedResults = new ConcurrentDictionary<string, int>();

    static ConcurrentDictionary<string, Dictionary<Krank.Result, int>> threadedContestResults;

    public static int numRollsPerThread;
    public static int numDice;
    public static int numThreads;

    public static int numDiceB;

    public static TimeSpan delta;


    public static Dictionary<Krank.Result, int> ContestRolls()
    {
      // Scrub the dictionary clean
      threadedContestResults = new ConcurrentDictionary<string, Dictionary<Krank.Result, int>>();

      DoThreading(ContestRollsAction);

      // Combine the dictionaries

      Dictionary<Krank.Result, int> finalResults = new Dictionary<Krank.Result, int>();
      finalResults.Add(Krank.Result.win, 0);
      finalResults.Add(Krank.Result.lose, 0);
      finalResults.Add(Krank.Result.draw, 0);
      finalResults.Add(Krank.Result.nowin, 0);

      foreach (Dictionary<Krank.Result, int> result in threadedContestResults.Values)
      {
        foreach (Krank.Result resultKey in result.Keys)
        {
          finalResults[resultKey] += result[resultKey];
        }
      }

      return finalResults;
    }

    public static void ContestRollsAction()
    {
      Random generator = new Random();
      int[] batches = new int[20];

      for (int i = 0; i < batches.Length; i++)
      {
        if (i == batches.Length - 1 && numRollsPerThread % 20 != 0)
        {
          batches[i] = numRollsPerThread % 20;
        }
        else
        {
          batches[i] = numRollsPerThread / 20;
        }
      }

      // Split into batches (5% in each)
      // Do 1 batch at a time, update

      Dictionary<Krank.Result, int> results = new Dictionary<Krank.Result, int>();

      results.Add(Krank.Result.win, 0);
      results.Add(Krank.Result.lose, 0);
      results.Add(Krank.Result.draw, 0);
      results.Add(Krank.Result.nowin, 0);

      for (int i = 0; i < batches.Length; i++)
      {

        Console.WriteLine($"Thread #{Thread.CurrentThread.Name} batch [{i+1}/{batches.Length}]" );

        int batchSize = batches[i];
        
        Dictionary<Krank.Result, int> batchResults = Krank.ManyContests(batchSize, numDice, numDiceB, generator);

        foreach (Krank.Result key in batchResults.Keys)
        {
          results[key] += batchResults[key];
        }
      }
      Console.WriteLine($"Thread #{Thread.CurrentThread.Name} DONE" );

      threadedContestResults.TryAdd(Thread.CurrentThread.Name, results);
    }

    public static int StraightRolls()
    {
      int numSuccessful = 0;

      DoThreading(StraightRollsAction);

      // Add together all successes
      numSuccessful = 0;
      foreach (int results in threadedResults.Values) numSuccessful += results;

      return numSuccessful;
    }

    static void StraightRollsAction()
    {
      int r = Krank.ManyDieRoll(numRollsPerThread, numDice, new Random());

      threadedResults.TryAdd(Thread.CurrentThread.Name, r);
    }

    private static void DoThreading(ThreadStart a)
    {
      // Init threads
      Thread[] threads = new Thread[numThreads];

      for (int i = 0; i < numThreads; i++)
      {
        threads[i] = new Thread(a);
        threads[i].Name = "" + i;
      }

      DateTime before = DateTime.Now;

      // Start all threads

      foreach (Thread t in threads) t.Start();

      // Wait for all threads to finish before continuing
      foreach (Thread t in threads) t.Join();

      DateTime after = DateTime.Now;

      delta = after - before;
    }

  }
}
