﻿using Stemmer.Cvb;
using System;
using System.Runtime.Serialization;

namespace CVBImageProc.Processing.Filter
{
  /// <summary>
  /// Laplace filter processor.
  /// </summary>
  [DataContract]
  public class Laplace : WeightedFilterBase
  {
    /// <summary>
    /// Name of the filter.
    /// </summary>
    public override string Name => "Laplace";

    /// <summary>
    /// Processes the <paramref name="inputImage"/>.
    /// </summary>
    /// <param name="inputImage">Image to process.</param>
    /// <returns>Processed image.</returns>
    public override Image Process(Image inputImage)
    {
      if (inputImage == null)
        throw new ArgumentNullException(nameof(inputImage));

      var kernel = CalculateKernel(KernelSize);
      var plane = ProcessingHelper.ProcessMonoKernel(inputImage.Planes[PlaneIndex], (kl) =>
      {
        return ApplyWeights(kl, kernel);
      }, KernelSize, this.GetProcessingBounds(inputImage), PixelFilter);

      plane.CopyTo(inputImage.Planes[PlaneIndex]);
      return inputImage;
    }

    /// <summary>
    /// Calculate a laplace kernel for the given <paramref name="kernelSize"/>.
    /// </summary>
    /// <param name="kernelSize">Kernel size to calculate laplace kernel for.</param>
    /// <returns>Calculated laplace kernel.</returns>
    private static int[] CalculateKernel(KernelSize kernelSize)
    {
      int kernelNum = kernelSize.GetKernelNumber();
      int fullKernelNum = kernelNum * kernelNum;
      var kernel = new int[fullKernelNum];
      for (int i = 0; i < kernel.Length; i++)
        kernel[i] = 1;
      kernel[(int)Math.Floor(fullKernelNum / 2.0)] = 1 - fullKernelNum;

      return kernel;
    }
  }
}