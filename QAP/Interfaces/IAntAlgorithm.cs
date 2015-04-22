namespace QAP.Interfaces
{
    public interface IAntAlgorithm
    {
        void Start();

        //Initialization
        void ReadData();
        void InitializeAntColony();
        void InitializeGlobalMemory();
        //

        //Generating Solution
        void GenerateSolutionPath();

        //Computing Costs
        int ComputePathCost();
        int ComputeMoveCost(int r, int s);
        //

        //Local Search
        void LocalSearch();
        //

        //Updates
        void UpdateBestPath();
        void UpdateGlobalMemory();
        //

        //Logging
        void LogBestPath(int iteration);
        //
        
        //Finishing
        bool IsFinished(int counter);
        //
    }
}