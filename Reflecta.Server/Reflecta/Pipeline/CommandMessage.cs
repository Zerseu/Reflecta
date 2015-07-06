#region Using

using System.IO;
using ProtoBuf;

#endregion

namespace Reflecta
{
    [ProtoContract]
    public struct CommandMessage
    {
        [ProtoMember(1)] public CommandType CommandType;
        [ProtoMember(2)] public Viseme Viseme;
        [ProtoMember(3)] public string VoiceCommand;
        [ProtoMember(4)] public MoCapBodyFrame KinectBody;
        [ProtoMember(5)] public MoCapFaceFrame KinectFace;
        [ProtoMember(6)] public char KeyPress;
        [ProtoMember(7)] public int KeyDown;
        [ProtoMember(8)] public int KeyUp;

        public static byte[] ToBytes(CommandMessage message)
        {
            using (var memoryStream = new MemoryStream())
            {
                Serializer.SerializeWithLengthPrefix(memoryStream, message, PrefixStyle.Fixed32);
                return memoryStream.GetBuffer();
            }
        }

        public static CommandMessage FromBytes(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream(bytes))
            {
                return Serializer.DeserializeWithLengthPrefix<CommandMessage>(memoryStream, PrefixStyle.Fixed32);
            }
        }
    }
}