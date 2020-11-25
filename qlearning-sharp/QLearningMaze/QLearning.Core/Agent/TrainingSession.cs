namespace QLearning.Core.Agent
{
    public class TrainingSession
    {
        public int Episode { get; set; }
        public int Moves { get; set; }
        public double Score { get; set; }
        public double[][] Quality { get; set; }
    }
}
