namespace QLearningMaze.Ui.Forms
{
    partial class TrainingProgress
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.controlsPanel = new System.Windows.Forms.Panel();
            this.closeButton = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.progressLabel = new System.Windows.Forms.Label();
            this.trainingProgressBar = new System.Windows.Forms.ProgressBar();
            this.trainingProgressTextbox = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.controlsPanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1330, 100);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1330, 100);
            this.label1.TabIndex = 0;
            this.label1.Text = "Training Progress";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // controlsPanel
            // 
            this.controlsPanel.Controls.Add(this.closeButton);
            this.controlsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.controlsPanel.Location = new System.Drawing.Point(0, 280);
            this.controlsPanel.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.controlsPanel.Name = "controlsPanel";
            this.controlsPanel.Size = new System.Drawing.Size(1330, 87);
            this.controlsPanel.TabIndex = 1;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(1166, 30);
            this.closeButton.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(150, 47);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Done";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Visible = false;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.progressLabel);
            this.panel3.Controls.Add(this.trainingProgressBar);
            this.panel3.Controls.Add(this.trainingProgressTextbox);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 100);
            this.panel3.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1330, 180);
            this.panel3.TabIndex = 2;
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(280, 41);
            this.progressLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(133, 32);
            this.progressLabel.TabIndex = 2;
            this.progressLabel.Text = "Initializing...";
            // 
            // trainingProgressBar
            // 
            this.trainingProgressBar.Location = new System.Drawing.Point(280, 90);
            this.trainingProgressBar.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.trainingProgressBar.Name = "trainingProgressBar";
            this.trainingProgressBar.Size = new System.Drawing.Size(769, 47);
            this.trainingProgressBar.TabIndex = 1;
            // 
            // trainingProgressTextbox
            // 
            this.trainingProgressTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.trainingProgressTextbox.Location = new System.Drawing.Point(-591, -492);
            this.trainingProgressTextbox.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.trainingProgressTextbox.Multiline = true;
            this.trainingProgressTextbox.Name = "trainingProgressTextbox";
            this.trainingProgressTextbox.ReadOnly = true;
            this.trainingProgressTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.trainingProgressTextbox.Size = new System.Drawing.Size(1921, 638);
            this.trainingProgressTextbox.TabIndex = 0;
            this.trainingProgressTextbox.Visible = false;
            this.trainingProgressTextbox.WordWrap = false;
            // 
            // TrainingProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1330, 367);
            this.ControlBox = false;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.controlsPanel);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TrainingProgress";
            this.ShowInTaskbar = false;
            this.Text = "TrainingProgress";
            this.Shown += new System.EventHandler(this.TrainingProgress_Shown);
            this.panel1.ResumeLayout(false);
            this.controlsPanel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel controlsPanel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox trainingProgressTextbox;
        private System.Windows.Forms.ProgressBar trainingProgressBar;
        private System.Windows.Forms.Label progressLabel;
    }
}