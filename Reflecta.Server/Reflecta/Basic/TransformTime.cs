#region Using

using System;
using System.Collections.Generic;
using ProtoBuf;

#endregion

namespace Reflecta
{
    [ProtoContract]
    public struct TransformTime
    {
        #region Math

        public static readonly TransformTime Identity = new TransformTime(Vector3.Zero, Quaternion.Identity,
            Vector3.One, 0);

        #endregion

        [ProtoMember(1)] public Vector3 Position;
        [ProtoMember(2)] public Quaternion Rotation;
        [ProtoMember(3)] public Vector3 Scale;
        [ProtoMember(4)] public float Time;

        public TransformTime(Vector3 position, Quaternion rotation, Vector3 scale, float time)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Time = time;
        }

        public static TransformTime Average(params TransformTime[] values)
        {
            if (values == null || values.Length == 0)
                throw new Exception("Empty values list!");

            var positions = new List<Vector3>(values.Length);
            var rotations = new List<Quaternion>(values.Length);
            var scales = new List<Vector3>(values.Length);
            var times = new List<float>(values.Length);

            for (var i = 0; i < values.Length; i++)
            {
                if (values[i].Time != 0)
                {
                    positions.Add(values[i].Position);
                    rotations.Add(values[i].Rotation);
                    scales.Add(values[i].Scale);
                    times.Add(values[i].Time);
                }
            }

            var ret = new TransformTime();
            if (times.Count == 0)
            {
                ret.Position = new Vector3 {X = 0, Y = 0, Z = 0};
                ret.Rotation = new Quaternion {X = 0, Y = 0, Z = 0, W = 1};
                ret.Scale = new Vector3 {X = 1, Y = 1, Z = 1};
                ret.Time = 0;
            }
            else
            {
                ret.Position = Vector3.Average(positions.ToArray());
                ret.Rotation = Quaternion.Average(rotations.ToArray());
                ret.Scale = Vector3.Average(scales.ToArray());
                ret.Time = MathHelper.Average(times.ToArray());
            }
            return ret;
        }
    }
}