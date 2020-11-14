namespace QLearningMaze.Core
{
    using System;
    using Mazes;

    public class TrainingEpisodeCompletedEventArgs : EventArgs
    {
        public TrainingEpisodeCompletedEventArgs(int currentEpisode, int totalEpisodes, int totalMoves, double totalScore, bool succeeded)
        {
            CurrentEpisode = currentEpisode;
            TotalEpisodes = totalEpisodes;
            TotalMoves = totalMoves;
            TotalScore = totalScore;
            Success = succeeded;

        }

        public int CurrentEpisode { get; set; }
        public int TotalEpisodes { get; set; }
        public int TotalMoves { get; set; }
        public double TotalScore { get; set; }
        public bool Success { get; set; }
    }

    public class ObstructionEventArgs : EventArgs
    {
        public MazeObstruction Obstruction { get; set; }
    }
    
    public class AgentStateChangedEventArgs : EventArgs
    {
        public AgentStateChangedEventArgs(int newPosition, int newState, int movesMade, double rewardsEarned)
        {
            this.NewPosition = newPosition;
            this. RewardsEarned = rewardsEarned;
            this.MovesMade = movesMade;
        }

        public int NewPosition { get; set; }
        public int NewState { get; set; }
        public double RewardsEarned { get; set; }
        public int MovesMade { get; set; }
        
    }

    public class TrainingAgentStateChangedEventArgs : EventArgs
    {
        public TrainingAgentStateChangedEventArgs(
            int action, 
            int state,
            int position,
            double newQuality,
            double oldQuality)
        {
            Action = action;
            State = state;
            Position = position;
            NewQuality = newQuality;
            OldQuality = oldQuality;
        }
        public int Action { get; private set; }
        public int State { get; private set; }
        public int Position { get; private set; }
        public double NewQuality { get; private set; }
        public double OldQuality { get; private set; }
    }

    public class AgentCompletedMazeEventArgs : EventArgs
    {
        public AgentCompletedMazeEventArgs(int moves, double rewards)
        {
            Moves = moves;
            Rewards = rewards;
        }

        public int Moves { get; set; }
        public double Rewards { get; set; }
    }
}
