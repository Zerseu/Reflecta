#region Using

using System;
using System.IO;
using System.Xml.Serialization;

#endregion

namespace Reflecta
{
    [Serializable]
    public struct MoCapFacialExpressionCalibration
    {
        public static readonly MoCapFacialExpressionCalibration Default;
        public static readonly MoCapFacialExpressionCalibration Optimized;
        public MoCapFacialExpressionCalibrationUnit[] Calibration;

        static MoCapFacialExpressionCalibration()
        {
            Default = new MoCapFacialExpressionCalibration
            {
                Calibration = new MoCapFacialExpressionCalibrationUnit[(int) MoCapKinectFacialExpression.Count]
            };

            foreach (MoCapKinectFacialExpression expression in Enum.GetValues(typeof (MoCapKinectFacialExpression)))
                if (MoCapFacialExpressionMapper.IsValidKinectFacialExpression(expression))
                    Default.Calibration[(int) expression] = new MoCapFacialExpressionCalibrationUnit(-1.00f, 1.00f);

            Deserialize(out Optimized);
        }

        public static void Serialize(ref MoCapFacialExpressionCalibration calibration)
        {
            using (var stream = File.Open("Face.calib", FileMode.Create))
            {
                var xml = new XmlSerializer(typeof (MoCapFacialExpressionCalibration));
                xml.Serialize(stream, calibration);
            }
        }

        public static void Deserialize(out MoCapFacialExpressionCalibration calibration)
        {
            using (var stream = File.Open("Face.calib", FileMode.Open))
            {
                var xml = new XmlSerializer(typeof (MoCapFacialExpressionCalibration));
                calibration = (MoCapFacialExpressionCalibration) xml.Deserialize(stream);
            }
        }
    }
}