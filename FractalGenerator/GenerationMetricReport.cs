using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FractalGenerator
{
    public class GenerationMetricReport
    {
        #region Fields

        /// <summary>
        /// The list of metrics.
        /// </summary>
        private IList<GenerationMetric> metrics;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="GenerationMetricReport"/> class.
        /// </summary>
        /// <param name="metrics">The metrics.</param>
        public GenerationMetricReport(IList<GenerationMetric> metrics)
        {
            this.metrics = metrics;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Saves the report to the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <exception cref="System.IO.IOException">
        /// Thrown when an error occurs with the underlying IO system.
        /// </exception>
        public void Save(string path)
        {
            StreamWriter writer;

            // Attempt to open the file at the specified path for writing.
            writer = new StreamWriter(path);

            // Write the header.
            writer.WriteLine(GenerationMetric.Header);

            // Loop over the generation metrics.
            foreach (GenerationMetric metric in this.metrics)
            {
                writer.WriteLine(metric.ToString());
            }
            
            // Close the writer.
            writer.Close();
        }

        #endregion
    }
}
