using System.ComponentModel;
using QAP.Classes;

namespace QAP
{
    internal class Program
    {
        private const string Path = @"c:\users\alex\documents\GitHub\QAP\QAP\txt\";
        private static AlgorithmActor _actor;

        private static void Initialize()
        {
            _actor = new AlgorithmActor();
        }

        private static void Main()
        {
            Initialize();

            _actor.Load(Path + "MatrixData.txt", Path + "BestPathes.txt");
            _actor.Save(Path + "BestPathes.txt");
        }
    }
}
