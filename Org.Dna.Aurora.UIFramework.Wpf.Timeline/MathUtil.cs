using System;

namespace Org.Dna.Aurora.UIFramework.Wpf.Timeline {
  /// <summary>
  /// Utility for math.
  /// </summary>
  public static class MathUtil {
    /// <summary>
    /// Reduce <paramref name="reduce"/> from <paramref name="value"/> but never bellow the value one (1).
    /// </summary>
    /// <param name="value"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    public static double ReduceUntilOne(double value, double reduce) {
      double result = Math.Max(1, value - reduce);
      return result;
    }
  }
}
