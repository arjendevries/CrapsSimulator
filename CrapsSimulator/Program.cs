using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CrapsSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rndOne = new Random();

            int numRuns = 0;
            bool test = false;

            if (args.Length > 0)
            {
                test = int.TryParse(args[0], out numRuns);
            }
            if (!test)
            {
                Console.WriteLine("Enter number of runs:");
                numRuns = int.Parse(Console.ReadLine());
            }

            Console.WriteLine($"Number of runs: {numRuns}\n");

            //delay instantiation of rndTwo for better results.
            Thread.Sleep(5000);
            Random rndTwo = new Random();

            List<Run> runs = new List<Run>();

            for (int i = 1; i <= numRuns; i++)
            {
                var run = RunRun(i, rndOne, rndTwo);
                runs.Add(run);
            }

            //DisplayRuns(runs);

            AnalyzeRuns(runs);

            Console.ReadLine();
        }
        public static void AnalyzeRuns(IEnumerable<Run> runs)
        {
            Dictionary<int, int> totalPointsFrequency = new Dictionary<int, int>();
            Dictionary<int, int> dieOneFrequency = new Dictionary<int, int>();
            Dictionary<int, int> dieTwoFrequency = new Dictionary<int, int>();
            Dictionary<int, int> diceTotalFrequency = new Dictionary<int, int>();

            int totalRuns = 0;
            int totalRolls = 0;

            foreach (var run in runs)
            {
                var totalPoints = run.TotalPointsHit;
                totalRuns++;
                //Total points analysis
                if (totalPointsFrequency.ContainsKey(totalPoints)) {
                    totalPointsFrequency[totalPoints]++;
                }
                else
                {
                    totalPointsFrequency.Add(totalPoints, 1);
                }

                //looking at all rolls in all runs
                foreach (var roll in run.AllRoles)
                {
                    var dieOne = roll.DieOne;
                    var dieTwo = roll.DieTwo;
                    var diceTotal = roll.DiceTotal;
                    totalRolls++;

                    // Die One
                    if (dieOneFrequency.ContainsKey(dieOne))
                    {
                        dieOneFrequency[dieOne]++;
                    }
                    else
                    {
                        dieOneFrequency.Add(dieOne, 1);
                    }
                    // Die Two
                    if (dieTwoFrequency.ContainsKey(dieTwo))
                    {
                        dieTwoFrequency[dieTwo]++;
                    }
                    else
                    {
                        dieTwoFrequency.Add(dieTwo, 1);
                    }
                    // Dice Total
                    if (diceTotalFrequency.ContainsKey(diceTotal)) {
                        diceTotalFrequency[diceTotal]++;
                    }
                    else
                    {
                        diceTotalFrequency.Add(diceTotal, 1);
                    }
                    
                }
            }

            //total points
            var sortedTotalPointFrequency = totalPointsFrequency.OrderBy(x => x.Key);

            foreach (var entry in sortedTotalPointFrequency)
            {
                double freq = (double)entry.Value * 100 / totalRuns;
                Console.WriteLine($"Total Points Hit:  {entry.Key} happened {entry.Value} times, {freq}%");
            }

            Console.WriteLine("\n");

            int cumulative = totalRuns;           

            // 1 or more, 2 or more
            foreach (var entry in sortedTotalPointFrequency)
            {
                double freq = (double)cumulative * 100 / totalRuns;

                if (entry.Key == 0)
                {
                    Console.WriteLine($"Total Points Hit:  {entry.Key} happened {entry.Value} times, {(double)entry.Value * 100 / totalRuns}%");
                }
                else
                {
                    Console.WriteLine($"Total Points Hit:  {entry.Key} or more happened {cumulative} times, {freq}%");
                }

                cumulative -= entry.Value;
            }

            Console.WriteLine("\n");

            Console.WriteLine($"Total Rolls: {totalRolls}");
            Console.WriteLine($"Avg Rolls per Run: { (double)totalRolls / totalRuns}\n");

            //Die one freq
            var sortedDieOneFrequency = dieOneFrequency.OrderBy(x => x.Key);

            foreach (var entry in sortedDieOneFrequency)
            {
                double freq = (double)entry.Value * 100 / totalRolls;
                Console.WriteLine($"Die One of {entry.Key} was rolled {entry.Value} times, {freq}%");
            }

            Console.WriteLine("\n");

            //Die two freq
            var sortedDieTwoFrequency = dieTwoFrequency.OrderBy(x => x.Key);

            foreach (var entry in sortedDieTwoFrequency)
            {
                double freq = (double)entry.Value * 100 / totalRolls;
                Console.WriteLine($"Die Two of {entry.Key} was rolled {entry.Value} times, {freq}%");
            }

            Console.WriteLine("\n");

            // Dice Total freq
            var sortedDiceTotalFrequency = diceTotalFrequency.OrderBy(x => x.Key);

            foreach (var entry in sortedDiceTotalFrequency)
            {
                double freq = (double)entry.Value * 100 / totalRolls;
                Console.WriteLine($"Dice Total of {entry.Key} was rolled {entry.Value} times, {freq}%");
            }

            Console.WriteLine("\n");


        }
        public static void DisplayRuns(IEnumerable<Run> runs)
        {
            Console.WriteLine();
            foreach (var run in runs)
            {
                Console.WriteLine($"\nrun {run.RunNumber}:  \n");
                Console.WriteLine($"Number of Points hit {run.TotalPointsHit}");
                Console.Write($"Points hit: ");
                run.AllRoles.Where(x => x.RollIsPointHit).ToList().ForEach(x => { Console.Write($"{x.DiceTotal}, "); });
                Console.WriteLine();
                Console.WriteLine($"Last Point established is {run.Point}");
                Console.Write("All Roles: ");
                foreach (var r in run.AllRoles)
                {
                    Console.Write($"{r.DiceTotal}, ");
                }
                Console.WriteLine("\n\n");
            }
        }
        public static Run RunRun(int runNum, Random rndOne, Random rndTwo)
        {
            List<int> points = new List<int>() { 4, 5, 6, 8, 9, 10 };

            var run = new Run();

            run.AllRoles = new List<Roll>();
            run.PointOn = false;
            run.SevensOut = false;
            run.RunNumber = runNum;

            while (!run.SevensOut)
            {
                //Come out rolls, point yet to be established
                while (!run.PointOn)
                {
                    var roll = RollDice(rndOne, rndTwo);
                    run.AllRoles.Add(roll);

                    if (points.Contains(roll.DiceTotal))
                    {
                        run.PointOn = true;
                        run.Point = roll.DiceTotal;
                    }
                }
                //point established
                while (run.PointOn)
                {
                    var roll = RollDice(rndOne, rndTwo);
                    run.AllRoles.Add(roll);

                    //Seven out, roller turns over/run over
                    if (roll.DiceTotal == 7)
                    {
                        run.PointOn = false;
                        run.SevensOut = true;
                    }
                    else if (roll.DiceTotal == run.Point)
                    {
                        run.PointOn = false;
                        run.TotalPointsHit++;

                        var lastRoll = run.AllRoles.LastOrDefault();
                        run.AllRoles.RemoveAt(run.AllRoles.Count() - 1);
                        lastRoll.RollIsPointHit = true;
                        run.AllRoles.Add(lastRoll);
                    }
                }
            }

            return run;
        }

        public static Roll RollDice(Random rndOne, Random rndTwo)
        {
            return new Roll
            {
                DieOne = rndOne.Next(1, 7),
                DieTwo = rndTwo.Next(1, 7)
            };
        }
    }
}
