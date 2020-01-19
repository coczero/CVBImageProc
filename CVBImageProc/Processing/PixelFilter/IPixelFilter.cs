﻿namespace CVBImageProc.Processing.PixelFilter
{
  /// <summary>
  /// Interface for an object checking validity of pixels.
  /// </summary>
  public interface IPixelFilter
  {
    /// <summary>
    /// Name of the filter.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Byte to compare to.
    /// </summary>
    byte CompareByte { get; set; }

    /// <summary>
    /// Max value of the <see cref="CompareByte"/>.
    /// </summary>
    byte MaxCompareByte { get; }

    /// <summary>
    /// Min value of the <see cref="CompareByte"/>.
    /// </summary>
    byte MinCompareByte { get; }

    /// <summary>
    /// If true, inverts the logic of the
    /// <see cref="Check(byte)"/>.
    /// </summary>
    bool Not { get; set; }

    /// <summary>
    /// Checks if the given <paramref name="pixel"/>
    /// passes the filter.
    /// </summary>
    /// <param name="pixel">Pixel to check.</param>
    /// <returns>True if the <paramref name="pixel"/> passes
    /// the filter, otherwise false.</returns>
    bool Check(byte pixel);
  }
}