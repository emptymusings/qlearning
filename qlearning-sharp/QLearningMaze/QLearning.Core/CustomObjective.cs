namespace QLearning.Core
{
    public class CustomObjective
    {
        public int State { get; set; }
        public double Value { get; set; }
        public bool IsRequired { get; set; } = true;
    }
}
