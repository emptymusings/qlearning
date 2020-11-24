using System;
using System.Collections.Generic;
using System.Text;

namespace QLearning.Core
{
    public abstract partial class QEnvironmentBase : IQEnvironment
    {
        public event EventHandler StateTableCreating;
        public event EventHandler StateTableCreated;
        public event EventHandler RewardTableCreating;
        public event EventHandler RewardTableCreated;
        public event EventHandler QualityTableCreating;
        public event EventHandler QualityTableCreated;
        public event EventHandler<AgentStateChangedEventArgs> AgentStateChanged;
        public event EventHandler<TrainingAgentStateChangedEventArgs> TrainingAgentStateChanged;
        public event EventHandler<TrainingEpisodeCompletedEventArgs> TrainingEpisodeCompleted;
        public event EventHandler<bool> TrainingStateChanged;
        public event EventHandler<AgentCompletedEventArgs> AgentCompleted;

        protected virtual void OnStateTableCreating()
        {
            EventHandler handler = StateTableCreating;
            handler?.Invoke(this, new EventArgs());
        }

        protected virtual void OnStateTableCreated()
        {
            EventHandler handler = StateTableCreated;
            handler?.Invoke(this, new EventArgs());
        }

        protected virtual void OnRewardTableCreating()
        {
            EventHandler handler = RewardTableCreating;
            handler?.Invoke(this, new EventArgs());
        }

        protected virtual void OnRewardTableCreated()
        {
            EventHandler handler = RewardTableCreated;
            handler?.Invoke(this, new EventArgs());
        }

        protected virtual void OnQualityTableCreating()
        {
            EventHandler handler = QualityTableCreating;
            handler?.Invoke(this, new EventArgs());
        }

        protected virtual void OnQualityTableCreated()
        {
            EventHandler handler = QualityTableCreated;
            handler?.Invoke(this, new EventArgs());
        }

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

        protected virtual void OnTrainingEpisodeCompleted(int currentEpisode, int totalEpisodes, int totalMoves, double totalScore, bool succeeded)
        {
            EventHandler<TrainingEpisodeCompletedEventArgs> handler = TrainingEpisodeCompleted;
            handler?.Invoke(this, new TrainingEpisodeCompletedEventArgs(currentEpisode, totalEpisodes, totalMoves, totalScore, succeeded));
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
