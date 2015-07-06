#region Using

using System;

#endregion

namespace Reflecta
{
    [Serializable]
    public struct MoCapFacialExpressionCalibrationUnit
    {
        public float WeightMax;
        public float WeightMin;

        public MoCapFacialExpressionCalibrationUnit(float weightMin, float weightMax)
        {
            WeightMin = weightMin;
            WeightMax = weightMax;
        }
    }
}