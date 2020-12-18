namespace QLearningMaze.Ui.Forms.Controls
{
    partial class ObservationSpaceRow
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
            this.rowLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // rowLabel
            // 
            this.rowLabel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.rowLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rowLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.rowLabel.Location = new System.Drawing.Point(0, 0);
            this.rowLabel.Name = "rowLabel";
            this.rowLabel.Size = new System.Drawing.Size(192, 209);
            this.rowLabel.TabIndex = 0;
            this.rowLabel.Text = "Row: ";
            this.rowLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ObservationSpaceRow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rowLabel);
            this.Name = "ObservationSpaceRow";
            this.Size = new System.Drawing.Size(1415, 209);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label rowLabel;
    }
}
