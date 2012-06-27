using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FractalGenerator
{
    /// <summary>
    /// A class which generates the Mandelbrot set.
    /// </summary>
    public class Mandelbrot : IFractalGenerator
    {
        #region Fields

        private ParallelOptions options;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Mandelbrot"/> class.
        /// </summary>
        public Mandelbrot()
        {
            this.Mode = ConcurrencyMode.Sequential;

            this.options = new ParallelOptions();

            this.options.MaxDegreeOfParallelism = Environment.ProcessorCount;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates an image containing the Mandelbrot set.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>An image containing the Mandelbrot set.</returns>
        public Image Create(int width, int height)
        {
            if (this.Mode == ConcurrencyMode.Sequential)
            {
                return this.CreateSequential(width, height);
            }

            else
            {
                return this.CreateParallel(width, height);
            }
        }

        /// <summary>
        /// Creates the Mandelbrot image in parallel.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>An image containing the Mandelbrot set.</returns>
        private Image CreateParallel(int width, int height)
        {
            Bitmap image;
            BitmapData data;
            Color color;
            byte[] buffer;
            double realLeft, realRight, imaginaryBottom, imaginaryTop,
                realFactor, imaginaryFactor;
            int bytes;
            uint maxIterations;

            // Create a bitmap to store the image.
            image = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            // Lock the image into system memory.
            data = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.WriteOnly, image.PixelFormat);

            // Get the number of bytes in the image.
            bytes = data.Stride * image.Height;

            // Create a buffer to store the image data.
            buffer = new byte[bytes];

            // Initialize the position of the set.
            realLeft = -2.0;
            realRight = 1.0;
            imaginaryBottom = -1.2;
            imaginaryTop = imaginaryBottom + (realRight - realLeft) * height / width;

            /* Compute factors for translating from the imaginary plane to the
             * Cartesian plane. */
            realFactor = (realRight - realLeft) / (width - 1);
            imaginaryFactor = (imaginaryTop - imaginaryBottom) / (height - 1);

            // Set the number of iterations to check for values in the set.
            maxIterations = 30;

            // Loop over the rows in parallel.
            Parallel.For(0, height, delegate(int y)
            {
                double c_im = imaginaryTop - y * imaginaryFactor;

                lock (buffer)
                {
                    // Loop over the columns.
                    for (int x = 0; x < width; x++)
                    {
                        double c_re = realLeft + x * realFactor;

                        double Z_re = c_re, Z_im = c_im;

                        uint n;

                        bool isInside = true;

                        /* Loop until a value is proved to be out of the set or
                         * until maxIterations is reached. */
                        for (n = 0; n < maxIterations; ++n)
                        {
                            double Z_re2 = Z_re * Z_re, Z_im2 = Z_im * Z_im;

                            if (Z_re2 + Z_im2 > 4)
                            {
                                isInside = false;

                                break;
                            }

                            Z_im = 2 * Z_re * Z_im + c_im;

                            Z_re = Z_re2 - Z_im2 + c_re;
                        }

                        if (isInside)
                        {
                            color = Color.Black;
                        }

                        else
                        {
                            double colorValue = (double)n / (double)maxIterations * 256.0;

                            if (n < (maxIterations / 2) - 1)
                            {
                                color = Color.FromArgb(0, 0, (int)colorValue);
                            }

                            else
                            {
                                color = Color.FromArgb((int)colorValue, (int)colorValue, 255);
                            }
                        }

                        buffer[y * data.Stride + (x * 3)] = color.B;
                        buffer[y * data.Stride + (x * 3) + 1] = color.G;
                        buffer[y * data.Stride + (x * 3) + 2] = color.R;
                    }
                }
            });

            // Copy the byte buffer to the Bitmap.
            Marshal.Copy(buffer, 0, data.Scan0, bytes);

            // Unlock the Bitmap.
            image.UnlockBits(data);

            // Return the image.
            return (image);
        }

        /// <summary>
        /// Creates the Mandelbrot image sequentially.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>An image containing the Mandelbrot set.</returns>
        private Image CreateSequential(int width, int height)
        {
            Bitmap image;
            Color color;
            double realLeft, realRight, imaginaryBottom, imaginaryTop,
                realFactor, imaginaryFactor;
            uint maxIterations;

            // Create a bitmap to store the image.
            image = new Bitmap(width, height);

            // Initialize the position of the set.
            realLeft = -2.0;
            realRight = 1.0;
            imaginaryBottom = -1.2;
            imaginaryTop = imaginaryBottom + (realRight - realLeft) * height / width;

            /* Compute factors for translating from the imaginary plane to the
             * Cartesian plane. */
            realFactor = (realRight - realLeft) / (width - 1);
            imaginaryFactor = (imaginaryTop - imaginaryBottom) / (height - 1);

            // Set the number of iterations to check for values in the set.
            maxIterations = 30;

            // Loop over the rows.
            for (int y = 0; y < height; ++y)
            {
                double c_im = imaginaryTop - y * imaginaryFactor;

                // Loop over the columns.
                for (int x = 0; x < width; ++x)
                {
                    double c_re = realLeft + x * realFactor;

                    double Z_re = c_re, Z_im = c_im;

                    uint n;

                    bool isInside = true;

                    /* Loop until a value is proved to be out of the set or
                     * until maxIterations is reached. */
                    for (n = 0; n < maxIterations; ++n)
                    {
                        double Z_re2 = Z_re * Z_re, Z_im2 = Z_im * Z_im;

                        if (Z_re2 + Z_im2 > 4)
                        {
                            isInside = false;

                            break;
                        }

                        Z_im = 2 * Z_re * Z_im + c_im;

                        Z_re = Z_re2 - Z_im2 + c_re;
                    }

                    if (isInside)
                    {
                        color = Color.Black;
                    }

                    else
                    {
                        double colorValue = (double)n / (double)maxIterations * 256.0;

                        if (n < (maxIterations / 2) - 1)
                        {
                            color = Color.FromArgb(0, 0, (int)colorValue);
                        }

                        else
                        {
                            color = Color.FromArgb((int)colorValue, (int)colorValue, 255);
                        }

                    }

                    image.SetPixel(x, y, color);
                }
            }

            return (image);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the concurrency mode.
        /// </summary>
        /// <value>The mode.</value>
        public ConcurrencyMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return "Mandelbrot";
            }
        }

        #endregion
    }
}