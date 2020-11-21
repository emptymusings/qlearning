namespace QLearningMaze.Ui.Forms
{
    partial class TrainingSessionSelector
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
            this.sesssionList = new System.Windows.Forms.ListView();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.useSessionButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // sesssionList
            // 
            this.sesssionList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.sesssionList.FullRowSelect = true;
            this.sesssionList.GridLines = true;
            this.sesssionList.HideSelection = false;
            this.sesssionList.Location = new System.Drawing.Point(11, 13);
            this.sesssionList.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.sesssionList.MultiSelect = false;
            this.sesssionList.Name = "sesssionList";
            this.sesssionList.Size = new System.Drawing.Size(693, 480);
            this.sesssionList.TabIndex = 0;
            this.sesssionList.UseCompatibleStateImageBehavior = false;
            this.sesssionList.View = System.Windows.Forms.View.Details;
            this.sesssionList.SelectedIndexChanged += new System.EventHandler(this.sesssionList_SelectedIndexChanged);
            this.sesssionList.DoubleClick += new System.EventHandler(this.sesssionList_DoubleClick);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Name = "columnHeader5";
            this.columnHeader5.Text = "Episode";
            this.columnHeader5.Width = 120;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Name = "columnHeader1";
            this.columnHeader1.Text = "Max Episode";
            this.columnHeader1.Width = 0;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Name = "columnHeader2";
            this.columnHeader2.Text = "Moves";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Name = "columnHeader3";
            this.columnHeader3.Text = "Score";
            this.columnHeader3.Width = 100;
            // 
            // useSessionButton
            // 
            this.useSessionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.useSessionButton.Enabled = false;
            this.useSessionButton.Location = new System.Drawing.Point(384, 525);
            this.useSessionButton.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.useSessionButton.Name = "useSessionButton";
            this.useSessionButton.Size = new System.Drawing.Size(150, 47);
            this.useSessionButton.TabIndex = 1;
            this.useSessionButton.Text = "Use";
            this.useSessionButton.UseVisualStyleBackColor = true;
            this.useSessionButton.Click += new System.EventHandler(this.useSessionButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Enabled = false;
            this.cancelButton.Location = new System.Drawing.Point(555, 525);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(150, 47);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Name = "columnHeader4";
            this.columnHeader4.Text = "Succeeded";
            this.columnHeader4.Width = 140;
            // 
            // TrainingSessionSelector
            // 
            this.AcceptButton = this.useSessionButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(732, 582);
            this.ControlBox = false;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.useSessionButton);
            this.Controls.Add(this.sesssionList);
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TrainingSessionSelector";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Training Sessions";
            this.Load += new System.EventHandler(this.TrainingSessionSelector_Load);
            this.Shown += new System.EventHandler(this.TrainingSessionSelector_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ListView sesssionList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button useSessionButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader4;
    }
}