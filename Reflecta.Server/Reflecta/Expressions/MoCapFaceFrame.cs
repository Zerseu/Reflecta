#region Using

using System;
using ProtoBuf;

#endregion

namespace Reflecta
{
    [ProtoContract]
    public struct MoCapFaceFrame
    {
        [ProtoMember(1)] public TransformTime FaceTransform;
        [ProtoMember(2)] public float[] ExpressionWeights;

        public static MoCapFaceFrame Average(params MoCapFaceFrame[] values)
        {
            if (values == null || values.Length == 0)
                throw new Exception("Empty values list!");

            var transforms = new TransformTime[values.Length];

            for (var i = 0; i < values.Length; i++)
                transforms[i] = values[i].FaceTransform;

            var weights = new float[(int) MoCapKinectFacialExpression.Count][];
            for (var i = 0; i < (int) MoCapKinectFacialExpression.Count; i++)
            {
                weights[i] = new float[values.Length];

                for (var j = 0; j < values.Length; j++)
                    weights[i][j] = values[j].ExpressionWeights[i];
            }

            var ret = new MoCapFaceFrame
            {
                ExpressionWeights = new float[(int) MoCapKinectFacialExpression.Count]
            };

            ret.FaceTransform = TransformTime.Average(transforms);

            for (var i = 0; i < (int) MoCapKinectFacialExpression.Count; i++)
                ret.ExpressionWeights[i] = MathHelper.Average(weights[i]);

            return ret;
        }
    }
}