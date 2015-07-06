#region Using

using System;

#endregion

namespace Reflecta
{
    [Serializable]
    public struct MoCapBoneConstraintUnit
    {
        public Vector3 Max;
        public Vector3 Min;

        public MoCapBoneConstraintUnit(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }
    }
}