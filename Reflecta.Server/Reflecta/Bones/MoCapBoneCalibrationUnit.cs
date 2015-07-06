#region Using

using System;

#endregion

namespace Reflecta
{
    [Serializable]
    public struct MoCapBoneCalibrationUnit
    {
        public Quaternion Unit;

        public MoCapBoneCalibrationUnit(Quaternion unit)
        {
            Unit = unit;
        }
    }
}