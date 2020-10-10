using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.ConstrainedExecution;
using System.Collections.Generic;
using System;

namespace KrankProbabilities
{
  class Program
  {

    static int successMinValue = 4;
    static int maxRecursion = 20;
    static Random generator = new Random();

    static int numRolls;
    static int numDice;

    static ConcurrentDictionary<string, int> threadedResults = new ConcurrentDictionary<string, int>();

    static void Main(string[] args)
    {

      numRolls = 10000000; // per thread
      numDice = 6;

      int numSuccessful = 0;

      int numThreads = 16;

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

      // Add together all successes
      numSuccessful = 0;
      foreach (int results in threadedResults.Values) numSuccessful += results;

      // Present the results
      Console.WriteLine($"{numRolls * numThreads} rolls, {numSuccessful} were successful ({((double)numSuccessful / (double)(numRolls * numThreads)) * 100}%)");
      Console.WriteLine((after - before));

      Console.ReadLine();
    }

    static void ThreadedRolls()
    {
      int r = ManyDieRoll(numRolls, numDice, new Random());
      //Console.WriteLine($"Result in thread: {r} as a result of rolling {numDice} dice {numRolls} times");
      

      threadedResults.TryAdd(Thread.CurrentThread.Name, r);
    }

    
    static int ManyDieRoll(int numRolls, int numDice)
    {
      return ManyDieRoll(numRolls, numDice, generator);
    }

    static int ManyDieRoll(int numRolls, int numDice, Random generator)
    {
      int numSuccessful = 0;

      for (int i = 0; i < numRolls; i++)
      {
        (int successes, int[] results) dieRollResults = DieRoll(numDice, generator);

        numSuccessful += dieRollResults.successes > 0 ? 1 : 0;

      }

      return numSuccessful;
    }

    static (int, int[]) DieRoll(int numOfDice)
    {
      return DieRoll(numDice, generator);
    }

    static (int, int[]) DieRoll(int numOfDice, Random generator)
    {

      List<int> results = new List<int>();
      int successes = 0;
      int recursion = 0;

      for (int i = 0; i < numOfDice; i++)
      {

        int dieResult = generator.Next(1, 7);
        results.Add(dieResult);
        if (dieResult >= successMinValue)
        {
          successes++;
        }

        while (i == numOfDice - 1 && dieResult == 6 && recursion < maxRecursion)
        {
          dieResult = generator.Next(1, 7);
          results.Add(dieResult);
          if (dieResult >= successMinValue)
          {
            successes++;
          }
        }

      }

      return (successes, results.ToArray());
    }
  }
}
