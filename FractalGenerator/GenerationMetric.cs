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
        /// The number of milliseconds required to process the fractal.
        /// </summary>
        private double milliseconds;

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
        /// <param name="type">The type of fractal generated.</param>
        /// <param name="mode">The ConcurrencyMode used to generate the image.</param>
        /// <param name="width">The width of the generated image.</param>
        /// <param name="height">The height of the generated image.</param>
        /// <param name="milliseconds">The milliseconds required to generate the image.</param>
        public GenerationMetric(string type, ConcurrencyMode mode, int width,
            int height, double milliseconds)
        {
            this.type = type;

            this.mode = mode;

            this.width = width;

            this.height = height;

            this.milliseconds = milliseconds;
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
                + "," + milliseconds);
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

        /// <summary>
        /// The height of the generated image.
        /// </summary>
        public int Height
        {
            get
            {
                return this.height;
            }
        }

        /// <summary>
        /// The ConcurrencyMode used to generate the image.
        /// </summary>
        public ConcurrencyMode Mode
        {
            get
            {
                return this.mode;
            }
        }

        /// <summary>
        /// The number of milliseconds required to generate the image.
        /// </summary>
        public double Milliseconds
        {
            get
            {
                return this.milliseconds;
            }
        }

        /// <summary>
        /// The type of fractal generated.
        /// </summary>
        public string Type
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// The width of the generated image.
        /// </summary>
        public int Width
        {
            get
            {
                return this.width;
            }
        }

        #endregion
    }
}
