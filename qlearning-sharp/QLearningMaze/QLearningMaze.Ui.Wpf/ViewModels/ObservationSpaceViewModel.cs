namespace QLearningMaze.Ui.Wpf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;

    public class ObservationSpaceViewModel : ViewModelBase
    {        
        private bool _isActive;
        private object _visibilityLock = new object();

        public bool IsActive
        {
            get { return _isActive; }
            set 
            { 
                SetProperty(ref _isActive, value);
            }
        }

        private string _activeImageSource;

        public string ActiveImageSource
        {
            get { return _activeImageSource; }
            set { SetProperty(ref _activeImageSource, value); }
        }

        private Visibility _activeVisibility = Visibility.Hidden;

        public Visibility ActiveVisibility
        {
            get { return _activeVisibility; }
            set 
            { 
                SetProperty(ref _activeVisibility, value); 
            }
        }

        private bool _isStart;

        public bool IsStart
        {
            get { return _isStart; }
            set 
            {
                if (value != _isStart)
                {
                    SetProperty(ref _isStart, value);
                    OnPropertyChanged(nameof(ExtrasMessage));
                    OnPropertyChanged(nameof(ExtrasVisibility));
                }
            }
        }

        private bool _isGoal;

        public bool IsGoal
        {
            get { return _isGoal; }
            set 
            {
                if (value != _isGoal)
                {
                    SetProperty(ref _isGoal, value);
                    OnPropertyChanged(nameof(ExtrasMessage));
                    OnPropertyChanged(nameof(ExtrasVisibility));
                }
            }
        }

        public string ExtrasMessage
        {
            get
            {
                if (IsStart)
                {
                    return "Start";
                }

                if (IsGoal)
                {
                    return "Goal";
                }
                
                if (Reward > 0)
                {
                    return $"Reward: {Reward}";
                }

                if (Reward < 0)
                {
                    return $"Punish: {Reward}";
                }

                return null;
            }
        }

        public Visibility ExtrasVisibility
        {
            get
            {
                return string.IsNullOrEmpty(ExtrasMessage) ? Visibility.Hidden : Visibility.Visible;
            }
        }

        private int _position;

        public int Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        private double _reward;

        public double Reward
        {
            get { return _reward; }
            set 
            { 
                SetProperty(ref _reward, value);
                OnPropertyChanged(nameof(ExtrasMessage));
                OnPropertyChanged(nameof(ExtrasVisibility));
            }
        }

        private Visibility _leftWallVisibility = Visibility.Hidden;

        public Visibility LeftWallVisibility
        {
            get { return _leftWallVisibility; }
            set { SetProperty(ref _leftWallVisibility, value); }
        }

        private Visibility _rightWallVisibility = Visibility.Hidden;

        public Visibility RightWallVisibility
        {
            get { return _rightWallVisibility; }
            set { SetProperty(ref _rightWallVisibility, value); }
        }

        private Visibility _topWallVisibility = Visibility.Hidden;

        public Visibility TopWallVisibility
        {
            get { return _topWallVisibility; }
            set { SetProperty(ref _topWallVisibility, value); }
        }


        private Visibility _bottomWallVisibility = Visibility.Hidden;

        public Visibility BottomWallVisibility
        {
            get { return _bottomWallVisibility; }
            set { SetProperty(ref _bottomWallVisibility, value); }
        }

        public void SetActive(bool isPrimaryAgent)
        {
            if (isPrimaryAgent)
            {
                ActiveImageSource = "/assets/ActiveDot.bmp";
            }
            else
            {
                ActiveImageSource = "/assets/ActiveDotSecondary.bmp";
            }

            lock (_visibilityLock)
            {
                ActiveVisibility = Visibility.Visible;
                OnPropertyChanged(nameof(ActiveVisibility));
            }
        }

        public void SetInactive()
        {
            lock (_visibilityLock)
            {
                ActiveVisibility = Visibility.Collapsed;
                OnPropertyChanged(nameof(ActiveVisibility));
            }
        }
    }
}
