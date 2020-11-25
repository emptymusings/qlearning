namespace QLearning.Core.Environment
{
    public class CustomObjective
    {
        public int State { get; set; }
        public double Value { get; set; }
        public bool IsRequired { get; set; } = true;
        public int Priority { get; set; }
        public int DistanceFromStart { get; set; }
        public int DistanceFromEnd { get; set; }
    }
}
