#region Using

using System;
using System.IO;
using ProtoBuf;

#endregion

namespace Reflecta
{
    [ProtoContract]
    public struct MoCapData
    {
        [ProtoMember(1)] public MoCapBodyFrame[] BodyFrames;
        [ProtoMember(2)] public MoCapFaceFrame[] FaceFrames;

        public static void Serialize(ref MoCapData data, string filename)
        {
            using (var stream = File.Open(filename, FileMode.Create))
            {
                Serializer.Serialize(stream, data);
            }
        }

        public static void Deserialize(string filename, out MoCapData data)
        {
            using (var stream = File.Open(filename, FileMode.Open))
            {
                data = Serializer.Deserialize<MoCapData>(stream);
            }
        }

        public static void Average(ref MoCapData data, int smoothingFactor)
        {
            if (data.BodyFrames == null || data.BodyFrames.Length == 0 || data.FaceFrames == null ||
                data.FaceFrames.Length == 0)
                throw new Exception("Invalid data!");
            if (smoothingFactor <= 0 || smoothingFactor > data.BodyFrames.Length ||
                smoothingFactor > data.FaceFrames.Length)
                throw new Exception("Invalid smoothing factor value!");

            var bodyFrames = new MoCapBodyFrame[data.BodyFrames.Length/smoothingFactor];
            var tempBodyFrames = new MoCapBodyFrame[smoothingFactor];
            for (var i = 0; i < data.BodyFrames.Length/smoothingFactor; i++)
            {
                for (var j = 0; j < smoothingFactor; j++)
                    tempBodyFrames[j] = data.BodyFrames[i*smoothingFactor + j];
                bodyFrames[i] = MoCapBodyFrame.Average(tempBodyFrames);
            }

            var faceFrames = new MoCapFaceFrame[data.FaceFrames.Length/smoothingFactor];
            var tempFaceFrames = new MoCapFaceFrame[smoothingFactor];
            for (var i = 0; i < data.FaceFrames.Length/smoothingFactor; i++)
            {
                for (var j = 0; j < smoothingFactor; j++)
                    tempFaceFrames[j] = data.FaceFrames[i*smoothingFactor + j];
                faceFrames[i] = MoCapFaceFrame.Average(tempFaceFrames);
            }

            data.BodyFrames = bodyFrames;
            data.FaceFrames = faceFrames;
        }
    }
}