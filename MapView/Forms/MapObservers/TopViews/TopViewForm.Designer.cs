﻿namespace MapView.Forms.MapObservers.TopViews
{
    partial class TopViewForm
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
            this.TopViewControl = new TopView();
            this.SuspendLayout();
            // 
            // TopViewControl
            // 
            this.TopViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TopViewControl.Location = new System.Drawing.Point(0, 0);
            this.TopViewControl.Name = "TopViewControl";
            this.TopViewControl.Size = new System.Drawing.Size(284, 261);
            this.TopViewControl.TabIndex = 1;
            // 
            // TopViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.TopViewControl);
            this.Name = "TopViewForm";
            this.Text = "Top View";
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.ResumeLayout(false);

        }

        #endregion

        public TopView TopViewControl;
    }
}