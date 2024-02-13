namespace LunaUpdater
{
    partial class UpdaterForm
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
			this.labelUpdate = new System.Windows.Forms.Label();
			this.buttonUpdate = new System.Windows.Forms.Button();
			this.buttonIgnore = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// labelUpdate
			// 
			this.labelUpdate.AutoSize = true;
			this.labelUpdate.Location = new System.Drawing.Point(13, 13);
			this.labelUpdate.Name = "labelUpdate";
			this.labelUpdate.Size = new System.Drawing.Size(134, 26);
			this.labelUpdate.TabIndex = 0;
			this.labelUpdate.Text = "An update is available!\r\nDo you want to download?";
			// 
			// buttonUpdate
			// 
			this.buttonUpdate.Location = new System.Drawing.Point(12, 51);
			this.buttonUpdate.Name = "buttonUpdate";
			this.buttonUpdate.Size = new System.Drawing.Size(75, 23);
			this.buttonUpdate.TabIndex = 1;
			this.buttonUpdate.Text = "Update";
			this.buttonUpdate.UseVisualStyleBackColor = true;
			this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
			// 
			// buttonIgnore
			// 
			this.buttonIgnore.Location = new System.Drawing.Point(99, 51);
			this.buttonIgnore.Name = "buttonIgnore";
			this.buttonIgnore.Size = new System.Drawing.Size(75, 23);
			this.buttonIgnore.TabIndex = 2;
			this.buttonIgnore.Text = "Ignore";
			this.buttonIgnore.UseVisualStyleBackColor = true;
			this.buttonIgnore.Click += new System.EventHandler(this.buttonIgnore_Click);
			// 
			// UpdaterForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(186, 87);
			this.Controls.Add(this.buttonIgnore);
			this.Controls.Add(this.buttonUpdate);
			this.Controls.Add(this.labelUpdate);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UpdaterForm";
			this.ShowIcon = false;
			this.Text = "Luna Updater";
			this.Load += new System.EventHandler(this.UpdaterForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelUpdate;
        private System.Windows.Forms.Button buttonUpdate;
        private System.Windows.Forms.Button buttonIgnore;
    }
}

