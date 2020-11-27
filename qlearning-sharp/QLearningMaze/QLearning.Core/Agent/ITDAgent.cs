namespace QLearning.Core.Agent
{
    using System;
    using System.Collections.Generic;
    using Environment;

    public interface ITDAgent<TEnvironment> : IAgent<TEnvironment>
        where TEnvironment : ITDEnvironment
    {
        
        /// <summary>
        /// Gets or Sets the maximum amount of times the Agent may move back and forth between two spaces before failing
        /// </summary>
        int MaximumAllowedBacktracks { get; set; }
        /// <summary>
        /// Gets or sets a list of stored successful training episodes
        /// </summary>
        List<TrainingSession> TrainingEpisodes { get; set; }
        /// <summary>
        /// Gets the best session from a training set, based on total score
        /// </summary>
        TrainingSession BestTrainingSession { get; }
        
    }
}
