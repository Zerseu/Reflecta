#region Using

using System;

#endregion

namespace Reflecta
{
    [Serializable]
    public struct MoCapBoneConstraint
    {
        public static readonly MoCapBoneConstraint Default;
        public MoCapBoneConstraintUnit[] Constraint;

        static MoCapBoneConstraint()
        {
            Default = new MoCapBoneConstraint
            {
                Constraint = new MoCapBoneConstraintUnit[(int) MoCapKinectBone.Count]
            };

            foreach (MoCapKinectBone bone in Enum.GetValues(typeof (MoCapKinectBone)))
                if (MoCapBoneMapper.IsValidKinectBone(bone))
                {
                    Default.Constraint[(int) bone].Min = -180.0f*Vector3.One;
                    Default.Constraint[(int) bone].Max = 180.0f*Vector3.One;
                }
        }
    }
}