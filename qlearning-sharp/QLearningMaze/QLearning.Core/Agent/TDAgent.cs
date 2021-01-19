namespace QLearning.Core.Agent
{
    using Environment;

    using Newtonsoft.Json;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class TDAgent<TEnvironment> : AgentBase<TEnvironment>, ITDAgent<TEnvironment>
        where TEnvironment : ITDEnvironment
    {
        protected int _trainingEpisodeStartPoint;

        public TDAgent() { }

        public TDAgent(
            TEnvironment environment,
            double learningRate,
            double discountFactor,
            int numberOfTrainingEpisodes,
            int maximumAllowedMoves = 1000,
            int maximumAllowedBacktracks = -1)
        {
            Environment = environment;
            NumberOfTrainingEpisodes = numberOfTrainingEpisodes;
            LearningRate = learningRate;
            DiscountFactor = discountFactor;
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

        public override void Run(int fromState, bool overrideBaseEvents = false)
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
                SetState(fromState, stepValues.newState);
                
                Score += stepValues.reward;
                Moves++;

                ValidateMoveCount(fromState);

                ValidateBacktracks(previousState, fromState, ref numberOfBacktracks);

                previousState = fromState;
                fromState = State;

                if (!overrideBaseEvents)
                    OnAgentStateChanged(fromState, Moves, Score);

                done = Environment.IsTerminalState(fromState, action, Moves, MaximumAllowedMoves);
            }

            if (!overrideBaseEvents)
                OnAgentCompleted(Moves, Score, (Environment.TerminalStates.Contains(fromState)));
        }

        #region Validation of State

        /// <summary>
        /// Sets a new state for the agent as it moves
        /// </summary>
        /// <param name="fromState">The state from which the agent is moving</param>
        /// <param name="toState">The state to which the agent is moving</param>
        protected virtual void SetState(int fromState, int toState)
        {
            State = toState;

            if (State < 0)
            {
                OnAgentCompleted(Moves, Score, (Environment.TerminalStates.Contains(fromState)));
                throw new InvalidOperationException($"I guess I didn't learn very well.  Please try training again (perhaps adjusing the learning rate, discount rate, and/or episode count)");
            }
        }

        /// <summary>
        /// Validates that the agent has not exceeded its allowed number of moves
        /// </summary>
        /// <param name="fromState">Indicator of which state the agent was in when it exceeded the allowed moves</param>
        protected virtual void ValidateMoveCount(int fromState)
        {
            if (Moves > MaximumAllowedMoves)
            {
                OnAgentCompleted(Moves, Score, (Environment.TerminalStates.Contains(fromState)));
                throw new InvalidOperationException($"Something's gone wrong, I've wandered around far too many times");
            }
        }

        /// <summary>
        /// Validates that the agent has not backtracked more than the acceptable number of times (helps keep the agent from taking too many redundant steps/moves)
        /// </summary>
        /// <param name="previousState">The state that the agent was in prior to moving into the fromState (if the current state matches this, it is considered a backtrack)</param>
        /// <param name="fromState">The state from which the agent moved into its current state (will become the previousState after evaluation)</param>
        /// <param name="numberOfBacktracks">The number of backtracks to allow the agent to take</param>
        protected virtual void ValidateBacktracks(int previousState, int fromState, ref int numberOfBacktracks)
        {
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
        }

        #endregion

        #region Training

        public override void Train()
        {
            Train(false);
        }

        public override void Train(bool overrideBaseEvents)
        {
            Environment.Initialize();
            double epsilon = DefaultEpsilon;

            if (UseDecayingEpsilon)
            {
                EpsilonDecayEnd = NumberOfTrainingEpisodes / 2;
                EpsilonDecayValue = GetEpsilonDecayValue(epsilon);
            }

            TrainingSessions = new List<TrainingSession>();

            if (!overrideBaseEvents)
                OnTrainingStateChanged(true);

            RunTrainingSet(epsilon, overrideBaseEvents);

            if (!overrideBaseEvents)
                OnTrainingStateChanged(false);
        }

        /// <summary>
        /// Iterates over the training episodes
        /// </summary>
        /// <param name="epsilon">The epsilon value used to determine whether to perform exploration/exploitation</param>
        /// <param name="overrideBaseEvents">Allows the caller to override default events</param>
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

                if (UseDecayingEpsilon)
                {
                    epsilon = DecayEpsilon(episode, epsilon);
                }

                if (!overrideBaseEvents)
                    OnTrainingEpisodeCompleted(episode, NumberOfTrainingEpisodes, _trainingEpisodeStartPoint, moves, Score, Environment.TerminalStates.Contains(state % Environment.StatesPerPhase));
            }
        }

        /// <summary>
        /// Sends the Agent through a single training episode
        /// </summary>
        /// <param name="epsilon">The epsilon value used to determine whether to perform exploration/exploitation</param>
        /// <param name="overrideBaseEvents">Allows the caller to override default events</param>
        /// <returns>A tuple which indicates the final state of the agent before it terminated (whether successful or not), an the total number of moves it took</returns>
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
                var nextActionSet = GetTrainingNextAction(State, epsilon);
                int nextAction = nextActionSet.nextAction;
                var oldQuality = Environment.QualityTable[State][nextAction];

                // Update the quality table
                if (LearningStyle == LearningStyles.QLearning)
                {
                    CalculateQLearning(State, nextAction, LearningRate, DiscountFactor);
                }
                else
                {
                    CalculateSarsa(State, nextAction, LearningRate, DiscountFactor, epsilon);
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

        /// <summary>
        /// Calculates the rate at which the epsilon value will decay
        /// </summary>
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

        /// <summary>
        /// Get the next action to take from the current state during a training session
        /// </summary>
        /// <param name="state">The state in which the agent currently resides</param>
        /// <param name="epsilon">The epsilon value used to determine whether to perform exploration/exploitation</param>
        protected override (int nextAction, bool usedGreedy) GetTrainingNextAction(int state, double epsilon)
        {
            double randRand = _random.NextDouble();
            int nextAction = -1;
            bool usedGreedy = false;

            if (randRand > epsilon)
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
        /// Selects the agent's next action randomly based on its current state (exploration)
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
        /// Gets all possible actions available to the agent in its current state (exploitation)
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

        /// <summary>
        /// Retrieves the maximum Q-value of a future state/action pair based on the specified state
        /// </summary>
        /// <param name="state">The state from which the future maximum Q-value will be determined</param>
        protected virtual double GetFuturePositionMaxQ(int state)
        {
            double maxQ = double.MinValue;


            List<int> possNextNextActions = GetPossibleNextActions(state);
            int selectedNextState = -1;

            for (int i = 0; i < possNextNextActions.Count; ++i)
            {
                int futureNextAction = possNextNextActions[i];  // short alias

                double futureQuality = Environment.QualityTable[state][futureNextAction];

                if (!Environment.IsValidState(state))
                {
                    throw new InvalidOperationException($"Attempting action {futureNextAction} from state {state} returned an invalid value");
                }


                if (futureQuality > maxQ)
                {
                    maxQ = futureQuality;
                    selectedNextState = state;
                }
            }


            return maxQ;
        }

        #endregion

        #region Quality functions

        /// <summary>
        /// Calculates quality based on the Q-Function
        /// </summary>
        /// <param name="state">The state in the Q-matrix to which the quality will be assigned</param>
        /// <param name="action">The action in the Q-matrix to which the quality will be assigned</param>
        /// <param name="learningRate">(AKA alpha) - A value between 0 and 1 that indicates weight given to new information, where 0 indicates no weight given to new information, and 1 indicates that only new information is important</param>
        /// <param name="discountFactor">(AKA gamma) - A value between 0 and 1 that indicates weight between immediate and long-term rewards, where 0 indicates concern only with immediate rewards, and 1 indicates concern only with long term rewards</param>
        public virtual void CalculateQLearning(int state, int action, double learningRate, double discountFactor)
        {
            var step = Environment.Step(state, action);
            var maxQ = GetFuturePositionMaxQ(step.newState);
            Environment.QualityTable[state][action] += (learningRate * (step.reward + (discountFactor * maxQ) - step.quality));
        }

        /// <summary>
        /// Calculates quality based on the SARSA-Function
        /// </summary>
        /// <param name="state">The state in the Q-matrix to which the quality will be assigned</param>
        /// <param name="action">The action in the Q-matrix to which the quality will be assigned</param>
        /// <param name="learningRate">(AKA alpha) - A value between 0 and 1 that indicates weight given to new information, where 0 indicates no weight given to new information, and 1 indicates that only new information is important</param>
        /// <param name="discountFactor">(AKA gamma) - A value between 0 and 1 that indicates weight between immediate and long-term rewards, where 0 indicates concern only with immediate rewards, and 1 indicates concern only with long term rewards</param>
        /// <param name="epsilon">The epsilon value used to determine whether to perform exploration/exploitation.  In this context, it is used to perform the next action in the policy to "backfill" quality</param>
        public virtual void CalculateSarsa(int state, int action, double learningRate, double discountFactor, double epsilon)
        {
            var step = Environment.Step(state, action);
            var nextActionSet = GetTrainingNextAction(step.newState, epsilon);
            var newQ = Environment.QualityTable[step.newState][nextActionSet.nextAction];
            Environment.QualityTable[state][action] += (learningRate * (step.reward + (discountFactor * newQ) - step.quality));
        }

        #endregion

        /// <summary>
        /// Retrieves the next action the agent should take from its current state based on the current policy
        /// </summary>
        /// <param name="state">The current state of the agent, from which its next action will be taken</param>
        /// <param name="excludedActions">Optional array of actions to be considered illegal</param>
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
    }
}
