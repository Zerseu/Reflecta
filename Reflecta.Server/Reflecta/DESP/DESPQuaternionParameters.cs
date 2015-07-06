#region Using

using System;

#endregion

namespace Reflecta
{
    [Serializable]
    public struct DESPQuaternionParameters
    {
        public static readonly DESPQuaternionParameters Low =
            new DESPQuaternionParameters(0.5f, 0.5f,
                0.5f, 0.05f, 0.04f);

        public static readonly DESPQuaternionParameters Medium = new DESPQuaternionParameters(0.5f,
            0.1f, 0.5f, 0.1f, 0.1f);

        public static readonly DESPQuaternionParameters High = new DESPQuaternionParameters(0.7f, 0.3f,
            1.0f, 1.0f, 1.0f);

        public float Correction;
        public float JitterRadius;
        public float MaxDeviationRadius;
        public float Prediction;
        public float Smoothing;

        public DESPQuaternionParameters(float smoothing, float correction, float prediction,
            float jitterRadius,
            float maxDeviationRadius)
        {
            Smoothing = smoothing;
            Correction = correction;
            Prediction = prediction;
            JitterRadius = jitterRadius;
            MaxDeviationRadius = maxDeviationRadius;
        }
    }
}