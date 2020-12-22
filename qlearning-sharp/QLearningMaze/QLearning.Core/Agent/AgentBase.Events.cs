namespace QLearning.Core.Agent
{
    using Environment;

    using System;

    public abstract partial class AgentBase<TEnvironment> : IAgent<TEnvironment>
        where TEnvironment : IRLEnvironment
    {
        public event EventHandler<AgentStateChangedEventArgs> AgentStateChanged;
        public event EventHandler<TrainingAgentStateChangedEventArgs> TrainingAgentStateChanged;
        public event EventHandler<TrainingEpisodeCompletedEventArgs> TrainingEpisodeCompleted;
        public event EventHandler<bool> TrainingStateChanged;
        public event EventHandler<AgentCompletedEventArgs> AgentCompleted;


        protected virtual void OnAgentStateChanged(int newState, int movesMade, double rewardsEarned)
        {
            EventHandler<AgentStateChangedEventArgs> handler = AgentStateChanged;
            handler?.Invoke(this, new AgentStateChangedEventArgs(newState, movesMade, rewardsEarned));
        }

        protected virtual void OnTrainingAgentStateChanged(int action, int newState, int movesMade, double rewardsEarned, double newQuality, double oldQuality)
        {
            EventHandler<TrainingAgentStateChangedEventArgs> handler = TrainingAgentStateChanged;
            handler?.Invoke(this, new TrainingAgentStateChangedEventArgs(action, newState, movesMade, rewardsEarned, newQuality, oldQuality));
        }

        protected virtual void OnTrainingEpisodeCompleted(int currentEpisode, int totalEpisodes, int startPoint, int totalMoves, double totalScore, bool succeeded)
        {
            EventHandler<TrainingEpisodeCompletedEventArgs> handler = TrainingEpisodeCompleted;
            handler?.Invoke(this, new TrainingEpisodeCompletedEventArgs(currentEpisode, totalEpisodes, startPoint, totalMoves, totalScore, succeeded));
        }

        protected virtual void OnTrainingStateChanged(bool isTraining)
        {
            EventHandler<bool> handler = TrainingStateChanged;
            handler?.Invoke(this, isTraining);
        }

        protected virtual void OnAgentCompleted(int moves, double rewards, bool success)
        {
            EventHandler<AgentCompletedEventArgs> handler = AgentCompleted;
            handler?.Invoke(this, new AgentCompletedEventArgs(moves, rewards, success));
        }

    }
}
