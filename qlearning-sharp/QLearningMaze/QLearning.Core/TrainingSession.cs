namespace QLearning.Core
{
    public class TrainingSession
    {
        public int Episode { get; set; }
        public int Moves { get; set; }
        public double Score { get; set; }
        public double[][] Quality { get; set; }
    }
}
