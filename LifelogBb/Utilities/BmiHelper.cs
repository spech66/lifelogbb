using LifelogBb.Models.Entities;

namespace LifelogBb.Utilities
{
    public static class BmiHelper
    {
        public static double Calculate(double bodyWeight, int height, Measurements unitsType)
        {
            if (unitsType == Measurements.Metric)
            {
                return bodyWeight / Math.Pow(height / 100.0, 2);
            }

            return bodyWeight / (height * height) * 703.0;
        }
    }
}
