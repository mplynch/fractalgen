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
using Cloo;

namespace FractalGenerator
{
    /// <summary>
    /// A class which generates the Julia set.
    /// </summary>
    public class Julia : IFractalGenerator
    {
        #region Constants

        private readonly uint MAX_ITERATIONS = 100;

        #endregion

        #region Fields

        private ParallelOptions options;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Julia"/> class.
        /// </summary>
        public Julia()
        {
            this.Mode = ConcurrencyMode.SequentialCPU;

            this.options = new ParallelOptions();

            this.options.MaxDegreeOfParallelism = Environment.ProcessorCount;
        }

        #endregion

        #region Kernels

        private static string kernelSource = @"
            __kernel void mandelbrot(const float realFactor,
            const float imaginaryFactor, const float realLeft,
            const float imaginaryBottom, const float imaginaryTop, 
            const unsigned int maxIterations, const unsigned int width,
            __global char* buffer)
            {
                int xDim = get_global_id(0);
                int yDim = get_global_id(1);
                int index = (4 * width * yDim) + (xDim * 4);

                float c_re = -0.4f;
                float c_im = 0.6f;

                float Z_re = realLeft + xDim * realFactor;
                float Z_im = imaginaryTop - yDim * imaginaryFactor;

                uint n;

                bool isInside = true;

                /* Loop until a value is proved to be out of the set or
                    * until maxIterations is reached. */
                for (n = 0; n < maxIterations; ++n)
                {
                    float Z_re2 = Z_re * Z_re, Z_im2 = Z_im * Z_im;

                    if (Z_re2 + Z_im2 > 4)
                    {
                        isInside = false;

                        break;
                    }

                    Z_im = 2 * Z_re * Z_im + c_im;

                    Z_re = Z_re2 - Z_im2 + c_re;
                }

                buffer[index] = 0xff;

                if (isInside)
                {
                    buffer[index + 1] = 0x0;
                    buffer[index + 2] = 0x0;
                    buffer[index + 3] = 0x0;
                }

                else
                {
                    float colorValue = (float)n / (float)maxIterations * 256.0f;

                    if (n  < (maxIterations / 2) - 1)
                    {
                        buffer[index + 1] = 0x0;
                        buffer[index + 2] = 0x0;
                        buffer[index + 3] = (char)colorValue;
                    }

                    else
                    {
                        buffer[index + 1] = (char)colorValue;
                        buffer[index + 2] = (char)colorValue;
                        buffer[index + 3] = 0xff;
                    }
                }
            }";

        #endregion

        #region Methods

        /// <summary>
        /// Creates an image containing the Julia set.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>An image containing the Julia set.</returns>
        public Image Create(int width, int height)
        {
            switch (this.Mode)
            {
                case ConcurrencyMode.SequentialCPU:
                    return this.CreateSequential(width, height);
                case ConcurrencyMode.ParallelCPU:
                    return this.CreateParallel(width, height);
                case ConcurrencyMode.GPU:
                    return this.CreateGPU(width, height);
                default:
                    throw new InvalidOperationException("The specified " +
                        "concurreny mode is invalid.");
            }
        }

        /// <summary>
        /// Creates the Julia image using the GPU.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>An image containing the Julia set.</returns>
        private Image CreateGPU(int width, int height)
        {
            Bitmap image;

            // Initialize OpenCL.
            ComputePlatform platform = ComputePlatform.Platforms[0];
            ComputeContextPropertyList properties =
                new ComputeContextPropertyList(platform);
            ComputeContext context = new ComputeContext(platform.Devices,
                properties, null, IntPtr.Zero);

            // Create the OpenCL kernel.
            ComputeProgram program = new ComputeProgram(context, new string[] { kernelSource });
            program.Build(null, null, null, IntPtr.Zero);
            ComputeKernel kernel = program.CreateKernel("mandelbrot");

            // Create objects needed for kernel launch/execution.
            ComputeCommandQueue commands = new ComputeCommandQueue(context,
                context.Devices[0], ComputeCommandQueueFlags.None);
            ComputeEventList events = new ComputeEventList();

            // Initialize the position of the set.
            float realLeft = -1.5f;
            float realRight = 1.5f;
            float imaginaryBottom = -1.2f;
            float imaginaryTop = imaginaryBottom + (realRight - realLeft) * height / width;

            /* Compute factors for translating from the imaginary plane to the
             * Cartesian plane. */
            float realFactor = (realRight - realLeft) / (width - 1);
            float imaginaryFactor = (imaginaryTop - imaginaryBottom) / (height - 1);

            // Set the number of iterations to check for values in the set.
            uint maxIterations = MAX_ITERATIONS;

            // Create a buffer to for kernel output.
            ComputeBuffer<char> kernelOutput = new ComputeBuffer<char>(
                context, ComputeMemoryFlags.WriteOnly, width * height * 4);

            // Set arguments for the kernel.
            kernel.SetValueArgument<float>(0, realFactor);
            kernel.SetValueArgument<float>(1, imaginaryFactor);
            kernel.SetValueArgument<float>(2, realLeft);
            kernel.SetValueArgument<float>(3, imaginaryBottom);
            kernel.SetValueArgument<float>(4, imaginaryTop);
            kernel.SetValueArgument<uint>(5, maxIterations);
            kernel.SetValueArgument<int>(6, width);
            kernel.SetMemoryArgument(7, kernelOutput);

            // TODO: Scale work group and work item sizes to fit the resolution.
            commands.Execute(kernel, null, new long[] { width, height },
                null, events);

            // Create a pinned buffer for kernel output.
            byte[] kernelResult = new byte[width * height * 4];
            GCHandle kernelResultHandle = GCHandle.Alloc(kernelResult,
                GCHandleType.Pinned);

            // Copy the kernel result into the pinned buffer.
            commands.Read(kernelOutput, false, 0, width * height * 4,
                kernelResultHandle.AddrOfPinnedObject(), events);
            commands.Finish();

            // Free the pinned handle.
            kernelResultHandle.Free();

            unsafe
            {
                fixed (byte* pKernelResult = kernelResult)
                {
                    IntPtr intPtr = new IntPtr((void*)pKernelResult);
                    image = new Bitmap(width, height, width * 4,
                        PixelFormat.Format32bppArgb, intPtr);
                }
            }

            return image;
        }

        /// <summary>
        /// Creates the Julia image in parallel using the CPU.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>An image containing the Julia set.</returns>
        private unsafe Image CreateParallel(int width, int height)
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
            realLeft = -1.5;
            realRight = 1.5;
            imaginaryBottom = -1.2;
            imaginaryTop = imaginaryBottom + (realRight - realLeft) * height / width;

            /* Compute factors for translating from the imaginary plane to the
             * Cartesian plane. */
            realFactor = (realRight - realLeft) / (width - 1);
            imaginaryFactor = (imaginaryTop - imaginaryBottom) / (height - 1);

            // Set the number of iterations to check for values in the set.
            maxIterations = MAX_ITERATIONS;

            // Loop over the rows in parallel.
            Parallel.For(0, height, delegate(int y)
            {
                //double c_im = imaginaryTop - y * imaginaryFactor;
                double c_im = 0.6;

                lock (buffer)
                {
                    // Loop over the columns.
                    for (int x = 0; x < width; x++)
                    {
                        //double c_re = realLeft + x * realFactor;
                        double c_re = -0.4;

                        double Z_re = realLeft + x * realFactor;
                        double Z_im = imaginaryTop - y * imaginaryFactor;

                        uint n;

                        bool isInside = true;

                        /* Loop until a value is proved to be out of the set or
                         * until maxIterations is reached. */
                        for (n = 0; n < maxIterations; ++n)
                        {
                            double Z_re2 = Z_re * Z_re;
                            double Z_im2 = Z_im * Z_im;

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
        /// Creates the Julia image sequentially using the CPU.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>An image containing the Julia set.</returns>
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
            realLeft = -1.5;
            realRight = 1.5;
            imaginaryBottom = -1.2;
            imaginaryTop = imaginaryBottom + (realRight - realLeft) * height / width;

            /* Compute factors for translating from the imaginary plane to the
             * Cartesian plane. */
            realFactor = (realRight - realLeft) / (width - 1);
            imaginaryFactor = (imaginaryTop - imaginaryBottom) / (height - 1);

            // Set the number of iterations to check for values in the set.
            maxIterations = MAX_ITERATIONS;

            // Loop over the rows.
            for (int y = 0; y < height; ++y)
            {
                //double c_im = imaginaryTop - y * imaginaryFactor;
                double c_im = 0.6;

                // Loop over the columns.
                for (int x = 0; x < width; ++x)
                {
                    double c_re = -0.4;

                    double Z_re = realLeft + x * realFactor;
                    double Z_im = imaginaryTop - y * imaginaryFactor;

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
                return "Julia";
            }
        }

        #endregion
    }
}
