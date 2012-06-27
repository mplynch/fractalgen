using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FractalGenerator
{
    /// <summary>
    /// An interface for objects which generate fractals.
    /// </summary>
    public interface IFractalGenerator
    {
        /// <summary>
        /// Creates the fractal using the specified width and height.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>An image containing the generated fractal.</returns>
        Image Create(int width, int height);

        /// <summary>
        /// Gets or sets the concurrency mode of the fractal generation.
        /// </summary>
        /// <value>The mode.</value>
        ConcurrencyMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name
        {
            get;
        }
    }
}
