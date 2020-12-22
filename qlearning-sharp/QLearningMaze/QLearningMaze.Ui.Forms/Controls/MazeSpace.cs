namespace QLearningMaze.Ui.Forms.Controls
{
    using Core.Mazes;

    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Windows.Forms;

    public partial class MazeSpace : UserControl
    {
        public MazeSpace()
        {
            InitializeComponent();
        }

        public List<ObservationSpaceRow> Rows { get; set; } = new List<ObservationSpaceRow>();
        public static ObservationSpace ActiveSpacePrimary { get; set; }
        public static ObservationSpace ActiveSpaceSecondary { get; set; }
        public void CreateMazeControls(IMaze maze)
        {
            this.SuspendLayout();
            Rows.Clear();
            var rowControls = CreateRowControls(maze);
            List<ObservationSpaceRow> rows = new List<ObservationSpaceRow>();
            foreach(var row in rowControls)
            {
                rows.Add(row);
                Rows.Add(row);
                row.Dock = DockStyle.Top;
            }

            this.Controls.Clear();
            
            this.Controls.AddRange(rows.ToArray());

            for (int i = rows.Count - 1; i >= 0; i--)
            {
                rows[i].SendToBack();
            }

            this.ResumeLayout();
        }

        private List<ObservationSpaceRow> CreateRowControls(IMaze maze)
        {
            int position = 0;
            int rowNumber = 0;
            List<ObservationSpaceRow> rows = new List<ObservationSpaceRow>();

            for (int i = 0; i < maze.Rows; i++)
            {
                var row = new ObservationSpaceRow();
                row.SetRowNumber(rowNumber);

                for (int j = 0; j < maze.Columns; j++)
                {
                    var space = new ObservationSpace();
                    space.SetPosition(position);

                    if (position == maze.GetInitialState())
                    {
                        space.SetStart(true);
                        space.SetActive();
                        space.Invalidate();
                    }

                    if (position == maze.GoalPosition)
                        space.SetGoal(true);

                    var customReward = maze.GetAdditionalRewards().Where(x => x.State == position).FirstOrDefault();

                    if (customReward != null)
                    {
                        space.SetReward(true, customReward.Value);
                    }
                    
                    DrawWalls(maze, position, space);

                    row.AddSpace(space);
                    space.BorderStyle = BorderStyle.FixedSingle;
                    space.Dock = DockStyle.Left;
                    space.BringToFront();

                    position++;
                }

                rows.Add(row);
                rowNumber++;
            }

            return rows;
        }

        private void DrawWalls(IMaze maze, int position, ObservationSpace space)
        {
            var walls = maze.Obstructions.Where(x => x.BetweenSpace == position || x.AndSpace == position);

            foreach (var wall in walls)
            {
                if (position == wall.BetweenSpace)
                {
                    DrawWalls(position, wall.AndSpace, space);
                }
                else
                {
                    DrawWalls(position, wall.BetweenSpace, space);
                }
            }            
        }

        private void DrawWalls(int currentSpace, int otherSpace, ObservationSpace space)
        {
            if (currentSpace == otherSpace - 1)
            {
                space.rightWall.Visible = true;
            }
            else if (currentSpace == otherSpace + 1)
            {
                space.leftWall.Visible = true;
            }
            else if (currentSpace > otherSpace)
            {
                space.topWall.Visible = true;
            }
            else if (currentSpace < otherSpace)
            {
                space.bottomWall.Visible = true;
            }
        }

        public ObservationSpace GetSpaceByPosition(int position)
        {
            var row = Rows.Where(row => row.Spaces.Any(s => s.Position == position)).FirstOrDefault();
            var newSpace = row.Spaces.Where(s => s.Position == position).FirstOrDefault();

            return newSpace;
        }
    }
}
