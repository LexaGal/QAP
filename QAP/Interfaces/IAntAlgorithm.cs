namespace QAP.Interfaces
{
    public interface IAntAlgorithm
    {
        //Running
        void Start();
        //

        //Initializations 
        void InitializeAntColony();
        void ReinitializeGlobalMemory();
        //

        //Getting Solution Path
        void GetSolutionPath();
        //

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

        //Finishing
        bool IsFinished(int counter);
        //
    }
}