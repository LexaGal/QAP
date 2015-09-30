using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grsu.Lab.Aoc.Contracts;

namespace QAP.Classes
{
    public class AlgorithmCreator
    {
        public IGraph CreatorGraph = new Graph();

        public IList<IAnt> CreatorAnts = new List<IAnt>();

        public Random CreatorRnd = new Random();

        public int CreatorAlpha;

        public int CreatorBeta;

        public int CreatorPheromone;

        public int CreatorStartPheromone;

        public IList<Node> CreatorMinPath = new List<Node>();

        public int CreatorMaxIterations;

        public int CreatorMaxIterationsNoChanges;

        public int CreatorNForBig;

        public AlgorithmCreator(Stream stream, int alpha, int beta, int pheromone, int startPheromone, int maxIterNc,
            int NForBig)
        {
            ((Graph) CreatorGraph).NForBig = NForBig;
            CreatorGraph.LoadGraph(stream);
            CreatorAlpha = alpha;
            CreatorBeta = beta;
            CreatorPheromone = pheromone;
            CreatorStartPheromone = startPheromone;
            CreatorMaxIterationsNoChanges = maxIterNc;
        }

        public IAlgorithm CreateStandartAlgorithm()
        {
            IList<INode> tMinPath = (IList<INode>) StandartAntAlgorithm.DeepClone(CreatorGraph.Nodes);

            tMinPath.Add(tMinPath.First());

            return new StandartAntAlgorithm
            {
                Graph = CreatorGraph,
                Alpha = CreatorAlpha,
                Ants = CreatorAnts,
                Beta = CreatorBeta,
                CurrentIteration = 0,
                CurrentIterationNoChanges = 0,
                MaxIterations = CreatorGraph.Nodes.Count*CreatorGraph.Nodes.Count,
                MaxIterationsNoChanges = CreatorMaxIterationsNoChanges,
                MinPath = CreatorGraph.Nodes,
                Pheromone = CreatorPheromone,
                Rnd = CreatorRnd
            };
        }
    }
}