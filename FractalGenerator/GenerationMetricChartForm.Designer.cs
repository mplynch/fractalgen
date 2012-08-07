namespace FractalGenerator
{
    partial class GenerationMetricChartForm
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
            this.zedGraphControlMandelbrot = new ZedGraph.ZedGraphControl();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageMandelbrot = new System.Windows.Forms.TabPage();
            this.tabPageJulia = new System.Windows.Forms.TabPage();
            this.zedGraphControlJulia = new ZedGraph.ZedGraphControl();
            this.tabControl1.SuspendLayout();
            this.tabPageMandelbrot.SuspendLayout();
            this.tabPageJulia.SuspendLayout();
            this.SuspendLayout();
            // 
            // zedGraphControlMandelbrot
            // 
            this.zedGraphControlMandelbrot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zedGraphControlMandelbrot.IsEnableHPan = false;
            this.zedGraphControlMandelbrot.IsEnableHZoom = false;
            this.zedGraphControlMandelbrot.IsEnableVPan = false;
            this.zedGraphControlMandelbrot.IsEnableVZoom = false;
            this.zedGraphControlMandelbrot.IsEnableWheelZoom = false;
            this.zedGraphControlMandelbrot.IsShowPointValues = true;
            this.zedGraphControlMandelbrot.Location = new System.Drawing.Point(3, 3);
            this.zedGraphControlMandelbrot.Name = "zedGraphControlMandelbrot";
            this.zedGraphControlMandelbrot.ScrollGrace = 0D;
            this.zedGraphControlMandelbrot.ScrollMaxX = 0D;
            this.zedGraphControlMandelbrot.ScrollMaxY = 0D;
            this.zedGraphControlMandelbrot.ScrollMaxY2 = 0D;
            this.zedGraphControlMandelbrot.ScrollMinX = 0D;
            this.zedGraphControlMandelbrot.ScrollMinY = 0D;
            this.zedGraphControlMandelbrot.ScrollMinY2 = 0D;
            this.zedGraphControlMandelbrot.Size = new System.Drawing.Size(672, 380);
            this.zedGraphControlMandelbrot.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageMandelbrot);
            this.tabControl1.Controls.Add(this.tabPageJulia);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(686, 412);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPageMandelbrot
            // 
            this.tabPageMandelbrot.Controls.Add(this.zedGraphControlMandelbrot);
            this.tabPageMandelbrot.Location = new System.Drawing.Point(4, 22);
            this.tabPageMandelbrot.Name = "tabPageMandelbrot";
            this.tabPageMandelbrot.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMandelbrot.Size = new System.Drawing.Size(678, 386);
            this.tabPageMandelbrot.TabIndex = 0;
            this.tabPageMandelbrot.Text = "Mandelbrot";
            this.tabPageMandelbrot.UseVisualStyleBackColor = true;
            // 
            // tabPageJulia
            // 
            this.tabPageJulia.Controls.Add(this.zedGraphControlJulia);
            this.tabPageJulia.Location = new System.Drawing.Point(4, 22);
            this.tabPageJulia.Name = "tabPageJulia";
            this.tabPageJulia.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageJulia.Size = new System.Drawing.Size(678, 386);
            this.tabPageJulia.TabIndex = 1;
            this.tabPageJulia.Text = "Julia";
            this.tabPageJulia.UseVisualStyleBackColor = true;
            // 
            // zedGraphControlJulia
            // 
            this.zedGraphControlJulia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zedGraphControlJulia.IsEnableHPan = false;
            this.zedGraphControlJulia.IsEnableHZoom = false;
            this.zedGraphControlJulia.IsEnableVPan = false;
            this.zedGraphControlJulia.IsEnableVZoom = false;
            this.zedGraphControlJulia.IsEnableWheelZoom = false;
            this.zedGraphControlJulia.IsShowPointValues = true;
            this.zedGraphControlJulia.Location = new System.Drawing.Point(3, 3);
            this.zedGraphControlJulia.Name = "zedGraphControlJulia";
            this.zedGraphControlJulia.ScrollGrace = 0D;
            this.zedGraphControlJulia.ScrollMaxX = 0D;
            this.zedGraphControlJulia.ScrollMaxY = 0D;
            this.zedGraphControlJulia.ScrollMaxY2 = 0D;
            this.zedGraphControlJulia.ScrollMinX = 0D;
            this.zedGraphControlJulia.ScrollMinY = 0D;
            this.zedGraphControlJulia.ScrollMinY2 = 0D;
            this.zedGraphControlJulia.Size = new System.Drawing.Size(672, 380);
            this.zedGraphControlJulia.TabIndex = 1;
            // 
            // GenerationMetricChartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 412);
            this.Controls.Add(this.tabControl1);
            this.Name = "GenerationMetricChartForm";
            this.ShowInTaskbar = false;
            this.Text = "Fractal Generation Metrics";
            this.tabControl1.ResumeLayout(false);
            this.tabPageMandelbrot.ResumeLayout(false);
            this.tabPageJulia.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl zedGraphControlMandelbrot;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageMandelbrot;
        private System.Windows.Forms.TabPage tabPageJulia;
        private ZedGraph.ZedGraphControl zedGraphControlJulia;

    }
}