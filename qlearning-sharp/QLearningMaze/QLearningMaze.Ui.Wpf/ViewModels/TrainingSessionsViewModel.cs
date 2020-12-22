namespace QLearningMaze.Ui.Wpf.ViewModels
{
    using Core;
    using Core.Agent;

    using System.Collections.Generic;
    using System.Linq;

    public class TrainingSessionsViewModel : ViewModelBase
    {
        private readonly MazeAgent _agent;

        private int _moves;
        private double _score;

        public TrainingSessionsViewModel() 
        {
            _agent = new MazeAgent();
        }
        
        public TrainingSessionsViewModel(MazeAgent agent)
        {
            _agent = agent;
        }

        private IList<TrainingSessionEx> _trainingSessions;

        public IList<TrainingSessionEx> TrainingSessions
        {
            get { return _trainingSessions; }
            set 
            {
                SetProperty(ref _trainingSessions, value);
            }
        }

        private TrainingSessionEx _selectedSession;

        public TrainingSessionEx SelectedSession
        {
            get { return _selectedSession; }
            set { SetProperty(ref _selectedSession, value); }
        }




        public void InitializeSessions()
        {
            var trainingSessions = new List<TrainingSessionEx>();
            var sessions = _agent.TrainingSessions.OrderBy(e => e.Episode).ToList();
            var agent = MazeUtilities.ConvertLoadedAgent(_agent);
            agent.Environment = MazeUtilities.CopyEnvironment(_agent.Environment);
            agent.AgentCompleted += Agent_AgentCompleted;

            for (int i = sessions.Count - 1; i >= 0; --i)
            {
                var session = new TrainingSessionEx
                {
                    Episode = sessions[i].Episode,
                    Moves = sessions[i].Moves,
                    Quality = sessions[i].Quality,
                    Score = sessions[i].Score
                };

                _moves = 0;
                _score = 0;

                agent.Environment.QualityTable = session.Quality;

                try
                {
                    agent.Run(agent.StartPosition);
                    session.Succeeded = true;
                }
                catch
                {
                    session.Succeeded = false;
                }

                session.Moves = _moves;
                session.Score = _score;

                trainingSessions.Add(session);
            }

            var selection = trainingSessions
                .GroupBy(g => new
                {
                    g.Moves,
                    g.Score,
                    g.Succeeded
                })
                .Select(t => new TrainingSessionEx()
                {
                    MinEpisode = t.Last().Episode,
                    MaxEpisode = t.First().Episode,
                    Episode = t.Last().Episode,
                    Moves = t.Key.Moves,
                    Score = t.Key.Score,
                    Succeeded = t.Key.Succeeded,
                    Quality = t.First().Quality
                });

            trainingSessions = selection
                .OrderByDescending(s => s.Succeeded)
                .ThenByDescending(m => m.Moves)
                .ThenByDescending(e => e.MinEpisode).ToList();

            TrainingSessions = trainingSessions;
            SelectedSession = TrainingSessions.FirstOrDefault();

            agent.AgentCompleted -= Agent_AgentCompleted;
        }

        private void Agent_AgentCompleted(object sender, QLearning.Core.AgentCompletedEventArgs e)
        {
            _moves += e.Moves;
            _score += e.Rewards;           
        }
    }

    public class TrainingSessionEx : TrainingSession
    {
        public int MinEpisode { get; set; }
        public int MaxEpisode { get; set; }
        public bool Succeeded { get; set; }
    }
}
