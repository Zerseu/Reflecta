#region Using

using System;

#endregion

namespace Reflecta
{
    public static class MoCapFacialExpressionMapper
    {
        public static readonly MoCapFacialExpressionCalibration Calibration = MoCapFacialExpressionCalibration.Optimized;

        public static bool IsValidKinectFacialExpression(MoCapKinectFacialExpression facialExpression)
        {
            return facialExpression != MoCapKinectFacialExpression.Unknown &&
                   facialExpression != MoCapKinectFacialExpression.Count;
        }

        public static bool IsValidMixamoFacialExpression(MoCapMixamoFacialExpression facialExpression)
        {
            return facialExpression != MoCapMixamoFacialExpression.Unknown &&
                   facialExpression != MoCapMixamoFacialExpression.LastBlendShape;
        }

        public static void Kinect2Mixamo(MoCapKinectFacialExpression facialExpressionKinect, float weightKinect,
            out MoCapMixamoFacialExpression facialExpressionMixamo, out float weightMixamo)
        {
            facialExpressionMixamo = MoCapMixamoFacialExpression.Unknown;
            weightMixamo = 0.00f;

            if (!IsValidKinectFacialExpression(facialExpressionKinect))
                return;

            var min = Calibration.Calibration[(int) facialExpressionKinect].WeightMin;
            var max = Calibration.Calibration[(int) facialExpressionKinect].WeightMax;

            switch (facialExpressionKinect)
            {
                case MoCapKinectFacialExpression.JawOpen:
                    weightMixamo = NormalizeWeight(max, weightKinect);
                    facialExpressionMixamo = MoCapMixamoFacialExpression.Jaw_Down;
                    break;

                case MoCapKinectFacialExpression.LipPucker:
                    break;

                case MoCapKinectFacialExpression.JawSlideRight:
                    weightMixamo = NormalizeWeight(min, max, weightKinect);
                    if (weightMixamo >= 0)
                    {
                        weightMixamo = +weightMixamo;
                        facialExpressionMixamo = MoCapMixamoFacialExpression.Jaw_Right;
                    }
                    else
                    {
                        weightMixamo = -weightMixamo;
                        facialExpressionMixamo = MoCapMixamoFacialExpression.Jaw_Left;
                    }
                    break;

                case MoCapKinectFacialExpression.LipStretcherRight:
                    break;
                case MoCapKinectFacialExpression.LipStretcherLeft:
                    break;

                case MoCapKinectFacialExpression.LipCornerPullerLeft:
                    weightMixamo = NormalizeWeight(max, weightKinect);
                    facialExpressionMixamo = MoCapMixamoFacialExpression.Smile_Left;
                    break;
                case MoCapKinectFacialExpression.LipCornerPullerRight:
                    weightMixamo = NormalizeWeight(max, weightKinect);
                    facialExpressionMixamo = MoCapMixamoFacialExpression.Smile_Right;
                    break;

                case MoCapKinectFacialExpression.LipCornerDepressorLeft:
                    weightMixamo = NormalizeWeight(max, weightKinect);
                    facialExpressionMixamo = MoCapMixamoFacialExpression.Frown_Left;
                    break;
                case MoCapKinectFacialExpression.LipCornerDepressorRight:
                    weightMixamo = NormalizeWeight(max, weightKinect);
                    facialExpressionMixamo = MoCapMixamoFacialExpression.Frown_Right;
                    break;

                case MoCapKinectFacialExpression.LeftcheekPuff:
                    weightMixamo = NormalizeWeight(max, weightKinect);
                    facialExpressionMixamo = MoCapMixamoFacialExpression.CheekPuff_Left;
                    break;
                case MoCapKinectFacialExpression.RightcheekPuff:
                    weightMixamo = NormalizeWeight(max, weightKinect);
                    facialExpressionMixamo = MoCapMixamoFacialExpression.CheekPuff_Right;
                    break;

                case MoCapKinectFacialExpression.LefteyeClosed:
                    weightMixamo = NormalizeWeight(max, weightKinect);
                    facialExpressionMixamo = MoCapMixamoFacialExpression.Blink_Left;
                    break;
                case MoCapKinectFacialExpression.RighteyeClosed:
                    weightMixamo = NormalizeWeight(max, weightKinect);
                    facialExpressionMixamo = MoCapMixamoFacialExpression.Blink_Right;
                    ;
                    break;

                case MoCapKinectFacialExpression.RighteyebrowLowerer:
                    weightMixamo = NormalizeWeight(min, max, weightKinect);
                    if (weightMixamo >= 0)
                    {
                        weightMixamo = +weightMixamo;
                        facialExpressionMixamo = MoCapMixamoFacialExpression.BrowsDown_Right;
                    }
                    else
                    {
                        weightMixamo = -weightMixamo;
                        facialExpressionMixamo = MoCapMixamoFacialExpression.BrowsUp_Right;
                    }
                    break;
                case MoCapKinectFacialExpression.LefteyebrowLowerer:
                    weightMixamo = NormalizeWeight(min, max, weightKinect);
                    if (weightMixamo >= 0)
                    {
                        weightMixamo = +weightMixamo;
                        facialExpressionMixamo = MoCapMixamoFacialExpression.BrowsDown_Left;
                    }
                    else
                    {
                        weightMixamo = -weightMixamo;
                        facialExpressionMixamo = MoCapMixamoFacialExpression.BrowsUp_Left;
                    }
                    break;

                case MoCapKinectFacialExpression.LowerlipDepressorLeft:
                    weightMixamo = NormalizeWeight(max, weightKinect);
                    facialExpressionMixamo = MoCapMixamoFacialExpression.LowerLipDown_Left;
                    break;
                case MoCapKinectFacialExpression.LowerlipDepressorRight:
                    weightMixamo = NormalizeWeight(max, weightKinect);
                    facialExpressionMixamo = MoCapMixamoFacialExpression.LowerLipDown_Right;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public static void Mixamo2Kinect(MoCapMixamoFacialExpression facialExpressionMixamo, float weightMixamo,
            out MoCapKinectFacialExpression facialExpressionKinect, out float weightKinect)
        {
            throw new NotImplementedException();
        }

        private static float NormalizeWeight(float peak, float x)
        {
            peak = Math.Abs(peak);
            x = Math.Abs(x);

            return (float) (100*Math.Pow(x/peak, 3));
        }

        private static float NormalizeWeight(float peakMin, float peakMax, float x)
        {
            peakMin = -Math.Abs(peakMin);
            peakMax = Math.Abs(peakMax);

            if (x < 0)
                return -NormalizeWeight(-peakMin, -x);
            return NormalizeWeight(peakMax, x);
        }
    }
}