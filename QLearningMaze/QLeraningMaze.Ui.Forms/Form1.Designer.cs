namespace QLearningMaze.Ui.Forms
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.entryPanel = new System.Windows.Forms.Panel();
            this.rewardsButton = new System.Windows.Forms.Button();
            this.clearObstructionsButton = new System.Windows.Forms.Button();
            this.removeObstructionButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.addObstructionButton = new System.Windows.Forms.Button();
            this.andText = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.betweenText = new System.Windows.Forms.TextBox();
            this.obstructionsList = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.trainingEpisodesText = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.discountRateText = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.learningRateText = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.startPositionText = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.goalPositionText = new System.Windows.Forms.TextBox();
            this.columnsText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.rowsText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rewardsLabel = new System.Windows.Forms.Label();
            this.mazeSpace = new QLearningMaze.Ui.Forms.MazeSpace();
            this.panel3 = new System.Windows.Forms.Panel();
            this.trainMazeButton = new System.Windows.Forms.Button();
            this.runMazeButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.qualityMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rewardsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.entryPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // entryPanel
            // 
            this.entryPanel.Controls.Add(this.rewardsButton);
            this.entryPanel.Controls.Add(this.clearObstructionsButton);
            this.entryPanel.Controls.Add(this.removeObstructionButton);
            this.entryPanel.Controls.Add(this.groupBox1);
            this.entryPanel.Controls.Add(this.obstructionsList);
            this.entryPanel.Controls.Add(this.label8);
            this.entryPanel.Controls.Add(this.label7);
            this.entryPanel.Controls.Add(this.trainingEpisodesText);
            this.entryPanel.Controls.Add(this.label6);
            this.entryPanel.Controls.Add(this.discountRateText);
            this.entryPanel.Controls.Add(this.label5);
            this.entryPanel.Controls.Add(this.learningRateText);
            this.entryPanel.Controls.Add(this.label4);
            this.entryPanel.Controls.Add(this.startPositionText);
            this.entryPanel.Controls.Add(this.label3);
            this.entryPanel.Controls.Add(this.goalPositionText);
            this.entryPanel.Controls.Add(this.columnsText);
            this.entryPanel.Controls.Add(this.label2);
            this.entryPanel.Controls.Add(this.rowsText);
            this.entryPanel.Controls.Add(this.label1);
            this.entryPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.entryPanel.Location = new System.Drawing.Point(0, 40);
            this.entryPanel.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.entryPanel.Name = "entryPanel";
            this.entryPanel.Size = new System.Drawing.Size(1775, 277);
            this.entryPanel.TabIndex = 0;
            // 
            // rewardsButton
            // 
            this.rewardsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.rewardsButton.Location = new System.Drawing.Point(1601, 211);
            this.rewardsButton.Name = "rewardsButton";
            this.rewardsButton.Size = new System.Drawing.Size(150, 46);
            this.rewardsButton.TabIndex = 12;
            this.rewardsButton.Text = "Rewards";
            this.rewardsButton.UseVisualStyleBackColor = true;
            this.rewardsButton.Click += new System.EventHandler(this.rewardsButton_Click);
            // 
            // clearObstructionsButton
            // 
            this.clearObstructionsButton.Location = new System.Drawing.Point(1187, 211);
            this.clearObstructionsButton.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.clearObstructionsButton.Name = "clearObstructionsButton";
            this.clearObstructionsButton.Size = new System.Drawing.Size(150, 47);
            this.clearObstructionsButton.TabIndex = 11;
            this.clearObstructionsButton.TabStop = false;
            this.clearObstructionsButton.Text = "Clear All";
            this.clearObstructionsButton.UseVisualStyleBackColor = true;
            this.clearObstructionsButton.Visible = false;
            this.clearObstructionsButton.Click += new System.EventHandler(this.clearObstructionsButton_Click);
            // 
            // removeObstructionButton
            // 
            this.removeObstructionButton.Location = new System.Drawing.Point(1187, 160);
            this.removeObstructionButton.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.removeObstructionButton.Name = "removeObstructionButton";
            this.removeObstructionButton.Size = new System.Drawing.Size(150, 47);
            this.removeObstructionButton.TabIndex = 11;
            this.removeObstructionButton.TabStop = false;
            this.removeObstructionButton.Text = "Remove";
            this.removeObstructionButton.UseVisualStyleBackColor = true;
            this.removeObstructionButton.Visible = false;
            this.removeObstructionButton.Click += new System.EventHandler(this.removeObstructionButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.addObstructionButton);
            this.groupBox1.Controls.Add(this.andText);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.betweenText);
            this.groupBox1.Location = new System.Drawing.Point(1187, 13);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.groupBox1.Size = new System.Drawing.Size(470, 124);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Add Obstruction";
            this.groupBox1.Visible = false;
            // 
            // addObstructionButton
            // 
            this.addObstructionButton.Location = new System.Drawing.Point(336, 45);
            this.addObstructionButton.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.addObstructionButton.Name = "addObstructionButton";
            this.addObstructionButton.Size = new System.Drawing.Size(102, 47);
            this.addObstructionButton.TabIndex = 10;
            this.addObstructionButton.Text = "Add";
            this.addObstructionButton.UseVisualStyleBackColor = true;
            this.addObstructionButton.Click += new System.EventHandler(this.addObstructionButton_Click);
            // 
            // andText
            // 
            this.andText.Location = new System.Drawing.Point(264, 47);
            this.andText.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.andText.Name = "andText";
            this.andText.Size = new System.Drawing.Size(58, 39);
            this.andText.TabIndex = 9;
            this.andText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(202, 51);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(54, 32);
            this.label10.TabIndex = 0;
            this.label10.Text = "and";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(20, 51);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(106, 32);
            this.label9.TabIndex = 0;
            this.label9.Text = "Between";
            // 
            // betweenText
            // 
            this.betweenText.Location = new System.Drawing.Point(134, 47);
            this.betweenText.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.betweenText.Name = "betweenText";
            this.betweenText.Size = new System.Drawing.Size(58, 39);
            this.betweenText.TabIndex = 8;
            this.betweenText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // obstructionsList
            // 
            this.obstructionsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.obstructionsList.FullRowSelect = true;
            this.obstructionsList.GridLines = true;
            this.obstructionsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.obstructionsList.HideSelection = false;
            this.obstructionsList.Location = new System.Drawing.Point(864, 13);
            this.obstructionsList.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.obstructionsList.MultiSelect = false;
            this.obstructionsList.Name = "obstructionsList";
            this.obstructionsList.Size = new System.Drawing.Size(262, 260);
            this.obstructionsList.TabIndex = 3;
            this.obstructionsList.TabStop = false;
            this.obstructionsList.UseCompatibleStateImageBehavior = false;
            this.obstructionsList.View = System.Windows.Forms.View.Details;
            this.obstructionsList.Visible = false;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Between";
            this.columnHeader1.Width = 130;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "And";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader2.Width = 90;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(709, 13);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(149, 32);
            this.label8.TabIndex = 2;
            this.label8.Text = "Obstructions";
            this.label8.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(297, 196);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(199, 32);
            this.label7.TabIndex = 0;
            this.label7.Text = "Training Episodes";
            // 
            // trainingEpisodesText
            // 
            this.trainingEpisodesText.Location = new System.Drawing.Point(500, 196);
            this.trainingEpisodesText.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.trainingEpisodesText.Name = "trainingEpisodesText";
            this.trainingEpisodesText.Size = new System.Drawing.Size(97, 39);
            this.trainingEpisodesText.TabIndex = 6;
            this.trainingEpisodesText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.trainingEpisodesText.Leave += new System.EventHandler(this.trainingEpisodesText_Leave);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(318, 17);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(162, 32);
            this.label6.TabIndex = 0;
            this.label6.Text = "Discount Rate";
            // 
            // discountRateText
            // 
            this.discountRateText.Location = new System.Drawing.Point(539, 13);
            this.discountRateText.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.discountRateText.Name = "discountRateText";
            this.discountRateText.Size = new System.Drawing.Size(58, 39);
            this.discountRateText.TabIndex = 4;
            this.discountRateText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.discountRateText.Leave += new System.EventHandler(this.discountRateText_Leave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(318, 75);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(160, 32);
            this.label5.TabIndex = 0;
            this.label5.Text = "Learning Rate";
            // 
            // learningRateText
            // 
            this.learningRateText.Location = new System.Drawing.Point(539, 70);
            this.learningRateText.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.learningRateText.Name = "learningRateText";
            this.learningRateText.Size = new System.Drawing.Size(58, 39);
            this.learningRateText.TabIndex = 5;
            this.learningRateText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.learningRateText.Leave += new System.EventHandler(this.learningRateText_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 139);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(153, 32);
            this.label4.TabIndex = 0;
            this.label4.Text = "Start Position";
            // 
            // startPositionText
            // 
            this.startPositionText.Location = new System.Drawing.Point(195, 139);
            this.startPositionText.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.startPositionText.Name = "startPositionText";
            this.startPositionText.Size = new System.Drawing.Size(58, 39);
            this.startPositionText.TabIndex = 2;
            this.startPositionText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.startPositionText.Leave += new System.EventHandler(this.startPositionText_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 196);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(153, 32);
            this.label3.TabIndex = 0;
            this.label3.Text = "Goal Position";
            // 
            // goalPositionText
            // 
            this.goalPositionText.Location = new System.Drawing.Point(195, 196);
            this.goalPositionText.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.goalPositionText.Name = "goalPositionText";
            this.goalPositionText.Size = new System.Drawing.Size(58, 39);
            this.goalPositionText.TabIndex = 3;
            this.goalPositionText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.goalPositionText.Leave += new System.EventHandler(this.goalPositionText_Leave);
            // 
            // columnsText
            // 
            this.columnsText.Location = new System.Drawing.Point(195, 70);
            this.columnsText.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.columnsText.Name = "columnsText";
            this.columnsText.Size = new System.Drawing.Size(58, 39);
            this.columnsText.TabIndex = 1;
            this.columnsText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnsText.Leave += new System.EventHandler(this.columnsText_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 70);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 32);
            this.label2.TabIndex = 0;
            this.label2.Text = "Columns";
            // 
            // rowsText
            // 
            this.rowsText.Location = new System.Drawing.Point(195, 13);
            this.rowsText.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.rowsText.Name = "rowsText";
            this.rowsText.Size = new System.Drawing.Size(58, 39);
            this.rowsText.TabIndex = 0;
            this.rowsText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.rowsText.Leave += new System.EventHandler(this.rowsText_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Rows";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rewardsLabel);
            this.panel2.Controls.Add(this.mazeSpace);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 317);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1775, 622);
            this.panel2.TabIndex = 1;
            // 
            // rewardsLabel
            // 
            this.rewardsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rewardsLabel.Location = new System.Drawing.Point(1073, 11);
            this.rewardsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.rewardsLabel.Name = "rewardsLabel";
            this.rewardsLabel.Size = new System.Drawing.Size(678, 41);
            this.rewardsLabel.TabIndex = 16;
            this.rewardsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // mazeSpace
            // 
            this.mazeSpace.AutoScroll = true;
            this.mazeSpace.BackColor = System.Drawing.SystemColors.ControlDark;
            this.mazeSpace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mazeSpace.Location = new System.Drawing.Point(0, 66);
            this.mazeSpace.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.mazeSpace.Name = "mazeSpace";
            this.mazeSpace.Size = new System.Drawing.Size(1775, 556);
            this.mazeSpace.TabIndex = 0;
            this.mazeSpace.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.trainMazeButton);
            this.panel3.Controls.Add(this.runMazeButton);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1775, 66);
            this.panel3.TabIndex = 12;
            // 
            // trainMazeButton
            // 
            this.trainMazeButton.Location = new System.Drawing.Point(11, 11);
            this.trainMazeButton.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.trainMazeButton.Name = "trainMazeButton";
            this.trainMazeButton.Size = new System.Drawing.Size(150, 47);
            this.trainMazeButton.TabIndex = 15;
            this.trainMazeButton.Text = "Train";
            this.trainMazeButton.UseVisualStyleBackColor = true;
            this.trainMazeButton.Click += new System.EventHandler(this.trainMazeButton_Click);
            // 
            // runMazeButton
            // 
            this.runMazeButton.Location = new System.Drawing.Point(180, 11);
            this.runMazeButton.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.runMazeButton.Name = "runMazeButton";
            this.runMazeButton.Size = new System.Drawing.Size(150, 47);
            this.runMazeButton.TabIndex = 15;
            this.runMazeButton.Text = "Run Maze";
            this.runMazeButton.UseVisualStyleBackColor = true;
            this.runMazeButton.Click += new System.EventHandler(this.runMaze_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.viewStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1775, 40);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveMenuItem,
            this.openMenuItem,
            this.exitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(71, 36);
            this.fileMenuItem.Text = "&File";
            // 
            // saveMenuItem
            // 
            this.saveMenuItem.Name = "saveMenuItem";
            this.saveMenuItem.Size = new System.Drawing.Size(206, 44);
            this.saveMenuItem.Text = "Save";
            // 
            // openMenuItem
            // 
            this.openMenuItem.Name = "openMenuItem";
            this.openMenuItem.Size = new System.Drawing.Size(206, 44);
            this.openMenuItem.Text = "&Open";
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(206, 44);
            this.exitMenuItem.Text = "E&xit";
            // 
            // viewStripMenuItem
            // 
            this.viewStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.qualityMenuItem,
            this.rewardsMenuItem});
            this.viewStripMenuItem.Name = "viewStripMenuItem";
            this.viewStripMenuItem.Size = new System.Drawing.Size(85, 36);
            this.viewStripMenuItem.Text = "&View";
            // 
            // qualityMenuItem
            // 
            this.qualityMenuItem.Name = "qualityMenuItem";
            this.qualityMenuItem.Size = new System.Drawing.Size(296, 44);
            this.qualityMenuItem.Text = "Quality Table";
            // 
            // rewardsMenuItem
            // 
            this.rewardsMenuItem.Name = "rewardsMenuItem";
            this.rewardsMenuItem.Size = new System.Drawing.Size(296, 44);
            this.rewardsMenuItem.Text = "Rewards Table";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1775, 939);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.entryPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.entryPanel.ResumeLayout(false);
            this.entryPanel.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel entryPanel;
        private System.Windows.Forms.Panel panel2;
        private MazeSpace mazeSpace;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button runMazeButton;
        private System.Windows.Forms.TextBox columnsText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox rowsText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button addObstructionButton;
        private System.Windows.Forms.TextBox andText;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox betweenText;
        private System.Windows.Forms.ListView obstructionsList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox trainingEpisodesText;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox discountRateText;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox learningRateText;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox startPositionText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox goalPositionText;
        private System.Windows.Forms.Button removeObstructionButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.Button trainMazeButton;
        private System.Windows.Forms.Button clearObstructionsButton;
        private System.Windows.Forms.ToolStripMenuItem viewStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem qualityMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rewardsMenuItem;
        private System.Windows.Forms.Label rewardsLabel;
        private System.Windows.Forms.Button rewardsButton;
    }
}

