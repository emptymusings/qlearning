using System;
using System.Collections.Generic;
using System.Text;

namespace QLearningMaze.Core.Mazes
{
    public abstract partial class MazeBase : IMaze
    {
        public event EventHandler ObservationSpaceCreatingEventHandler;

        public event EventHandler ObservationSpaceCreatedEventHandler;

        public event EventHandler RewardsCreatedEventHandler;

        public event EventHandler QualityCreatedEventHandler;

        public event EventHandler<ObstructionEventArgs> ObstructionAddedEventHandler;

        public event EventHandler<ObstructionEventArgs> ObstructionRemovedEventHandler;

        public event EventHandler<AgentStateChangedEventArgs> AgentStateChangedEventHandler;

        public event EventHandler<TrainingAgentStateChangedEventArgs> TrainingAgentStateChangingEventHandler;

        public event EventHandler<bool> TrainingStatusChangedEventHandler;

        public event EventHandler<TrainingEpisodeCompletedEventArgs> TrainingEpisodeCompletedEventHandler;
        public event EventHandler<AgentCompletedMazeEventArgs> AgentCompletedMazeEventHandler;

        protected virtual void OnObservationSpaceCreating()
        {
            EventHandler handler = ObservationSpaceCreatingEventHandler;
            handler?.Invoke(this, new EventArgs());
        }

        protected virtual void OnObservationSpaceCreated()
        {
            EventHandler handler = ObservationSpaceCreatedEventHandler;
            handler?.Invoke(this, new EventArgs());
        }

        protected virtual void OnRewardsCreated()
        {
            EventHandler handler = RewardsCreatedEventHandler;
            handler?.Invoke(this, new EventArgs());
        }

        protected virtual void OnQualityCreated()
        {
            EventHandler handler = QualityCreatedEventHandler;
            handler?.Invoke(this, new EventArgs());
        }

        protected virtual void OnObstructionAdded(MazeObstruction obstruction)
        {
            EventHandler<ObstructionEventArgs> handler = ObstructionAddedEventHandler;
            handler?.Invoke(this, new ObstructionEventArgs { Obstruction = obstruction });
        }

        protected virtual void OnObstructionRemoved(MazeObstruction obstruction)
        {
            EventHandler<ObstructionEventArgs> handler = ObstructionRemovedEventHandler;
            handler?.Invoke(this, new ObstructionEventArgs { Obstruction = obstruction });
        }

        protected virtual void OnAgentStateChanged(int newPosition, int newState, double totalRewards, int totalMoves)
        {
            EventHandler<AgentStateChangedEventArgs> handler = AgentStateChangedEventHandler;
            handler?.Invoke(this, new AgentStateChangedEventArgs(newPosition, newState, totalMoves, totalRewards));
        }

        protected virtual void OnTrainingAgentStateChanging(int action, int state, int position, double newQuality, double oldQuality)
        {
            EventHandler<TrainingAgentStateChangedEventArgs> handler = TrainingAgentStateChangingEventHandler;
            handler?.Invoke(this, new TrainingAgentStateChangedEventArgs(action, state, position, newQuality, oldQuality));
        }

        protected virtual void OnTrainingStatusChanged(bool isTraining)
        {
            EventHandler<bool> handler = TrainingStatusChangedEventHandler;
            handler?.Invoke(this, isTraining);
        }

        protected virtual void OnTrainingEpisodeCompleted(TrainingEpisodeCompletedEventArgs e)
        {
            EventHandler<TrainingEpisodeCompletedEventArgs> handler = TrainingEpisodeCompletedEventHandler;
            handler?.Invoke(this, e);
        }

        protected virtual void OnAgentCompletedMaze(AgentCompletedMazeEventArgs e)
        {
            EventHandler<AgentCompletedMazeEventArgs> handler = AgentCompletedMazeEventHandler;
            handler?.Invoke(this, e);
        }
    }

}
