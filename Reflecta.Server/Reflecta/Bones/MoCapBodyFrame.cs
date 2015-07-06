#region Using

using System;
using ProtoBuf;

#endregion

namespace Reflecta
{
    [ProtoContract]
    public struct MoCapBodyFrame
    {
        [ProtoMember(1)] public TransformTime[] SkeletonTransforms;

        public static MoCapBodyFrame Average(params MoCapBodyFrame[] values)
        {
            if (values == null || values.Length == 0)
                throw new Exception("Empty values list!");

            var transforms = new TransformTime[(int) MoCapKinectBone.Count][];
            for (var i = 0; i < (int) MoCapKinectBone.Count; i++)
            {
                transforms[i] = new TransformTime[values.Length];

                for (var j = 0; j < values.Length; j++)
                    transforms[i][j] = values[j].SkeletonTransforms[i];
            }

            var ret = new MoCapBodyFrame
            {
                SkeletonTransforms = new TransformTime[(int) MoCapKinectBone.Count]
            };
            for (var i = 0; i < (int) MoCapKinectBone.Count; i++)
                ret.SkeletonTransforms[i] = TransformTime.Average(transforms[i]);
            return ret;
        }
    }
}