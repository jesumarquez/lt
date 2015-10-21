namespace Logictracker.Routing.Client
{
    public class ProblemType
    {
        public ProblemFleetSize FleetSize { get; set; }
        public ProblemFleetComposition FleetComposition { get; set; } 
    }

    public enum ProblemFleetSize
    {
        Infinite, Finite
    }

    public enum ProblemFleetComposition
    {
        Homogeneous, Heterogeneous
    }
}