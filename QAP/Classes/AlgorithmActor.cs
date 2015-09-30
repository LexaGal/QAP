using System;
using System.Collections.Generic;
using System.IO;
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

        public void Save(string pathOut)
        {
            if (_algorithm.Finished)
            {
                Tuple<string, List<int>> result = _algorithm.Result;

                using (TextWriter textWriter = new StreamWriter(pathOut, true))
                {
                    textWriter.WriteLine("\n" + result.Item1);
                    textWriter.Write("The best solution: ");

                    foreach (int p in result.Item2)
                    {
                        textWriter.Write("{0} ", p);
                    }
                    textWriter.Write("\n");
                }
            }
 
        }
    }
}