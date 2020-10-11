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


    public static int ManyDieRoll(int numRolls, int numDice)
    {
      return ManyDieRoll(numRolls, numDice, generator);
    }

    public static int ManyDieRoll(int numRolls, int numDice, Random generator)
    {
      int numSuccessful = 0;

      for (int i = 0; i < numRolls; i++)
      {
        (int successes, int[] results) dieRollResults = DieRoll(numDice, generator);

        numSuccessful += dieRollResults.successes > 0 ? 1 : 0;

      }

      return numSuccessful;
    }

    public static (int, int[]) DieRoll(int numDice)
    {
      return DieRoll(numDice, generator);
    }

    public static (int, int[]) DieRoll(int numOfDice, Random generator)
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

      return (successes, results.ToArray());
    }
  }
}
