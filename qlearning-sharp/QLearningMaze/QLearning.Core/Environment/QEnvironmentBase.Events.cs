namespace QLearning.Core.Environment
{
    using System;

    public abstract partial class QEnvironmentBase : IQEnvironment
    {
        public event EventHandler StateTableCreating;
        public event EventHandler StateTableCreated;
        public event EventHandler RewardTableCreating;
        public event EventHandler RewardTableCreated;
        public event EventHandler QualityTableCreating;
        public event EventHandler QualityTableCreated;

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
    }
}
