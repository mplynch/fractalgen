using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FractalGenerator
{
    public class GenerationMetric
    {
        #region Fields

        /// <summary>
        /// The mode of generation.
        /// </summary>
        private ConcurrencyMode mode;

        /// <summary>
        /// The number of seconds required to process the fractal.
        /// </summary>
        private double seconds;

        /// <summary>
        /// The height of the fractal image.
        /// </summary>
        private int height;

        /// <summary>
        /// The width of the fractal image.
        /// </summary>
        private int width;

        /// <summary>
        /// The type of the fractal.
        /// </summary>
        private string type;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerationMetric"/>
        /// class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="seconds">The seconds.</param>
        public GenerationMetric(string type, ConcurrencyMode mode, int width,
            int height, double seconds)
        {
            this.type = type;

            this.mode = mode;

            this.width = width;

            this.height = height;

            this.seconds = seconds;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this
        /// instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return (type + "," + mode.ToString() + "," + width + "x" + height
                + "," + seconds);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>The header.</value>
        public static string Header
        {
            get
            {
                return "Type,Mode,Resolution,Running Time";
            }
        }

        #endregion
    }
}
