#region Using

using System;

#endregion

namespace Reflecta
{
    [Serializable]
    public struct MoCapBoneCalibration
    {
        public static readonly MoCapBoneCalibration Default;
        public MoCapBoneCalibrationUnit[] Calibration;

        static MoCapBoneCalibration()
        {
            Default = new MoCapBoneCalibration
            {
                Calibration = new MoCapBoneCalibrationUnit[(int) MoCapKinectBone.Count]
            };

            foreach (MoCapKinectBone bone in Enum.GetValues(typeof (MoCapKinectBone)))
                if (MoCapBoneMapper.IsValidKinectBone(bone))
                    Default.Calibration[(int) bone].Unit = Quaternion.Identity;

            Default.Calibration[(int) MoCapKinectBone.ShoulderLeft].Unit = Quaternion.AxisAngle(Vector3.Up, -180.0f);
            Default.Calibration[(int) MoCapKinectBone.ShoulderRight].Unit = Quaternion.AxisAngle(Vector3.Up, 180.0f);
            Default.Calibration[(int) MoCapKinectBone.ElbowLeft].Unit = Quaternion.AxisAngle(Vector3.Up, -90.0f);
            Default.Calibration[(int) MoCapKinectBone.ElbowRight].Unit = Quaternion.AxisAngle(Vector3.Up, 90.0f);
            Default.Calibration[(int) MoCapKinectBone.WristLeft].Unit = Quaternion.AxisAngle(Vector3.Up, 180.0f);
            Default.Calibration[(int) MoCapKinectBone.WristRight].Unit = Quaternion.AxisAngle(Vector3.Up, -180.0f);
            Default.Calibration[(int) MoCapKinectBone.HandLeft].Unit = Quaternion.AxisAngle(Vector3.Up, 90.0f);
            Default.Calibration[(int) MoCapKinectBone.HandRight].Unit = Quaternion.AxisAngle(Vector3.Up, -90.0f);

            Default.Calibration[(int) MoCapKinectBone.KneeLeft].Unit = Quaternion.AxisAngle(Vector3.Up, 90.0f);
            Default.Calibration[(int) MoCapKinectBone.KneeRight].Unit = Quaternion.AxisAngle(Vector3.Up, -90.0f);
            Default.Calibration[(int) MoCapKinectBone.AnkleLeft].Unit = Quaternion.AxisAngle(Vector3.Up, 90.0f);
            Default.Calibration[(int) MoCapKinectBone.AnkleRight].Unit = Quaternion.AxisAngle(Vector3.Up, -90.0f);
            Default.Calibration[(int) MoCapKinectBone.FootLeft].Unit = Quaternion.AxisAngle(Vector3.Right, -90.0f);
            Default.Calibration[(int) MoCapKinectBone.FootRight].Unit = Quaternion.AxisAngle(Vector3.Right, -90.0f);
        }
    }
}