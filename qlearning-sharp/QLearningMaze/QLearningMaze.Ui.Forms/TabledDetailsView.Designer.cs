namespace QLearningMaze.Ui.Forms
{
    partial class TabledDetailsView
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
            this.detailsView1 = new QLearningMaze.Ui.Forms.DetailsView();
            this.SuspendLayout();
            // 
            // detailsView1
            // 
            this.detailsView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsView1.Location = new System.Drawing.Point(0, 0);
            this.detailsView1.Name = "detailsView1";
            this.detailsView1.Size = new System.Drawing.Size(1092, 619);
            this.detailsView1.TabIndex = 0;
            // 
            // TabledDetailsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1092, 619);
            this.Controls.Add(this.detailsView1);
            this.Name = "TabledDetailsView";
            this.Text = "TabledDetailsView";
            this.Shown += new System.EventHandler(this.TabledDetailsView_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private DetailsView detailsView1;
    }
}