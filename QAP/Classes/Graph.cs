using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grsu.Lab.Aoc.Contracts;

namespace QAP.Classes
{
    [Serializable]
    public class Graph :  IGraph
    {
        public IList<IEdge> Edges { get; set; }
        public IList<INode> Nodes { get; set; }
        public int[,] RawGraph { get; set; }
        public int NForBig { get; set; }

        public IList<INode> GetSibilings(INode node)
        {
            List<INode> retNodes = new List<INode>();

            if (Nodes.Count < NForBig)
            {
                for (int currentStep = 0; currentStep < Nodes.Count; currentStep++)
                {
                    if (RawGraph[node.Id, currentStep] != int.MaxValue)
                    {
                        retNodes.Add(new Node { Id = currentStep });
                    }
                }
            }
            else
            {
                retNodes.AddRange(Edges.Where(edge => edge.Begin == node.Id).Select(edge => new Node { Id = edge.End }));
            }
            return retNodes;
        }

        public void LoadGraph(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            IList<string> data = new List<string>();
            Nodes = new List<INode>();
            Edges = new List<IEdge>();

            while (reader.Peek() >= 0)
            {
                data.Add(reader.ReadLine());
            }

            int numberOfNodes = int.Parse(data[0]);

            foreach (var currentNumber in Enumerable.Range(0, numberOfNodes))
            {
                Nodes.Add(new Node { Id = currentNumber });
            }

            if (numberOfNodes < NForBig)
            {
                RawGraph = new int[numberOfNodes, numberOfNodes];

                for (int i = 1; i < data.Count; i++)
                {
                    int currentString = i - 1;
                    string[] matrixUnits = data[i].Split(' ');

                    for (int currentColumn = 0; currentColumn < numberOfNodes; currentColumn++)
                    {
                        int tValue = int.Parse(matrixUnits[currentColumn]);

                        //-1 - edge with no path. 
                        if (tValue == 0)
                        {
                            tValue = int.MaxValue;
                        }

                        RawGraph[currentString, currentColumn] = tValue;

                        Edges.Add(new Edge { Begin = currentString, End = currentColumn, HeuristicInformation = tValue });
                    }
                }

            }
            else
            {
                for (int i = 1; i < data.Count; i++)
                {
                    int currentStartNode = i - 1;
                    string[] dataUnits = data[i].Split(' ');

                    foreach (var dataUnit in dataUnits)
                    {
                        string[] separatedData = dataUnit.Split(':');

                        Edges.Add(
                            new Edge
                            {
                                Begin = currentStartNode,
                                End = int.Parse(separatedData[0]),
                                HeuristicInformation = int.Parse(separatedData[1])
                            });
                    }
                }
            }
        }
    }
}