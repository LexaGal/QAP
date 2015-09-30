using System;
using Grsu.Lab.Aoc.Contracts;

namespace QAP.Classes
{
    [Serializable]
    public class Node : INode
    {
        public int Id { get; set; }

        public bool Equals(INode other)
        {
            return Id == other.Id;
        }
    }
}