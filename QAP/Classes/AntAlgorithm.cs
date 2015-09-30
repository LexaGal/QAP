using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using QAP.Classes;
using QAP.Interfaces;

namespace QAP.Classes
{
    internal class AntAlgorithm : IAntAlgorithm, IPathGenerator, IDataReader, ILogger
    {
        public bool Finished { get; private set; }
        public string PathIn { get; private set; }
        public string PathOut { get; private set; }
        public int PheromoneIncrement { get; private set; }
        public int ExtraPheromoneIncrement { get; private set; }
        public int NIterations { get; private set; }
        public int NAnts { get; private set; }
        public int PathCost { get; private set; }
        public int BestPathCost { get; private set; }
        public int NoUpdatesLimit { get; private set; }
        public int[,] DistanceMatrix { get; private set; }
        public int[,] FlowMatrix { get; private set; }
        public int[,] GlobalMemory { get; private set; }
        public int[] Path { get; private set; }
        public int[] BestPath { get; private set; }
        public Tuple<string, List<int>> Result;

        public AntAlgorithm(string pathIn, string pathOut)
        {
            PathIn = pathIn;
            PathOut = pathOut;
        }

        
        //==================== Running ============================

        public void Start()
        {
            InitializeAntColony();

            // Ants iterations
            int counter = 0;
            for (int iteration = 1; iteration <= NIterations; iteration++)
            {
                if (IsFinished(counter))
                {
                    Finished = true;
                    Result = new Tuple<string, List<int>>(String.Format("Finished as there were no updates during {0} iterations",
                        NoUpdatesLimit), BestPath.ToList());
                    return;
                }

                // Build a new solution
                GetSolutionPath();

                PathCost = ComputePathCost();

                // Improve solution with a local search
                LocalSearch();

                // Is best solution improved?
                if (PathCost < BestPathCost)
                {
                    counter = 0;

                    LogBestPath(iteration);

                    UpdateBestPath();

                    BestPathCost = PathCost;

                    PheromoneIncrement = 1;

                    ReinitializeGlobalMemory();
                }
                else
                {
                    counter++;

                    // Memory update
                    UpdateGlobalMemory();
                }
            }

            Finished = true;
            Result = new Tuple<string, List<int>>(String.Format("Finished after {0} iterations", NIterations), BestPath.ToList());
        }

        //=========================================================

        
        //==================== Initializing =======================

        public void ReadData()
        {
            using (TextReader textReader = new StreamReader(PathIn))
            {
                textReader.ReadLine();
                string s = textReader.ReadLine();
                if (!String.IsNullOrEmpty(s))
                {
                    PheromoneIncrement = int.Parse(s);
                }

                textReader.ReadLine();
                s = textReader.ReadLine();
                if (!String.IsNullOrEmpty(s))
                {
                    ExtraPheromoneIncrement = int.Parse(s);
                }

                textReader.ReadLine();
                s = textReader.ReadLine();
                if (!String.IsNullOrEmpty(s))
                {
                    NoUpdatesLimit = int.Parse(s);
                }

                textReader.ReadLine();
                s = textReader.ReadLine();
                if (!String.IsNullOrEmpty(s))
                {
                    NIterations = int.Parse(s);
                }

                textReader.ReadLine();
                s = textReader.ReadLine();
                if (!String.IsNullOrEmpty(s))
                {
                    NAnts = int.Parse(s);
                }

                DistanceMatrix = new int[NAnts, NAnts];
                FlowMatrix = new int[NAnts, NAnts];

                int i = 0;
                while (!String.IsNullOrEmpty(s))
                {
                    s = textReader.ReadLine();

                    if (!String.IsNullOrEmpty(s))
                    {
                        if (s == "Distances Matrix:" || s == "Flows Matrix:")
                        {
                            continue;
                        }

                        string[] strings = Regex.Split(s, @"\D+");

                        if (i < NAnts)
                        {
                            for (int j = 0; j < strings.Length; j++)
                            {
                                DistanceMatrix[i, j] = int.Parse(strings[j]);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < strings.Length; j++)
                            {
                                FlowMatrix[i - NAnts, j] = int.Parse(strings[j]);
                            }
                        }
                    }
                    i++;
                }
            }

            File.AppendAllText(PathOut, "\n============================== New Run ==============================\n");
            string[] lines = File.ReadAllLines(PathIn);
            File.AppendAllLines(PathOut, lines);
        }

        // memory management
        // (re-) initialization of the memory
        public void ReinitializeGlobalMemory()
        {
            for (int i = 0; i < NAnts; i++)
            {
                for (int j = 0; j < NAnts; j++)
                {
                    GlobalMemory[i, j] = PheromoneIncrement;
                }
            }
        }

        public void InitializeAntColony()
        {
            ReadData();

            Finished = false;
            PathCost = 0;
            BestPathCost = int.MaxValue;
            Path = new int[NAnts];
            BestPath = new int[NAnts];
            GlobalMemory = new int[NAnts, NAnts];

            ReinitializeGlobalMemory();
        }

        //=========================================================


        //==================== QAP Path Generating ================

        public double GetCoefficient()
        {
            int x10 = 12345;
            int x11 = 67890;
            int x12 = 13579;
            int x20 = 24680;
            int x21 = 98765;
            int x22 = 43210;
            const int m = 2147483647;
            const int m2 = 2145483479;
            const int a12 = 63308;
            const int q12 = 33921;
            const int r12 = 12979;
            const int a13 = -183326;
            const int q13 = 11714;
            const int r13 = 2883;
            const int a21 = 86098;
            const int q21 = 24919;
            const int r21 = 7417;
            const int a23 = -539608;
            const int q23 = 3976;
            const int r23 = 2071;
            const double coeff = 4.656612873077393e-10;

            int h = x10/q13;
            int p13 = (-1)*a13*(x10 - h*q13) - h*r13;
            h = x11/q12;
            int p12 = a12*(x11 - h*q12) - h*r12;

            if (p13 < 0)
            {
                p13 = p13 + m;
            }

            if (p12 < 0)
            {
                p12 = p12 + m;
            }

            x10 = x11;
            x11 = x12;
            x12 = p12 - p13;

            if (x12 < 0)
            {
                x12 = x12 + m;
            }

            h = x20/q23;
            int p23 = (-1)*a23*(x20 - h*q23) - h*r23;
            h = x22/q21;
            int p21 = a21*(x22 - h*q21) - h*r21;

            if (p23 < 0)
            {
                p23 = p23 + m2;
            }

            if (p21 < 0)
            {
                p21 = p21 + m2;
            }

            x20 = x21;
            x21 = x22;
            x22 = p21 - p23;

            if (x22 < 0)
            {
                x22 = x22 + m2;
            }

            if (x12 < x22)
            {
                h = x12 - x22 + m;
            }

            else h = x12 - x22;

            if (h == 0)
            {
                Console.WriteLine("h == 0\n");
                return 1.0;
            }

            return h*coeff;
        }

        public int GetPermutationalIndex(int low, int high)
        {
            int index = low + (int) ((high - low + 1)*GetCoefficient());
            return index;
        }

        // generate a random permutation Path   
        public void GeneratePath(int[] path)
        {
            for (int i = 0; i < NAnts; i++)
            {
                path[i] = i;
            }

            for (int i = 0; i < NAnts - 1; i++)
            {
                int x = GetPermutationalIndex(i, NAnts - 1);
                int y = path[i];
                path[i] = path[x];
                path[x] = y;
            }
        }

        //=========================================================


        //==================== Getting Path =======================

        // generate a solution with probability of setting facility j in Path[i] 
        // proportionnal to GlobalMemory[i, j]
        public void GetSolutionPath()
        {
            int[] nextI = new int[NAnts];
            int[] nextJ = new int[NAnts];
            int[] sumTrace = new int[NAnts];

            GeneratePath(nextI);
            GeneratePath(nextJ);

            for (int i = 0; i < NAnts; i++)
            {
                for (int j = 0; j < NAnts; j++)
                {
                    sumTrace[i] += GlobalMemory[i, j];
                }
            }

            for (int i = 0; i < NAnts; i++)
            {
                int j = i;

                int target = GetPermutationalIndex(0, sumTrace[nextI[i]] - 1);
                
                int sum = GlobalMemory[nextI[i], nextJ[j]];
               
                while (sum < target)
                {
                    j++;
                    sum += GlobalMemory[nextI[i], nextJ[j]];
                }

                Path[nextI[i]] = nextJ[j];

                for (int k = i; k < NAnts; k++)
                {
                    sumTrace[nextI[k]] -= GlobalMemory[nextI[k], nextJ[j]];
                }

                int y = nextJ[j];
                nextJ[j] = nextJ[i];
                nextJ[i] = y;

            }
        }

        //=========================================================


        //==================== Local search =======================

        // Scans the neighbourhood at most twice
        // Perform improvements as soon as they are found
        public void LocalSearch()
        {
            // set of moves, numbered from 0 to index
            int[] move = new int[NAnts*(NAnts - 1)/2];
            int nMoves = 0;

            for (int i = 0; i < NAnts - 1; i++)
            {
                for (int j = i + 1; j < NAnts; j++)
                {
                    move[nMoves++] = NAnts*i + j;
                }
            }

            bool isImproved = true;

            for (int scan = 0; scan < 2 && isImproved; scan++)
            {
                isImproved = false;
                
                for (int i = 0; i < nMoves - 1; i++)
                {
                    int x = GetPermutationalIndex(i + 1, nMoves - 1);
                    int y = move[i];
                    move[i] = move[x];
                    move[x] = y;
                }

                for (int i = 0; i < nMoves; i++)
                {
                    int r = move[i]/NAnts;
                    int s = move[i]%NAnts;
                    int moveCost = ComputeMoveCost(r, s);

                    if (moveCost < 0)
                    {
                        PathCost += moveCost;

                        int y = Path[r];
                        Path[r] = Path[s];
                        Path[s] = y;

                        isImproved = true;
                    }
                }
            }
        }

        //=========================================================


        //==================== Computing Costs ====================

        // compute the value of move (r -> s) on solution Path
        public int ComputeMoveCost(int r, int s)
        {
            int d = (DistanceMatrix[r, r] - DistanceMatrix[s, s]) *
                    (FlowMatrix[Path[s], Path[s]] - FlowMatrix[Path[r], Path[r]]) +
                    (DistanceMatrix[r, s] - DistanceMatrix[s, r]) *
                    (FlowMatrix[Path[s], Path[r]] - FlowMatrix[Path[r], Path[s]]);

            for (int k = 0; k < NAnts; k++)
            {
                if (k != r && k != s)
                {
                    d += (DistanceMatrix[k, r] - DistanceMatrix[k, s]) *
                         (FlowMatrix[Path[k], Path[s]] - FlowMatrix[Path[k], Path[r]]) +
                         (DistanceMatrix[r, k] - DistanceMatrix[s, k]) *
                         (FlowMatrix[Path[s], Path[k]] - FlowMatrix[Path[r], Path[k]]);
                }
            }
            return d;
        }

        // compute the Cost of solution Path
        public int ComputePathCost()
        {
            int c = 0;

            for (int i = 0; i < NAnts; i++)
            {
                for (int j = 0; j < NAnts; j++)
                {
                    c += DistanceMatrix[i, j] * FlowMatrix[Path[i], Path[j]];
                }
            }
            return c;
        }

        //=========================================================


        //==================== Updates ============================

        // memory update
        public void UpdateGlobalMemory()
        {
            int i = 0;

            while (i < NAnts && Path[i] == BestPath[i])
            {
                i++;
            }

            if (i == NAnts)
            {
                PheromoneIncrement++;
                ReinitializeGlobalMemory();
            }
            else
            {
                for (i = 0; i < NAnts; i++)
                {
                    GlobalMemory[i, Path[i]] += PheromoneIncrement;
                    GlobalMemory[i, BestPath[i]] += ExtraPheromoneIncrement;
                }
            }
        }

        public void UpdateBestPath()
        {
            for (int k = 0; k < NAnts; k++)
            {
                BestPath[k] = Path[k];
            }
        }

        //=========================================================


        //==================== Logging ============================

        public void LogBestPath(int iteration)
        {
            using (TextWriter textWriter = new StreamWriter(PathOut, true))
            {
                textWriter.Write("\nNew best solution found at iteration: {0}\nCost: {1}\nPath: ", iteration, PathCost);

                for (int i = 0; i < NAnts; i++)
                {
                    textWriter.Write("{0} ", Path[i]);
                }
                textWriter.Write("\n");
            }
        }

        //=========================================================


        //==================== Finishing ============================

        public bool IsFinished(int counter)
        {
            return counter == NoUpdatesLimit;
        }

        //=========================================================

    }
}
