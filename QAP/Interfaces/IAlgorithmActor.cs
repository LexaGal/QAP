namespace QAP.Interfaces
{
    internal interface IAlgorithmActor
    {
        void Load(string pathIn, string pathOut);
        void Save();
    }
}