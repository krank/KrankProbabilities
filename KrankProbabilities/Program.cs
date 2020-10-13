using System.IO;
using System.Globalization;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.ConstrainedExecution;
using System.Collections.Generic;
using System;

namespace KrankProbabilities
{
  class Program
  {
    static void Main(string[] args)
    {
      NumberFormatInfo format = new NumberFormatInfo { NumberGroupSeparator = " ", NumberDecimalDigits = 0 };

      // Setup
      ThreadedRolling.numRollsPerThread = 100000000; // per thread
      ThreadedRolling.numDice = 5;
      ThreadedRolling.numDiceB = 5;
      ThreadedRolling.numThreads = 16;
      Krank.useChaosDie = true;

      string csvFilename = @"data.csv";

      // Do the rollin'
      Dictionary<Krank.Result, int> results = ThreadedRolling.ContestRolls();

      // Show results
      int numContests = ThreadedRolling.numRollsPerThread * ThreadedRolling.numThreads;

      Console.WriteLine($"\nA: {ThreadedRolling.numDice} B: {ThreadedRolling.numDiceB}");

      Console.WriteLine($"Out of {numContests.ToString("n", format)} contests, these are the results:");
      Console.WriteLine($"A won: {results[Krank.Result.win].ToString("n", format)} times ({(results[Krank.Result.win] / (double)numContests).ToString("p")})");
      Console.WriteLine($"B won: {results[Krank.Result.lose].ToString("n", format)} times ({(results[Krank.Result.lose] / (double)numContests).ToString("p")})");
      Console.WriteLine($"Draw: {results[Krank.Result.draw].ToString("n", format)} times ({(results[Krank.Result.draw] / (double)numContests).ToString("p")})");
      Console.WriteLine($"Nobody won: {results[Krank.Result.nowin].ToString("n", format)} times ({(results[Krank.Result.nowin] / (double)numContests).ToString("p")})");

      Console.WriteLine(ThreadedRolling.delta);

      // Save results into csv file
      string line = String.Join(',', new double[]{
        ThreadedRolling.numDice,
        ThreadedRolling.numDiceB,
        numContests,
        results[Krank.Result.win],
        results[Krank.Result.lose],
        results[Krank.Result.draw],
        results[Krank.Result.nowin],
        });

      if (!File.Exists(csvFilename))
      {
        File.WriteAllText(csvFilename, "A-DICE,B-DICE,SIMULATED CONTESTS,A-WINS,A-LOSSES,DRAWS,NOWINS");
      }

      File.AppendAllText(csvFilename, "\n" + line);


      /*
      // Roll
      int numSuccessful = ThreadedRolling.StraightRolls();

      // Present the results

      int totalRolls = ThreadedRolling.numRolls * ThreadedRolling.numThreads;
      double percentSuccessful = ((double)numSuccessful / (double)(totalRolls));

      NumberFormatInfo formatInfo = new NumberFormatInfo {NumberGroupSeparator = " ", NumberDecimalDigits = 0};

      Console.WriteLine($"{totalRolls.ToString("n", formatInfo)} rolls, {numSuccessful.ToString("n", formatInfo)} were successful ({percentSuccessful.ToString("p")})");
      Console.WriteLine(ThreadedRolling.delta);
      */

      Console.ReadLine();
    }
  }
}
