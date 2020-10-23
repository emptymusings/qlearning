using System;
using System.Collections.Generic;
using System.Text;

namespace QLearningMaze.Core.Mazes
{
    public abstract partial class MazeBase : IMaze
    {
        public event EventHandler MazeCreatingEventHandler;

        public event EventHandler MazeCreatedEventHandler;

        public event EventHandler RewardsCreatedEventHandler;

        public event EventHandler QualityCreatedEventHandler;

        public event EventHandler<ObstructionEventArgs> ObstructionAddedEventHandler;

        public event EventHandler<ObstructionEventArgs> ObstructionRemovedEventHandler;

        public event EventHandler<int> AgentStateChangedEventHandler;

        public event EventHandler<(int newState, int previousState)> TrainingAgentStateChangingEventHandler;

        public event EventHandler<bool> TrainingStatusChangedEventHandler;

        public event EventHandler<TrainingEpochCompletedEventArgs> TrainingEpochCompletedEventHandler;

        protected virtual void OnMazeCreatingEventHandler()
        {
            EventHandler handler = MazeCreatingEventHandler;
            handler?.Invoke(this, new EventArgs());
        }

        protected virtual void OnMazeCreatedEventhHandler()
        {
            EventHandler handler = MazeCreatedEventHandler;
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

        protected virtual void OnAgentStateChanged(int newState)
        {
            EventHandler<int> handler = AgentStateChangedEventHandler;
            handler?.Invoke(this, newState);
        }

        protected virtual void OnTrainingAgentStateChanging(int newState, int previousState)
        {
            EventHandler<(int newState, int previousState)> handler = TrainingAgentStateChangingEventHandler;
            handler?.Invoke(this, (newState, previousState));
        }

        protected virtual void OnTrainingStatusChanged(bool isTraining)
        {
            EventHandler<bool> handler = TrainingStatusChangedEventHandler;
            handler?.Invoke(this, isTraining);
        }

        protected virtual void OnTrainingEpochCompleted(TrainingEpochCompletedEventArgs e)
        {
            EventHandler<TrainingEpochCompletedEventArgs> handler = TrainingEpochCompletedEventHandler;
            handler?.Invoke(this, e);
        }
    }

    public class TrainingEpochCompletedEventArgs : EventArgs
    {
        public int CurrentEpoch { get; set; }
        public int TotalEpochs { get; set; }
    }

    public class ObstructionEventArgs : EventArgs
    {
        public MazeObstruction Obstruction { get; set; }
    }
}
