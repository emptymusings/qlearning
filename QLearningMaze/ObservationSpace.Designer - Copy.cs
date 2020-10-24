namespace QLearningMaze.Ui.Forms
{
    partial class ObservationSpace
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObservationSpace));
            this.positionLabel = new System.Windows.Forms.Label();
            this.startLabel = new System.Windows.Forms.Label();
            this.goalLabel = new System.Windows.Forms.Label();
            this.activeImage = new System.Windows.Forms.PictureBox();
            this.leftWall = new System.Windows.Forms.PictureBox();
            this.rightWall = new System.Windows.Forms.PictureBox();
            this.topWall = new System.Windows.Forms.PictureBox();
            this.bottomWall = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.activeImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftWall)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightWall)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.topWall)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bottomWall)).BeginInit();
            this.SuspendLayout();
            // 
            // positionLabel
            // 
            this.positionLabel.AutoSize = true;
            this.positionLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.positionLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.positionLabel.Location = new System.Drawing.Point(11, 178);
            this.positionLabel.Name = "positionLabel";
            this.positionLabel.Size = new System.Drawing.Size(28, 32);
            this.positionLabel.TabIndex = 0;
            this.positionLabel.Text = "0";
            // 
            // startLabel
            // 
            this.startLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.startLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.startLabel.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.startLabel.Location = new System.Drawing.Point(11, 136);
            this.startLabel.Name = "startLabel";
            this.startLabel.Size = new System.Drawing.Size(251, 42);
            this.startLabel.TabIndex = 1;
            this.startLabel.Text = "Start";
            this.startLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.startLabel.Visible = false;
            // 
            // goalLabel
            // 
            this.goalLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.goalLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.goalLabel.ForeColor = System.Drawing.Color.LimeGreen;
            this.goalLabel.Location = new System.Drawing.Point(11, 94);
            this.goalLabel.Name = "goalLabel";
            this.goalLabel.Size = new System.Drawing.Size(251, 42);
            this.goalLabel.TabIndex = 1;
            this.goalLabel.Text = "Goal";
            this.goalLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.goalLabel.Visible = false;
            // 
            // activeImage
            // 
            this.activeImage.Image = ((System.Drawing.Image)(resources.GetObject("activeImage.Image")));
            this.activeImage.Location = new System.Drawing.Point(99, 85);
            this.activeImage.Name = "activeImage";
            this.activeImage.Size = new System.Drawing.Size(74, 55);
            this.activeImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.activeImage.TabIndex = 2;
            this.activeImage.TabStop = false;
            this.activeImage.Visible = false;
            // 
            // leftWall
            // 
            this.leftWall.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftWall.Image = ((System.Drawing.Image)(resources.GetObject("leftWall.Image")));
            this.leftWall.Location = new System.Drawing.Point(0, 15);
            this.leftWall.Name = "leftWall";
            this.leftWall.Size = new System.Drawing.Size(11, 195);
            this.leftWall.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.leftWall.TabIndex = 3;
            this.leftWall.TabStop = false;
            this.leftWall.Visible = false;
            // 
            // rightWall
            // 
            this.rightWall.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightWall.Image = ((System.Drawing.Image)(resources.GetObject("rightWall.Image")));
            this.rightWall.Location = new System.Drawing.Point(262, 15);
            this.rightWall.Name = "rightWall";
            this.rightWall.Size = new System.Drawing.Size(11, 195);
            this.rightWall.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.rightWall.TabIndex = 3;
            this.rightWall.TabStop = false;
            this.rightWall.Visible = false;
            // 
            // topWall
            // 
            this.topWall.Dock = System.Windows.Forms.DockStyle.Top;
            this.topWall.Image = ((System.Drawing.Image)(resources.GetObject("topWall.Image")));
            this.topWall.Location = new System.Drawing.Point(0, 0);
            this.topWall.Name = "topWall";
            this.topWall.Size = new System.Drawing.Size(273, 15);
            this.topWall.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.topWall.TabIndex = 4;
            this.topWall.TabStop = false;
            this.topWall.Visible = false;
            // 
            // bottomWall
            // 
            this.bottomWall.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomWall.Image = ((System.Drawing.Image)(resources.GetObject("bottomWall.Image")));
            this.bottomWall.Location = new System.Drawing.Point(0, 210);
            this.bottomWall.Name = "bottomWall";
            this.bottomWall.Size = new System.Drawing.Size(273, 15);
            this.bottomWall.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.bottomWall.TabIndex = 4;
            this.bottomWall.TabStop = false;
            this.bottomWall.Visible = false;
            // 
            // ObservationSpace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Ivory;
            this.Controls.Add(this.activeImage);
            this.Controls.Add(this.goalLabel);
            this.Controls.Add(this.startLabel);
            this.Controls.Add(this.positionLabel);
            this.Controls.Add(this.leftWall);
            this.Controls.Add(this.rightWall);
            this.Controls.Add(this.topWall);
            this.Controls.Add(this.bottomWall);
            this.Name = "ObservationSpace";
            this.Size = new System.Drawing.Size(273, 225);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ObservationSpace_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.activeImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftWall)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightWall)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.topWall)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bottomWall)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label positionLabel;
        private System.Windows.Forms.Label startLabel;
        private System.Windows.Forms.Label goalLabel;
        private System.Windows.Forms.PictureBox activeImage;
        public System.Windows.Forms.PictureBox leftWall;
        public System.Windows.Forms.PictureBox rightWall;
        public System.Windows.Forms.PictureBox topWall;
        public System.Windows.Forms.PictureBox bottomWall;
    }
}
