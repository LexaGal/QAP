namespace QAP.Interfaces
{
    public interface IPathGenerator
    {
        double GetCoefficient();
        int GetPermutationalIndex(int low, int high);
        void GeneratePath(int[] path);
    }
}