namespace QLearning.Core.Agent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Environment;

    public partial class QAgent<TEnvironment> : AgentBase<TEnvironment>, IQAgent<TEnvironment>
        where TEnvironment : IQEnvironment
    {
        private Random _random = new Random();
        

        public QAgent() { }

        public QAgent(
            TEnvironment environment,
            double learningRate,
            double discountRate,
            int numberOfTrainingEpisodes,
            int maximumAllowedMoves = 1000,
            int maximumAllowedBacktracks = -1)
        {
            Environment = environment;
            NumberOfTrainingEpisodes = numberOfTrainingEpisodes;
            LearningRate = learningRate;
            DiscountRate = discountRate;
            MaximumAllowedMoves = maximumAllowedMoves;
            MaximumAllowedBacktracks = maximumAllowedBacktracks;
        }

        public TEnvironment Environment { get; set; }
        public int NumberOfTrainingEpisodes { get; set; }
        /// <summary>
        /// Gets or Sets the Discount Rate, or Discount Factor, used in the Q-Learning formula (aka gamma)
        /// </summary>
        public double DiscountRate { get; set; }
        /// <summary>
        /// Gets or Sets the Learning Rate used in the Q-Learning formula (aka alpha)
        /// </summary>
        public double LearningRate { get; set; }
        /// <summary>
        /// Gets or Sets the episode in which epsilon (greedy strategy) decay will start.  Also used when calculating epsilon's decay value
        /// </summary>
        public double EpsilonDecayStart { get; set; }
        /// <summary>
        /// Gets or Sets the the episode in which epsilon (greedy strategy) decay will end.  Also used when calculating epsilon's decay value
        /// </summary>
        public double EpsilonDecayEnd { get; set; }
        public double EpsilonDecayValue { get; set; } = -1;
        public virtual List<TrainingSession> TrainingEpisodes { get; set; }
        public virtual TrainingSession BestTrainingSession
        {
            get
            {
                if (TrainingEpisodes == null ||
                    TrainingEpisodes.Count == 0)
                {
                    return null;
                }

                return TrainingEpisodes.OrderByDescending(r => r.Score).FirstOrDefault();
            }
        }

        public int State { get; set; }
        public int Moves { get; set; }
        public int MaximumAllowedMoves { get; set; }
        public int MaximumAllowedBacktracks { get; set; }
        public double Score { get; set; }

        public virtual void Run(int fromState)
        {
            Run(fromState, false);
        }

        public virtual void Run(int fromState, bool overrideBaseEvents)
        {

            int action = -1;
            int previousState = -1;
            int numberOfBacktracks = 0;
            bool done = false;

            Moves = 0;
            Score = 0;

            while (!done)
            {
                if (Environment.QualityTable == null)
                    throw new InvalidOperationException($"The Q-table has not been initialized.  Train the agent first");

                action = Environment.GetPreferredNextAction(fromState);

                var stepValues = Environment.Step(fromState, action);

                State = stepValues.newState;

                if (Environment.Step(fromState, action).newState < 0)
                {
                    OnAgentCompleted(Moves, Score, (Environment.ObjectiveStates.Contains(fromState)));
                    throw new InvalidOperationException($"I guess I didn't learn very well.  Please try training again (perhaps adjusing the learning rate, discount rate, and/or episode count)");
                }

                Score += stepValues.reward;
                Moves++;

                if (Moves > MaximumAllowedMoves)
                {
                    OnAgentCompleted(Moves, Score, (Environment.ObjectiveStates.Contains(fromState)));
                    throw new InvalidOperationException($"Something's gone wrong, I've wandered around far too many times");
                }

                if (State == previousState)
                {
                    if (MaximumAllowedBacktracks >= 0 &&
                        numberOfBacktracks > MaximumAllowedBacktracks)
                    {
                        OnAgentCompleted(Moves, Score, (Environment.ObjectiveStates.Contains(fromState)));
                        throw new InvalidOperationException($"The agent has exceeded the maximum number of backtracks: {MaximumAllowedBacktracks}");
                    }
                    else
                    {
                        numberOfBacktracks++;
                    }
                }

                previousState = fromState;
                fromState = State;

                if (!overrideBaseEvents)
                    OnAgentStateChanged(fromState, Moves, Score);

                if (Environment.IsTerminalState(fromState, action, Moves, MaximumAllowedMoves))
                {
                    done = true;
                }
            }

            if (!overrideBaseEvents)
                OnAgentCompleted(Moves, Score, (Environment.ObjectiveStates.Contains(fromState)));
        }

        public virtual void Train()
        {
            Train(false);
        }

        public virtual void Train(bool overrideBaseEvents)
        {
            Environment.Initialize();
            double epsilon = 1;

            EpsilonDecayEnd = NumberOfTrainingEpisodes / 2;
            EpsilonDecayValue = GetEpsilonDecayValue(epsilon);

            TrainingEpisodes = new List<TrainingSession>();

            if (!overrideBaseEvents)
                OnTrainingStateChanged(true);

            // This is to experiment with SARSA
            //Environment.LearningType = LearningTypes.SARSA;
            RunTrainingSet(epsilon, overrideBaseEvents);

            if (!overrideBaseEvents)
                OnTrainingStateChanged(false);
        }

        protected virtual void RunTrainingSet(double epsilon, bool overrideBaseEvents)
        {
            for (int episode = 0; episode < NumberOfTrainingEpisodes; ++episode)
            {
                var episodeResults = RunTrainingEpisode(epsilon, overrideBaseEvents);
                var state = episodeResults.finalState;
                var moves = episodeResults.moves;

                if (Environment.IsTerminalState(state, moves, MaximumAllowedMoves) &&
                    (1 + episode) % Environment.QualitySaveFrequency == 0)
                {
                    var trainingEpisode = Environment.SaveQualityForEpisode(episode + 1, moves, Score);
                    TrainingEpisodes.Add(trainingEpisode);
                }

                epsilon = DecayEpsilon(episode, epsilon);

                if (!overrideBaseEvents)
                    OnTrainingEpisodeCompleted(episode, NumberOfTrainingEpisodes, moves, Score, Environment.ObjectiveStates.Contains(state % Environment.StatesPerPhase));
            }
        }

        protected virtual (int finalState, int moves) RunTrainingEpisode(double epsilon, bool overrideBaseEvents)
        {
            Moves = 0;
            int previousState = -1;
            bool done = false;
            
            // Start at a random state
            State = _random.Next(0, Environment.NumberOfStates);

            Score = 0;

            while (!done)
            {
                // Determine the next action to take
                var nextActionSet = Environment.GetNextAction(State, epsilon);
                int nextAction = nextActionSet.nextAction;
                var oldQuality = Environment.QualityTable[State][nextAction];

                // Update the quality table
                if (Environment.LearningType == LearningTypes.QLearning)
                {
                    Environment.CalculateQLearning(State, nextAction, LearningRate, DiscountRate);
                }
                else
                {
                    Environment.CalculateSarsa(State, nextAction, LearningRate, DiscountRate, epsilon);
                }

                // Step to the next state using the assigned action
                var step = Environment.Step(State, nextAction);

                previousState = State;
                State = step.newState;
                Moves++;
                Score += step.reward;

                if (!overrideBaseEvents)
                    OnTrainingAgentStateChanged(nextAction, State, Moves, Score, step.quality, oldQuality);
                
                if (Environment.IsTerminalState(State, nextAction, Moves, MaximumAllowedMoves))
                {
                    done = true;
                }
                else if (State == previousState)
                {
                    // Check for repeated actions, and adjust if happening
                    State = _random.Next(0, Environment.NumberOfStates);
                }
            }

            return (State, Moves);
        }

        protected virtual double GetEpsilonDecayValue(double epsilon)
        {
            if (EpsilonDecayValue < 0)
            {
                return epsilon / (EpsilonDecayEnd - EpsilonDecayStart);
            }
            else
            {
                return EpsilonDecayValue;
            }
        }


        /// <summary>
        /// Decays the epsilon value so that as training progesses, known values will be more likely to be used
        /// </summary>
        protected virtual double DecayEpsilon(int episode, double epsilon)
        {
            if (EpsilonDecayEnd >= episode &&
                   episode >= EpsilonDecayStart)
            {
                epsilon -= EpsilonDecayValue;
            }

            return epsilon;
        }
    }
}
