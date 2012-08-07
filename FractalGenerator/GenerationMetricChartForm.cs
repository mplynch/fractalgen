using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using System.Collections;

namespace FractalGenerator
{
    public partial class GenerationMetricChartForm : Form
    {
        public GenerationMetricChartForm()
        {
            InitializeComponent();
        }

        private void LoadMetrics(IList<GenerationMetric> metrics,
            GraphPane pane)
        {
            List<Size> resolutions = new List<Size>();
            Dictionary<Size, GenerationMetric> serialCPUMetrics =
                new Dictionary<Size, GenerationMetric>();
            Dictionary<Size, GenerationMetric> parallelCPUMetrics =
                new Dictionary<Size, GenerationMetric>();
            Dictionary<Size, GenerationMetric> gpuMetrics =
                new Dictionary<Size, GenerationMetric>();

            pane.Title.Text = "Fractal Generation Metrics";
            pane.XAxis.Title.Text = "Resolution";
            pane.YAxis.Title.Text = "Generation Time (ms)";

            foreach (GenerationMetric metric in metrics)
            {
                Size resolution = new Size(metric.Width, metric.Height);

                if (!resolutions.Contains(resolution))
                    resolutions.Add(resolution);

                if (metric.Mode == ConcurrencyMode.SequentialCPU)
                    this.AddFastestMetric(serialCPUMetrics, metric);
                else if (metric.Mode == ConcurrencyMode.ParallelCPU)
                    this.AddFastestMetric(parallelCPUMetrics, metric);
                else if (metric.Mode == ConcurrencyMode.GPU)
                    this.AddFastestMetric(gpuMetrics, metric);
                else
                    throw new InvalidEnumArgumentException();
            }

            resolutions.Sort(new SizeComparer());

            int numResolutions = resolutions.Count;

            double[] serialCPUValues = new double[numResolutions];
            double[] parallelCPUValues = new double[numResolutions];
            double[] gpuValues = new double[numResolutions];

            for (int i = 0; i < numResolutions; i++)
            {
                if (serialCPUMetrics.ContainsKey(resolutions[i]))
                    serialCPUValues[i] = serialCPUMetrics[resolutions[i]].Milliseconds;
                if (parallelCPUMetrics.ContainsKey(resolutions[i]))
                    parallelCPUValues[i] = parallelCPUMetrics[resolutions[i]].Milliseconds;
                if (gpuMetrics.ContainsKey(resolutions[i]))
                    gpuValues[i] = gpuMetrics[resolutions[i]].Milliseconds;
            }

            BarItem serialCPUBar = pane.AddBar("Serial CPU", null, serialCPUValues, Color.Red);
            BarItem parallelCPUBar = pane.AddBar("Parallel CPU", null, parallelCPUValues, Color.Blue);
            BarItem gpuBar = pane.AddBar("GPU", null, gpuValues, Color.DarkGreen);

            List<string> xAxisLabels = new List<string>(resolutions.Count);
            foreach (Size size in resolutions)
            {
                xAxisLabels.Add(size.Width + "x" + size.Height);
            }

            pane.XAxis.MajorTic.IsBetweenLabels = true;
            pane.XAxis.Scale.TextLabels = xAxisLabels.ToArray<string>();
            pane.XAxis.Type = AxisType.Text;
        }

        /// <summary>
        /// Loads the specified metrics into the appropriate graphs.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        public void LoadMetrics(IList<GenerationMetric> metrics)
        {
            List<GenerationMetric> mandelbrotMetrics =
                new List<GenerationMetric>();
            List<GenerationMetric> juliaMetrics =
                new List<GenerationMetric>();

            foreach (GenerationMetric metric in metrics)
            {
                if (metric.Type == "Julia")
                    juliaMetrics.Add(metric);
                else if (metric.Type == "Mandelbrot")
                    mandelbrotMetrics.Add(metric);
            }

            this.LoadMetrics(juliaMetrics,
                zedGraphControlJulia.GraphPane);

            this.LoadMetrics(mandelbrotMetrics,
                zedGraphControlMandelbrot.GraphPane);

            zedGraphControlJulia.AxisChange();
            zedGraphControlMandelbrot.AxisChange();
        }

        /// <summary>
        /// Adds the specified metric to the specified metric dictionary if th
        /// e metric is the fastest recorded for a given resolution.
        /// </summary>
        /// <param name="metrics">The metric dictionary.</param>
        /// <param name="metric">The metric.</param>
        private void AddFastestMetric(Dictionary<Size, 
            GenerationMetric> metrics, GenerationMetric metric)
        {
            Size size = new Size(metric.Width, metric.Height);

            if (metrics.ContainsKey(size))
            {
                GenerationMetric previous = metrics[size];

                if (previous.Milliseconds > metric.Milliseconds)
                    metrics[size] = metric;
            }

            else
            {
                metrics.Add(size, metric);
            }
        }

        private class SizeComparer : IComparer<Size>
        {
            public int Compare(Size x, Size y)
            {
                int totalX = x.Width * x.Height;
                int totalY = y.Width * y.Height;

                return totalX - totalY;
            }
        }
    }
}
