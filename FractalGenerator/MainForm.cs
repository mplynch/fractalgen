using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace FractalGenerator
{
    public partial class MainForm : Form
    {
        #region Fields

        /// <summary>
        /// The concurrency mode to use for generating fractals.
        /// </summary>
        private ConcurrencyMode concurrencyMode;

        /// <summary>
        /// The last known form window state.
        /// </summary>
        private FormWindowState lastFormWindowState;

        /// <summary>
        /// The fractal generator currently in use.
        /// </summary>
        private IFractalGenerator currentGenerator;

        /// <summary>
        /// The Julia set generator.
        /// </summary>
        private Julia julia;

        /// <summary>
        /// The generation metrics collected.
        /// </summary>
        private List<GenerationMetric> metrics;

        /// <summary>
        /// The Mandelbrot set generator.
        /// </summary>
        private Mandelbrot mandelbrot;

        /// <summary>
        /// A high-performance timer used to measure execution times.
        /// </summary>
        private Stopwatch stopwatch;

        /// <summary>
        /// A flag denoting whether or not the application is in full screen mode.
        /// </summary>
        private bool fullScreenMode;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            // Create a Mandelbrot set generator.
            mandelbrot = new Mandelbrot();

            // Create the Julia set generator.
            julia = new Julia();

            // Create the list to store generation metrics.
            this.metrics = new List<GenerationMetric>();

            // Create a stopwatch and start it.
            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Draws the Mandelbrot set.
        /// </summary>
        private void Draw()
        {
            long start, end;
            double seconds;

            // Set the concurrency mode of the generator.
            this.currentGenerator.Mode = this.concurrencyMode;

            // Get the start time.
            start = stopwatch.ElapsedMilliseconds;

            // Draw the image.
            this.pictureBox1.Image = this.currentGenerator.Create(
                this.pictureBox1.Width, this.pictureBox1.Height);

            // Get the end time.
            end = stopwatch.ElapsedMilliseconds;

            // Display the resolution of the image.
            this.toolStripStatusLabelResolution.Text = this.pictureBox1.Width +
                "x" + this.pictureBox1.Height;

            // Store the seconds to generate the fractal.
            seconds = (end - start)/1000.0;

            // Display the running time in seconds.
            this.toolStripStatusLabelRunningTime.Text = seconds + "s";

            // Add a generation metric.
            this.metrics.Add(new GenerationMetric(this.currentGenerator.Name,
                this.concurrencyMode, this.pictureBox1.Width,
                this.pictureBox1.Height, seconds));
        }

        /// <summary>
        /// Toggles the concurrency mode.
        /// </summary>
        private void SetConcurrencyMode(ConcurrencyMode mode)
        {
            this.concurrencyMode = mode;

            this.parallelToolStripMenuItem.Checked = false;
            this.sequentialToolStripMenuItem.Checked = false;
            this.gPUToolStripMenuItem.Checked = false;

            if (this.concurrencyMode == ConcurrencyMode.ParallelCPU)
                this.parallelToolStripMenuItem.Checked = true;

            else if (this.concurrencyMode == ConcurrencyMode.SequentialCPU)
                this.sequentialToolStripMenuItem.Checked = true;

            else if (this.concurrencyMode == ConcurrencyMode.GPU)
                this.gPUToolStripMenuItem.Checked = true;
        }

        /// <summary>
        /// Sets the current fractal generator.
        /// </summary>
        /// <param name="generator">The generator.</param>
        private void SetCurrentGenerator(IFractalGenerator generator)
        {
            this.currentGenerator = generator;

            if (this.currentGenerator == this.mandelbrot)
            {
                this.mandelbrotToolStripMenuItem.Checked = true;
                this.juliaToolStripMenuItem.Checked = false;
            }

            else
            {
                this.mandelbrotToolStripMenuItem.Checked = false;
                this.juliaToolStripMenuItem.Checked = true;
            }
        }

        /// <summary>
        /// Sets the resolution to the specified width and height.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        private void SetResolution(int width, int height)
        {
            int clientWidth, clientHeight, offsetWidth, offsetHeight;

            // Get the width of the client area.
            clientWidth = this.pictureBox1.ClientRectangle.Right -
                this.pictureBox1.ClientRectangle.Left;

            // Get the height of the client area.
            clientHeight = this.pictureBox1.ClientRectangle.Bottom -
                this.pictureBox1.ClientRectangle.Top;

            // Get the offset in width between the form and the client area.
            offsetWidth = this.Width - clientWidth;

            // Get the offset in height between the form and the client area.
            offsetHeight = this.Height - clientHeight;

            /* Set the width of the form to get the client area to the
             * specified resolution. */
            this.Width = width + offsetWidth;

            /* Set the height of the form to get the client area to the
             * specified resolution. */
            this.Height = height + offsetHeight;

            // Draw the fractal.
            this.Draw();
        }

        /// <summary>
        /// Toggles full screen mode.
        /// </summary>
        private void ToggleFullScreenMode()
        {
            this.SuspendLayout();

            if (this.fullScreenMode)
            {
                this.fullScreenMode = false;

                this.menuStrip1.Show();
                this.statusStrip1.Show();

                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.Sizable;
            }

            else
            {
                this.fullScreenMode = true;

                this.menuStrip1.Hide();
                this.statusStrip1.Hide();

                this.WindowState = FormWindowState.Maximized;
                this.FormBorderStyle = FormBorderStyle.None;
            }

            this.ResumeLayout();
            this.PerformLayout();

            this.Draw();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the Click event of the fullScreenToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event
        /// data.
        /// </param>
        private void fullScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ToggleFullScreenMode();
        }

        /// <summary>
        /// Handles the Click event of the gpuToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event
        /// data.
        /// </param>
        private void gpuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Set the concurrency mode to parallel.
            this.SetConcurrencyMode(ConcurrencyMode.GPU);

            // Draw the current fractal.
            this.Draw();
        }

        /// <summary>
        /// Handles the KeyPress event of the MainForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.KeyPressEventArgs"/> instance
        /// containing the event data.
        /// </param>
        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.fullScreenMode && (e.KeyChar == (char)Keys.Escape))
            {
                this.ToggleFullScreenMode();
            }
        }

        /// <summary>
        /// Handles the Load event of the MainForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event
        /// data.
        /// </param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Store the window state.
            this.lastFormWindowState = this.WindowState;

            // Initialize the current generator.
            this.SetCurrentGenerator(this.mandelbrot);

            // Initialize the concurrency mode.
            this.SetConcurrencyMode(ConcurrencyMode.ParallelCPU);

            // Draw the current fractal.
            this.Draw();
        }

        /// <summary>
        /// Handles the Resize event of the MainForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event
        /// data.
        /// </param>
        private void MainForm_Resize(object sender, EventArgs e)
        {
            // The window state has changed?
            if (this.WindowState != this.lastFormWindowState)
            {
                // Redraw the fractal.
                this.Draw();

                // Store the window state.
                this.lastFormWindowState = this.WindowState;
            }
        }

        /// <summary>
        /// Handles the ResizeEnd event of the MainForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event
        /// data.
        /// </param>
        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            // Redraw the fractal.
            this.Draw();
        }

        /// <summary>
        /// Handles the Click event of the juliaToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void juliaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Set the current generator to the Julia set.
            this.SetCurrentGenerator(this.julia);

            // Draw the current fractal.
            this.Draw();
        }

        /// <summary>
        /// Handles the Click event of the mandelbrotToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mandelbrotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Set the current generator to the Mandelbrot set.
            this.SetCurrentGenerator(this.mandelbrot);

            // Draw the current fractal.
            this.Draw();
        }

        /// <summary>
        /// Handles the Click event of the parallelToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event
        /// data.
        /// </param>
        private void parallelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Set the concurrency mode to parallel.
            this.SetConcurrencyMode(ConcurrencyMode.ParallelCPU);

            // Draw the current fractal.
            this.Draw();
        }

        /// <summary>
        /// Handles the Click event of the pictureBox1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event
        /// data.
        /// </param>
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (this.fullScreenMode)
            {
                this.ToggleFullScreenMode();
            }
        }

        /// <summary>
        /// Handles the MouseMove event of the pictureBox1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.MouseEventArgs"/> instance
        /// containing the event data.
        /// </param>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            this.toolStripStatusLabelMousePosition.Text = e.X + "," + e.Y;
        }

        /// <summary>
        /// Handles the Click event of the refreshToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event
        /// data.
        /// </param>
        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Draw the current fractal.
            this.Draw();
        }

        /// <summary>
        /// Handles the Click event of the res320x240ToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event
        /// data.
        /// </param>
        private void res320x240ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetResolution(320, 240);
        }

        /// <summary>
        /// Handles the Click event of the res640x480ToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event
        /// data.
        /// </param>
        private void res640x480ToolStripMenuItem_Click(object sender,
            EventArgs e)
        {
            this.SetResolution(640, 480);
        }

        /// <summary>
        /// Handles the Click event of the res1024x768ToolStripMenuItem
        /// control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private void res1024x768ToolStripMenuItem_Click(object sender,
            EventArgs e)
        {
            this.SetResolution(1024, 768);
        }

        /// <summary>
        /// Handles the Click event of the saveToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event
        /// data.
        /// </param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;
            GenerationMetricReport report;

            // Prompt the user to save the report.
            result = this.saveFileDialog1.ShowDialog(this);

            // The user canceled the dialog?
            if (result == DialogResult.Cancel)
                return;

            // Create a new report.
            report = new GenerationMetricReport(this.metrics);
            
            // Try to save the report.
            try
            {
                // Save the report to the specified file.
                report.Save(this.saveFileDialog1.FileName);
            }

            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while attempting to save " +
                    "the report.  Details can be found below." +
                    Environment.NewLine + Environment.NewLine + ex.Message,
                    "Error");
            }
        }

        /// <summary>
        /// Handles the Click event of the sequentialToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event
        /// data.
        /// </param>
        private void sequentialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Set the concurrency mode to sequential.
            this.SetConcurrencyMode(ConcurrencyMode.SequentialCPU);

            // Draw the current fractal.
            this.Draw();
        }

        #endregion
    }
}
