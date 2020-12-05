namespace QLearningMaze.Core.Agent
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Mazes;
    using QLearning.Core.Agent;

    public class MazeAgent : TDAgent<MazeBase>, ITDAgent<MazeBase>
    {
        public MazeAgent() { }

        public MazeAgent(
            double learningRate,
            double discountRate,
            int numberOfTrainingEpisodes,
            int maximumAllowedMoves = 1000,
            int maximumAllowedBacktracks = -1)
            : base (
                  new MazeBase(1, 1, 0, 0, 200),
                  learningRate,
                  discountRate,
                  numberOfTrainingEpisodes,
                  maximumAllowedMoves,
                  maximumAllowedBacktracks)
        {

        }

        private int _startPosition;

        public int StartPosition
        {
            get { return _startPosition; }
            set 
            { 
                _startPosition = value;

                if (Environment != null)
                {
                    Environment.SetInitialState(value);
                }
            }
        }

    }
}
