using System.Collections.Generic;
using Grsu.Lab.Aoc.Contracts;

namespace QAP.Classes
{
    public class Ant : IAnt
    {
        public IList<INode> VisitedNodes { get; set; }
    }
}