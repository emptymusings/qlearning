namespace QLearningMaze.Ui.Wpf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;

    public class ObservationSpaceViewModel : ViewModelBase
    {        
        private bool _isActive;

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
                    StartVisibility = _isStart ? Visibility.Hidden : Visibility.Hidden;
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
                    GoalVisibility = _isGoal ? Visibility.Hidden : Visibility.Hidden;
                }
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
            set { SetProperty(ref _reward, value); }
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

        private Visibility _startVisibility;

        public Visibility StartVisibility
        {
            get { return _startVisibility; }
            set { SetProperty(ref _startVisibility, value); }
        }

        private Visibility _goalVisibility;

        public Visibility GoalVisibility
        {
            get { return _goalVisibility; }
            set { SetProperty(ref _goalVisibility, value); }
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

            ActiveVisibility = Visibility.Visible;
        }

        public void SetInactive()
        {
            ActiveVisibility = Visibility.Hidden;
        }
    }
}
