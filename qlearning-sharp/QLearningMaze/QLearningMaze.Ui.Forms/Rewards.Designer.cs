namespace QLearningMaze.Ui.Forms
{
    partial class Rewards
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
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.addButton = new System.Windows.Forms.Button();
            this.valueText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.positionText = new System.Windows.Forms.TextBox();
            this.removeButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.rewardsList = new System.Windows.Forms.ListView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.okButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Position";
            this.columnHeader1.Width = 130;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 90;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(352, 45);
            this.addButton.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(102, 47);
            this.addButton.TabIndex = 2;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // valueText
            // 
            this.valueText.Location = new System.Drawing.Point(280, 47);
            this.valueText.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.valueText.Name = "valueText";
            this.valueText.Size = new System.Drawing.Size(58, 39);
            this.valueText.TabIndex = 1;
            this.valueText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(200, 50);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Value";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 51);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 32);
            this.label2.TabIndex = 0;
            this.label2.Text = "Position";
            // 
            // positionText
            // 
            this.positionText.Location = new System.Drawing.Point(134, 47);
            this.positionText.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.positionText.Name = "positionText";
            this.positionText.Size = new System.Drawing.Size(58, 39);
            this.positionText.TabIndex = 0;
            this.positionText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(23, 299);
            this.removeButton.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(150, 47);
            this.removeButton.TabIndex = 11;
            this.removeButton.TabStop = false;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(23, 359);
            this.clearButton.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(150, 47);
            this.clearButton.TabIndex = 11;
            this.clearButton.TabStop = false;
            this.clearButton.Text = "Clear All";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // rewardsList
            // 
            this.rewardsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.rewardsList.FullRowSelect = true;
            this.rewardsList.GridLines = true;
            this.rewardsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.rewardsList.HideSelection = false;
            this.rewardsList.Location = new System.Drawing.Point(23, 11);
            this.rewardsList.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.rewardsList.MultiSelect = false;
            this.rewardsList.Name = "rewardsList";
            this.rewardsList.Size = new System.Drawing.Size(262, 260);
            this.rewardsList.TabIndex = 3;
            this.rewardsList.TabStop = false;
            this.rewardsList.UseCompatibleStateImageBehavior = false;
            this.rewardsList.View = System.Windows.Forms.View.Details;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.addButton);
            this.groupBox1.Controls.Add(this.valueText);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.positionText);
            this.groupBox1.Location = new System.Drawing.Point(312, 22);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.groupBox1.Size = new System.Drawing.Size(473, 124);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Add Reward";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(649, 363);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(150, 46);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "Done";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // Rewards
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(811, 421);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.rewardsList);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.removeButton);
            this.Name = "Rewards";
            this.Text = "Rewards";
            this.Shown += new System.EventHandler(this.Rewards_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.TextBox valueText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox positionText;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.ListView rewardsList;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button okButton;
    }
}