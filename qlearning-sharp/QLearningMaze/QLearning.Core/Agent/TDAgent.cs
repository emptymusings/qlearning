namespace QLearning.Core.Agent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Environment;

    public partial class TDAgent<TEnvironment> : AgentBase<TEnvironment>, ITDAgent<TEnvironment>
        where TEnvironment : ITDEnvironment
    {
        private Random _random = new Random();
        private int _trainingEpisodeStartPoint;

        public TDAgent() { }

        public TDAgent(
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

        
        public virtual IList<TrainingSession> TrainingSessions { get; set; }
        public virtual TrainingSession BestTrainingSession
        {
            get
            {
                if (TrainingSessions == null ||
                    TrainingSessions.Count == 0)
                {
                    return null;
                }

                return TrainingSessions.OrderByDescending(r => r.Score).FirstOrDefault();
            }
        }
        public int MaximumAllowedBacktracks { get; set; }

        public override void Run(int fromState)
        {
            Run(fromState, false);
        }

        public override void Run(int fromState, bool overrideBaseEvents)
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

        public override void Train()
        {
            Train(false);
        }

        public override void Train(bool overrideBaseEvents)
        {
            Environment.Initialize();
            double epsilon = 1;

            EpsilonDecayEnd = NumberOfTrainingEpisodes / 2;
            EpsilonDecayValue = GetEpsilonDecayValue(epsilon);

            TrainingSessions = new List<TrainingSession>();

            if (!overrideBaseEvents)
                OnTrainingStateChanged(true);

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
                    TrainingSessions.Add(trainingEpisode);
                }

                epsilon = DecayEpsilon(episode, epsilon);

                if (!overrideBaseEvents)
                    OnTrainingEpisodeCompleted(episode, NumberOfTrainingEpisodes, _trainingEpisodeStartPoint, moves, Score, Environment.ObjectiveStates.Contains(state % Environment.StatesPerPhase));
            }
        }

        protected virtual (int finalState, int moves) RunTrainingEpisode(double epsilon, bool overrideBaseEvents)
        {
            Moves = 0;
            int previousState = -1;
            bool done = false;
            
            // Start at a random state
            State = _random.Next(0, Environment.NumberOfStates);
            _trainingEpisodeStartPoint = State;
            Score = 0;

            while (!done)
            {
                // Determine the next action to take
                var nextActionSet = Environment.GetNextAction(State, epsilon);
                int nextAction = nextActionSet.nextAction;
                var oldQuality = Environment.QualityTable[State][nextAction];

                // Update the quality table
                if (LearningStyle == LearningStyles.QLearning)
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
