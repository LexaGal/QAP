using System;
using System.Collections.Generic;
using QAP.Interfaces;

namespace QAP.Classes
{
    public class AlgorithmActor : IAlgorithmActor
    {
        private AntAlgorithm _algorithm;

        public void Load(string pathIn, string pathOut)
        {
            _algorithm = new AntAlgorithm(pathIn, pathOut);
            _algorithm.Start();
        }

        public void Save()
        {
            if (_algorithm.Finished)
            {
                Tuple<string, List<int>> result = _algorithm.Result;
            }
 
        }
    }
}