#region Using

using System;

#endregion

namespace Reflecta
{
    public sealed class DESPQuaternion
    {
        public DESPQuaternionParameters parameters = DESPQuaternionParameters.High;
        private Quaternion prevFilteredOrientation = Quaternion.Identity;
        private Quaternion prevTrend = Quaternion.Identity;

        public Quaternion Predict(Quaternion value)
        {
            var filteredOrientation = Quaternion.Zero;
            var trend = Quaternion.Zero;

            var diffJitter = Quaternion.RotationBetween(value, prevFilteredOrientation);
            var diffJitterValue = Math.Abs(Quaternion.Angle(diffJitter));
            if (diffJitterValue <= parameters.JitterRadius)
                filteredOrientation = Quaternion.SlerpNeighborhood(prevFilteredOrientation, value,
                    diffJitterValue/parameters.JitterRadius);
            else
                filteredOrientation = value;
            filteredOrientation = Quaternion.SlerpNeighborhood(filteredOrientation,
                prevFilteredOrientation*prevTrend,
                parameters.Smoothing);
            diffJitter = Quaternion.RotationBetween(filteredOrientation, prevFilteredOrientation);

            trend = Quaternion.SlerpNeighborhood(prevTrend, diffJitter,
                parameters.Correction);

            var predictedOrientation = filteredOrientation*
                                       Quaternion.SlerpNeighborhood(Quaternion.Identity, trend,
                                           parameters.Prediction);

            var diff = Quaternion.RotationBetween(predictedOrientation, filteredOrientation);
            var diffValue = Math.Abs(Quaternion.Angle(diff));
            if (diffValue > parameters.MaxDeviationRadius)
                predictedOrientation = Quaternion.SlerpNeighborhood(filteredOrientation, predictedOrientation,
                    parameters.MaxDeviationRadius/diffValue);

            predictedOrientation = Quaternion.Normalize(predictedOrientation);
            filteredOrientation = Quaternion.Normalize(filteredOrientation);
            trend = Quaternion.Normalize(trend);

            prevFilteredOrientation = filteredOrientation;
            prevTrend = trend;

            return predictedOrientation;
        }
    }
}