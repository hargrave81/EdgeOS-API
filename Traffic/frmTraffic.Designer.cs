namespace Traffic
{
    partial class frmTraffic
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
            this.propTraffic = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // propertyGrid1
            // 
            this.propTraffic.Location = new System.Drawing.Point(11, 11);
            this.propTraffic.Name = "propTraffic";
            this.propTraffic.Size = new System.Drawing.Size(779, 431);
            this.propTraffic.TabIndex = 0;
            // 
            // frmTraffic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.propTraffic);
            this.Name = "frmTraffic";
            this.Text = "Traffic Anaylsis";
            this.ResumeLayout(false);

        }

        #endregion

        private PropertyGrid propTraffic;
    }
}