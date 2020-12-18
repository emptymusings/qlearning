namespace QLearning.Core.Agent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Environment;
    using Newtonsoft.Json;

    public partial class TDAgent<TEnvironment> : AgentBase<TEnvironment>, ITDAgent<TEnvironment>
        where TEnvironment : ITDEnvironment
    {
        protected int _trainingEpisodeStartPoint;

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

        [JsonIgnore]
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

                action = GetPreferredNextAction(fromState);

                var stepValues = Environment.Step(fromState, action);

                State = stepValues.newState;

                if (Environment.Step(fromState, action).newState < 0)
                {
                    OnAgentCompleted(Moves, Score, (Environment.TerminalStates.Contains(fromState)));
                    throw new InvalidOperationException($"I guess I didn't learn very well.  Please try training again (perhaps adjusing the learning rate, discount rate, and/or episode count)");
                }

                Score += stepValues.reward;
                Moves++;

                if (Moves > MaximumAllowedMoves)
                {
                    OnAgentCompleted(Moves, Score, (Environment.TerminalStates.Contains(fromState)));
                    throw new InvalidOperationException($"Something's gone wrong, I've wandered around far too many times");
                }

                if (State == previousState)
                {
                    if (MaximumAllowedBacktracks >= 0 &&
                        numberOfBacktracks > MaximumAllowedBacktracks)
                    {
                        OnAgentCompleted(Moves, Score, (Environment.TerminalStates.Contains(fromState)));
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
                OnAgentCompleted(Moves, Score, (Environment.TerminalStates.Contains(fromState)));
        }

        public override void Train()
        {
            Train(false);
        }

        public override void Train(bool overrideBaseEvents)
        {
            Environment.Initialize();

            if (UseDecayingEpsilon)
            {
                EpsilonDecayEnd = NumberOfTrainingEpisodes / 2;
                EpsilonDecayValue = GetEpsilonDecayValue();
            }

            TrainingSessions = new List<TrainingSession>();

            if (!overrideBaseEvents)
                OnTrainingStateChanged(true);

            RunTrainingSet(overrideBaseEvents);

            if (!overrideBaseEvents)
                OnTrainingStateChanged(false);
        }

        protected virtual void RunTrainingSet(bool overrideBaseEvents)
        {
            for (int episode = 0; episode < NumberOfTrainingEpisodes; ++episode)
            {
                var episodeResults = RunTrainingEpisode(overrideBaseEvents);
                var state = episodeResults.finalState;
                var moves = episodeResults.moves;

                if (Environment.IsTerminalState(state, moves, MaximumAllowedMoves) &&
                    (1 + episode) % Environment.QualitySaveFrequency == 0)
                {
                    var trainingEpisode = Environment.SaveQualityForEpisode(episode + 1, moves, Score);
                    TrainingSessions.Add(trainingEpisode);
                }

                if (UseDecayingEpsilon)
                {
                    DecayEpsilon(episode);
                }

                if (!overrideBaseEvents)
                    OnTrainingEpisodeCompleted(episode, NumberOfTrainingEpisodes, _trainingEpisodeStartPoint, moves, Score, Environment.TerminalStates.Contains(state % Environment.StatesPerPhase));
            }
        }

        protected virtual (int finalState, int moves) RunTrainingEpisode(bool overrideBaseEvents)
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
                var nextActionSet = GetNextAction(State);
                int nextAction = nextActionSet.nextAction;
                var oldQuality = Environment.QualityTable[State][nextAction];

                // Update the quality table
                if (LearningStyle == LearningStyles.QLearning)
                {
                    CalculateQLearning(State, nextAction, LearningRate, DiscountRate);
                }
                else
                {
                    CalculateSarsa(State, nextAction, LearningRate, DiscountRate);
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

        protected virtual double GetEpsilonDecayValue()
        {
            if (EpsilonDecayValue < 0)
            {
                return Epsilon / (EpsilonDecayEnd - EpsilonDecayStart);
            }
            else
            {
                return EpsilonDecayValue;
            }
        }


        /// <summary>
        /// Decays the epsilon value so that as training progesses, known values will be more likely to be used
        /// </summary>
        protected virtual void DecayEpsilon(int episode)
        {
            if (EpsilonDecayEnd >= episode &&
                   episode >= EpsilonDecayStart)
            {
                Epsilon -= EpsilonDecayValue;
            }
        }

        /// <summary>
        /// Selects the agent's next action based on the highest Q-Table's value for its current state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override int GetPreferredNextAction(int state, int[] excludedActions = null)
        {
            int preferredNext = -1;
            double max = double.MinValue;

            for (int i = 0; i < Environment.QualityTable[state].Length; ++i)
            {
                if (excludedActions != null &&
                    excludedActions.Contains(i))
                {
                    continue;
                }

                if (Environment.QualityTable[state][i] > max &&
                    Environment.QualityTable[state][i] != 0)
                {
                    max = Environment.QualityTable[state][i];
                    preferredNext = i;
                }
            }

            return preferredNext;
        }

        /// <summary>
        /// Get the next action to take from the current state
        /// </summary>
        /// <param name="state">The state in which the agent currently resides</param>
        /// <param name="epsilon"></param>
        protected override (int nextAction, bool usedGreedy) GetNextAction(int state)
        {
            double randRand = _random.NextDouble();
            int nextAction = -1;
            bool usedGreedy = false;

            if (randRand > Epsilon)
            {
                int preferredNext = GetPreferredNextAction(state);

                if (preferredNext >= 0)
                {
                    nextAction = preferredNext;
                    usedGreedy = true;
                }
            }

            while (nextAction < 0)
                nextAction = GetRandomNextAction(state);

            return (nextAction, usedGreedy);
        }


        /// <summary>
        /// Selects the agent's next action randomly based on its current state
        /// </summary>
        protected override int GetRandomNextAction(int state)
        {
            List<int> possibleNextStates = GetPossibleNextActions(state);

            int count = possibleNextStates.Count;
            int index = _random.Next(0, count);

            if (possibleNextStates.Count > 0)
                return possibleNextStates[index];
            else
                throw new NullReferenceException($"There are no possible actions that can be taken from the state {state}");
        }

        /// <summary>
        /// Gets all possible actions available to the agent in its current state
        /// </summary>
        protected override List<int> GetPossibleNextActions(int state)
        {
            List<int> result = new List<int>();
            int actionCount = Environment.NumberOfActions;

            for (int i = 0; i < actionCount; ++i)
            {
                if (Environment.StatesTable[state][i] >= 0)
                {
                    result.Add(i);
                }
            }

            return result;
        }

        public virtual void CalculateQLearning(int state, int action, double learningRate, double discountRate)
        {
            var step = Environment.Step(state, action);
            var forecaster = GetFuturePositionMaxQ(step.newState);
            var maxQ = forecaster.maxQ;
            Environment.QualityTable[state][action] += (learningRate * (step.reward + (discountRate * maxQ) - step.quality));

        }

        public virtual void CalculateSarsa(int state, int action, double learningRate, double discountRate)
        {
            var step = Environment.Step(state, action);
            var nextActionSet = GetNextAction(step.newState);
            var newQ = Environment.QualityTable[step.newState][nextActionSet.nextAction];
            Environment.QualityTable[state][action] += (learningRate * (step.reward + (discountRate * newQ) - step.quality));
        }

        protected virtual (int selectedNextState, double maxQ) GetFuturePositionMaxQ(int nextState)
        {
            double maxQ = double.MinValue;


            List<int> possNextNextActions = GetPossibleNextActions(nextState);
            int selectedNextState = -1;

            for (int i = 0; i < possNextNextActions.Count; ++i)
            {
                int futureNextAction = possNextNextActions[i];  // short alias

                double futureQuality = Environment.QualityTable[nextState][futureNextAction];

                if (!Environment.IsValidState(nextState))
                {
                    throw new InvalidOperationException($"Attempting action {futureNextAction} from state {nextState} returned an invalid value");
                }


                if (futureQuality > maxQ)
                {
                    maxQ = futureQuality;
                    selectedNextState = nextState;
                }
            }


            return (selectedNextState, maxQ);
        }


    }
}
