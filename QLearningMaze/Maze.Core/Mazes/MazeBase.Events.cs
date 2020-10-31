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

        public event EventHandler<(int newState, int previousState, double newQuality, double oldQuality)> TrainingAgentStateChangingEventHandler;

        public event EventHandler<bool> TrainingStatusChangedEventHandler;

        public event EventHandler<TrainingEpisodeCompletedEventArgs> TrainingEpisodeCompletedEventHandler;

        protected virtual void OnObservationSpaceCreatingEventHandler()
        {
            EventHandler handler = ObservationSpaceCreatingEventHandler;
            handler?.Invoke(this, new EventArgs());
        }

        protected virtual void OnObservationSpaceCreatedEventhHandler()
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

        protected virtual void OnAgentStateChanged(int newPosition, double totalRewards, int totalMoves)
        {
            EventHandler<AgentStateChangedEventArgs> handler = AgentStateChangedEventHandler;
            handler?.Invoke(this, new AgentStateChangedEventArgs(newPosition, totalMoves, totalRewards));
        }

        protected virtual void OnTrainingAgentStateChanging(int newState, int previousState, double newQuality, double oldQuality)
        {
            EventHandler<(int newState, int previousState, double newQuality, double oldQuality)> handler = TrainingAgentStateChangingEventHandler;
            handler?.Invoke(this, (newState, previousState, newQuality, oldQuality));
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
    }

}
