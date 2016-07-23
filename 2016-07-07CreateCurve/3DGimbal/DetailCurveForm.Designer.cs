namespace _3DGimbal
{
    partial class DetailCurveForm
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
            this.components = new System.ComponentModel.Container();
            this.DetailCurveControl = new ZedGraph.ZedGraphControl();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // DetailCurveControl
            // 
            this.DetailCurveControl.Location = new System.Drawing.Point(12, 12);
            this.DetailCurveControl.Name = "DetailCurveControl";
            this.DetailCurveControl.ScrollGrace = 0D;
            this.DetailCurveControl.ScrollMaxX = 0D;
            this.DetailCurveControl.ScrollMaxY = 0D;
            this.DetailCurveControl.ScrollMaxY2 = 0D;
            this.DetailCurveControl.ScrollMinX = 0D;
            this.DetailCurveControl.ScrollMinY = 0D;
            this.DetailCurveControl.ScrollMinY2 = 0D;
            this.DetailCurveControl.Size = new System.Drawing.Size(42, 71);
            this.DetailCurveControl.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // DetailCurveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 369);
            this.Controls.Add(this.DetailCurveControl);
            this.Name = "DetailCurveForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DetailCurve";
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl DetailCurveControl;
        private System.Windows.Forms.Timer timer1;
    }
}