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
      // Setup
      ThreadedRolling.numRolls = 100000000; // per thread
      ThreadedRolling.numDice = 3;
      ThreadedRolling.numThreads = 16;

      Krank.useChaosDie = true;

      // Roll
      int numSuccessful = ThreadedRolling.StraightRolls();

      // Present the results

      int totalRolls = ThreadedRolling.numRolls * ThreadedRolling.numThreads;
      double percentSuccessful = ((double)numSuccessful / (double)(totalRolls));

      NumberFormatInfo formatInfo = new NumberFormatInfo {NumberGroupSeparator = " ", NumberDecimalDigits = 0};

      Console.WriteLine($"{totalRolls.ToString("n", formatInfo)} rolls, {numSuccessful.ToString("n", formatInfo)} were successful ({percentSuccessful.ToString("p")})");
      Console.WriteLine(ThreadedRolling.delta);

      Console.ReadLine();
    }    
  }
}
