using System;
using System.Collections.Generic;

namespace KrankProbabilities
{
  public class Krank
  {
    public static int successMinValue = 4;
    public static int maxRecursion = 20;
    public static Random generator = new Random();
    public static bool useChaosDie = true;

    public enum Result
    {
      win,
      lose,
      draw,
      nowin
    }

    public static Dictionary<Result, int> ManyContests(int numContests, int numDiceA, int numDiceB)
    {
      return ManyContests(numContests, numDiceA, numDiceB, generator);
    }

    public static Dictionary<Result, int> ManyContests(int numContests, int numDiceA, int numDiceB, Random generator)
    {
      Dictionary<Result, int> results = new Dictionary<Result, int>();
      results.Add(Result.win, 0);
      results.Add(Result.lose, 0);
      results.Add(Result.draw, 0);
      results.Add(Result.nowin, 0);

      for (int i = 0; i < numContests; i++)
      {
        Result result = Contest(numDiceA, numDiceB, generator);
        results[result]++;
      }

      return results;
    }

    public static Result Contest(int numDiceA, int numDiceB)
    {
      return Contest(numDiceA, numDiceB, generator);
    }

    public static Result Contest(int numDiceA, int numDiceB, Random generator)
    {
      (int successes, List<int> results) resultA = DieRoll(numDiceA, generator);
      (int successes, List<int> results) resultB = DieRoll(numDiceB, generator);

      // Console.WriteLine("A results: " + String.Join(",", resultA.results));
      // Console.WriteLine("B results: " + String.Join(",", resultB.results));

      if (resultA.successes == 0 && resultB.successes == 0)
      {
        return Result.nowin;
      }

      if (resultA.successes > resultB.successes)
      {
        return Result.win;
      }
      else if (resultA.successes < resultB.successes)
      {
        return Result.lose;
      }
      else if (numDiceA != 0 && numDiceB != 0)
      {
        // compare chaos die results
        if (resultA.results[numDiceA - 1] > resultB.results[numDiceB - 1])
        {
          return Result.win;
        }
        else if (resultA.results[numDiceA - 1] < resultB.results[numDiceB - 1])
        {
          return Result.lose;
        }
      }
      
      // Give up & accept a draw
      return Result.draw;
    }

    public static int ManyDieRoll(int numRolls, int numDice)
    {
      return ManyDieRoll(numRolls, numDice, generator);
    }

    public static int ManyDieRoll(int numRolls, int numDice, Random generator)
    {
      int numSuccessful = 0;

      for (int i = 0; i < numRolls; i++)
      {
        (int successes, List<int> results) dieRollResults = DieRoll(numDice, generator);

        numSuccessful += dieRollResults.successes > 0 ? 1 : 0;

      }

      return numSuccessful;
    }


    // TODO: Test speed if using List<int> instead of int[] as return value
    public static (int, List<int>) DieRoll(int numDice)
    {
      return DieRoll(numDice, generator);
    }

    public static (int, List<int>) DieRoll(int numOfDice, Random generator)
    {

      List<int> results = new List<int>();
      int successes = 0;
      int recursion = 0;

      if (numOfDice == 0)
      {
        int dieResult = generator.Next(1, 7);
        results.Add(dieResult);

        if (dieResult == 6)
        {
          successes++;
        }
      }

      for (int i = 0; i < numOfDice; i++)
      {

        int dieResult = generator.Next(1, 7);
        results.Add(dieResult);
        if (dieResult >= successMinValue)
        {
          successes++;
        }

        // Chaos die
        while (useChaosDie && i == numOfDice - 1 && dieResult == 6 && recursion < maxRecursion)
        {
          dieResult = generator.Next(1, 7);
          results.Add(dieResult);
          if (dieResult >= successMinValue)
          {
            successes++;
          }
        }

      }

      return (successes, results);
    }
  }
}
