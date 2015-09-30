using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Grsu.Lab.Aoc.Contracts;

namespace QAP.Classes
{
    public class StandartAntAlgorithm : IAlgorithm
    {
        public IGraph Graph { get; set; }

        public IList<IAnt> Ants { get; set; }

        public Random Rnd { get; set; }

        public int Alpha { get; set; }

        public int Beta { get; set; }

        public int Pheromone { get; set; }

        public IList<INode> MinPath { get; set; }

        public int MaxIterations { get; set; }

        public int CurrentIteration { get; set; }

        public int MaxIterationsNoChanges { get; set; }

        public int CurrentIterationNoChanges { get; set; }

        public static object DeepClone(object obj)
        {
            object objResult;
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, obj);

                ms.Position = 0;
                objResult = bf.Deserialize(ms);
            }
            return objResult;
        }

        public double GetPathValue(IList<INode> path)
        {
            var tValue = 0D;
            for (int currentStep = 1; currentStep < path.Count; currentStep++)
            {
                tValue += Graph.Edges.First(
                    edge => edge.Begin == path[currentStep - 1].Id
                            && edge.End == path[currentStep].Id).HeuristicInformation;
            }
            return tValue;
        }

        public void CheckForMinPath()
        {
            foreach (var ant in Ants)
            {
                if (GetPathValue(ant.VisitedNodes) < GetPathValue(MinPath))
                {
                    MinPath = (IList<INode>) DeepClone(ant.VisitedNodes);
                }
            }
        }

        public void CreateAnts()
        {
            foreach (var node in Graph.Nodes)
            {
                Ants.Add(new Ant {VisitedNodes = new List<INode>()});
                Ants.Last().VisitedNodes.Add(node);
            }
        }

        public void AntTravel(IAnt ant)
        {
            while (ant.VisitedNodes.Last() != ant.VisitedNodes.First() || ant.VisitedNodes.Count == 1)
            {
                ant.VisitedNodes.Add(GetNextNode(ant));
            }
        }

        public IDictionary<INode, double> GetProbsToNodes(IList<INode> nodes, INode currentPosition)
        {
            var tProbs = new Dictionary<INode, double>();
            foreach (var node in nodes)
            {
                INode currentNode = node;

                IEdge currentEdge =
                    Graph.Edges.First(edge => edge.Begin == currentPosition.Id && edge.End == currentNode.Id);

                double currentProb = currentEdge.Pheromone*Alpha + currentEdge.HeuristicInformation*Beta;

                tProbs.Add(currentNode, currentProb + tProbs.Values.Sum());
            }
            return tProbs;
        }

        public INode GetNextNode(IAnt ant)
        {
            IList<INode> allPossibleNodes = Graph.GetSibilings(ant.VisitedNodes.Last());

            foreach (var node in ant.VisitedNodes)
            {
                INode currentNode = node;
                if (allPossibleNodes.Contains(currentNode))
                {
                    allPossibleNodes.Remove(currentNode);
                }
            }

            IDictionary<INode, double> probsToGo = GetProbsToNodes(allPossibleNodes, ant.VisitedNodes.Last());

            if (probsToGo.Count == 0)
            {
                return ant.VisitedNodes.First();
            }

            double currentProb = Rnd.NextDouble()*probsToGo.Values.Last();

            foreach (var pair in probsToGo)
            {
                if (currentProb < pair.Value)
                {
                    return pair.Key;
                }
            }
            return ant.VisitedNodes.First();
        }

        public void UpdateGeneration()
        {
            foreach (var ant in Ants)
            {
                for (int currentStep = 1; currentStep < ant.VisitedNodes.Count; currentStep++)
                {
                    Graph.Edges.First(
                        edge => edge.Begin == ant.VisitedNodes[currentStep - 1].Id &&
                                edge.End == ant.VisitedNodes[currentStep].Id).Pheromone += Pheromone;
                }
            }
        }

        public bool IsFinished()
        {
            if (CurrentIteration == MaxIterations)
            {
                return true;
            }

            if (CurrentIterationNoChanges == MaxIterationsNoChanges)
            {
                return true;
            }

            return false;
        }

        public void Run()
        {
            while (!IsFinished())
            {
                CreateAnts();

                foreach (var ant in Ants)
                {
                    AntTravel(ant);
                }

                IList<INode> oldMinPath = MinPath;

                CheckForMinPath();

                if (Equals(GetPathValue(MinPath), GetPathValue(oldMinPath)))
                {
                    CurrentIterationNoChanges++;
                }
                else CurrentIterationNoChanges = 0;

                UpdateGeneration();

                CurrentIteration++;

                Ants.Clear();
            }
        }

        public string Result()
        {
            var tResult = new StringBuilder();
            tResult.Append(GetPathValue(MinPath)).Append(Environment.NewLine);
            for (int currentNode = 0; currentNode < MinPath.Count - 1; currentNode++)
            {
                tResult.Append(MinPath[currentNode].Id).Append(" - ");
            }
            tResult.Append(MinPath.Last().Id).Append(Environment.NewLine);
            tResult.Append(CurrentIteration).Append(Environment.NewLine);
            return tResult.ToString();
        }
    }
}