#region Using

using System;

#endregion

namespace Reflecta
{
    public static class MoCapBoneMapper
    {
        public static readonly MoCapBoneCalibration Calibration = MoCapBoneCalibration.Default;
        private static readonly MoCapKinectBoneHierarchyNode KinectBoneHierarchyRoot;
        private static readonly MoCapMecanimBone[] Kinect2MecanimMapping;
        private static readonly MoCapKinectBone[] Mecanim2KinectMapping;

        static MoCapBoneMapper()
        {
            #region Bone Hierarchy

            var KinectBoneHierarchyNodes =
                new MoCapKinectBoneHierarchyNode[(int) MoCapKinectBone.Count];
            for (var kinectBoneIndex = 0; kinectBoneIndex < KinectBoneHierarchyNodes.Length; kinectBoneIndex++)
                KinectBoneHierarchyNodes[kinectBoneIndex] = new MoCapKinectBoneHierarchyNode
                {
                    Parent = null,
                    Current = (MoCapKinectBone) kinectBoneIndex,
                    Children = null
                };

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.SpineBase].Parent = null;
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.SpineBase].Children = new[]
            {
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.SpineMid],
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.HipLeft],
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.HipRight]
            };

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.SpineMid].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.SpineBase];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.SpineMid].Children = new[]
            {KinectBoneHierarchyNodes[(int) MoCapKinectBone.SpineShoulder]};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.SpineShoulder].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.SpineMid];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.SpineShoulder].Children =
                new[]
                {
                    KinectBoneHierarchyNodes[(int) MoCapKinectBone.Neck],
                    KinectBoneHierarchyNodes[(int) MoCapKinectBone.ShoulderLeft],
                    KinectBoneHierarchyNodes[(int) MoCapKinectBone.ShoulderRight]
                };

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.Neck].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.SpineShoulder];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.Neck].Children = new[]
            {KinectBoneHierarchyNodes[(int) MoCapKinectBone.Head]};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.Head].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.Neck];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.Head].Children = new MoCapKinectBoneHierarchyNode[] {};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.ShoulderLeft].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.SpineShoulder];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.ShoulderLeft].Children = new[]
            {KinectBoneHierarchyNodes[(int) MoCapKinectBone.ElbowLeft]};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.ElbowLeft].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.ShoulderLeft];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.ElbowLeft].Children = new[]
            {KinectBoneHierarchyNodes[(int) MoCapKinectBone.WristLeft]};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.WristLeft].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.ElbowLeft];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.WristLeft].Children = new[]
            {KinectBoneHierarchyNodes[(int) MoCapKinectBone.HandLeft]};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.HandLeft].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.WristLeft];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.HandLeft].Children = new[]
            {
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.ThumbLeft],
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.HandTipLeft]
            };

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.ThumbLeft].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.HandLeft];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.ThumbLeft].Children = new MoCapKinectBoneHierarchyNode[] {};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.HandTipLeft].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.HandLeft];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.HandTipLeft].Children = new MoCapKinectBoneHierarchyNode[] {};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.ShoulderRight].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.SpineShoulder];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.ShoulderRight].Children =
                new[] {KinectBoneHierarchyNodes[(int) MoCapKinectBone.ElbowRight]};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.ElbowRight].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.ShoulderRight];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.ElbowRight].Children = new[]
            {KinectBoneHierarchyNodes[(int) MoCapKinectBone.WristRight]};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.WristRight].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.ElbowRight];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.WristRight].Children = new[]
            {KinectBoneHierarchyNodes[(int) MoCapKinectBone.HandRight]};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.HandRight].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.WristRight];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.HandRight].Children = new[]
            {
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.ThumbRight],
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.HandTipRight]
            };

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.ThumbRight].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.HandRight];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.ThumbRight].Children = new MoCapKinectBoneHierarchyNode[] {};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.HandTipRight].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.HandRight];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.HandTipRight].Children = new MoCapKinectBoneHierarchyNode[]
            {};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.HipLeft].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.SpineBase];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.HipLeft].Children = new[]
            {KinectBoneHierarchyNodes[(int) MoCapKinectBone.KneeLeft]};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.KneeLeft].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.HipLeft];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.KneeLeft].Children = new[]
            {KinectBoneHierarchyNodes[(int) MoCapKinectBone.AnkleLeft]};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.AnkleLeft].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.KneeLeft];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.AnkleLeft].Children = new[]
            {KinectBoneHierarchyNodes[(int) MoCapKinectBone.FootLeft]};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.FootLeft].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.AnkleLeft];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.FootLeft].Children = new MoCapKinectBoneHierarchyNode[] {};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.HipRight].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.SpineBase];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.HipRight].Children = new[]
            {KinectBoneHierarchyNodes[(int) MoCapKinectBone.KneeRight]};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.KneeRight].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.HipRight];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.KneeRight].Children = new[]
            {KinectBoneHierarchyNodes[(int) MoCapKinectBone.AnkleRight]};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.AnkleRight].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.KneeRight];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.AnkleRight].Children = new[]
            {KinectBoneHierarchyNodes[(int) MoCapKinectBone.FootRight]};

            KinectBoneHierarchyNodes[(int) MoCapKinectBone.FootRight].Parent =
                KinectBoneHierarchyNodes[(int) MoCapKinectBone.AnkleRight];
            KinectBoneHierarchyNodes[(int) MoCapKinectBone.FootRight].Children = new MoCapKinectBoneHierarchyNode[] {};

            KinectBoneHierarchyRoot = KinectBoneHierarchyNodes[(int) MoCapKinectBone.SpineBase];

            #endregion

            #region Direct Mapping

            Kinect2MecanimMapping = new MoCapMecanimBone[(int) MoCapKinectBone.Count];
            for (var kinectBoneIndex = 0; kinectBoneIndex < Kinect2MecanimMapping.Length; kinectBoneIndex++)
                Kinect2MecanimMapping[kinectBoneIndex] = MoCapMecanimBone.Unknown;

            Kinect2MecanimMapping[(int) MoCapKinectBone.SpineBase] = MoCapMecanimBone.Hips;
            Kinect2MecanimMapping[(int) MoCapKinectBone.SpineMid] = MoCapMecanimBone.Spine;
            Kinect2MecanimMapping[(int) MoCapKinectBone.SpineShoulder] = MoCapMecanimBone.Chest;
            Kinect2MecanimMapping[(int) MoCapKinectBone.Neck] = MoCapMecanimBone.Neck;
            Kinect2MecanimMapping[(int) MoCapKinectBone.Head] = MoCapMecanimBone.Head;

            Kinect2MecanimMapping[(int) MoCapKinectBone.ShoulderLeft] = MoCapMecanimBone.RightShoulder;
            Kinect2MecanimMapping[(int) MoCapKinectBone.ElbowLeft] = MoCapMecanimBone.RightUpperArm;
            Kinect2MecanimMapping[(int) MoCapKinectBone.WristLeft] = MoCapMecanimBone.RightLowerArm;
            Kinect2MecanimMapping[(int) MoCapKinectBone.HandLeft] = MoCapMecanimBone.RightHand;
            Kinect2MecanimMapping[(int) MoCapKinectBone.ThumbLeft] = MoCapMecanimBone.RightThumbDistal;
            Kinect2MecanimMapping[(int) MoCapKinectBone.HandTipLeft] = MoCapMecanimBone.RightMiddleDistal;

            Kinect2MecanimMapping[(int) MoCapKinectBone.ShoulderRight] = MoCapMecanimBone.LeftShoulder;
            Kinect2MecanimMapping[(int) MoCapKinectBone.ElbowRight] = MoCapMecanimBone.LeftUpperArm;
            Kinect2MecanimMapping[(int) MoCapKinectBone.WristRight] = MoCapMecanimBone.LeftLowerArm;
            Kinect2MecanimMapping[(int) MoCapKinectBone.HandRight] = MoCapMecanimBone.LeftHand;
            Kinect2MecanimMapping[(int) MoCapKinectBone.ThumbRight] = MoCapMecanimBone.LeftThumbDistal;
            Kinect2MecanimMapping[(int) MoCapKinectBone.HandTipRight] = MoCapMecanimBone.LeftMiddleDistal;

            Kinect2MecanimMapping[(int) MoCapKinectBone.HipLeft] = MoCapMecanimBone.Unknown;
            Kinect2MecanimMapping[(int) MoCapKinectBone.KneeLeft] = MoCapMecanimBone.RightUpperLeg;
            Kinect2MecanimMapping[(int) MoCapKinectBone.AnkleLeft] = MoCapMecanimBone.RightLowerLeg;
            Kinect2MecanimMapping[(int) MoCapKinectBone.FootLeft] = MoCapMecanimBone.RightFoot;

            Kinect2MecanimMapping[(int) MoCapKinectBone.HipRight] = MoCapMecanimBone.Unknown;
            Kinect2MecanimMapping[(int) MoCapKinectBone.KneeRight] = MoCapMecanimBone.LeftUpperLeg;
            Kinect2MecanimMapping[(int) MoCapKinectBone.AnkleRight] = MoCapMecanimBone.LeftLowerLeg;
            Kinect2MecanimMapping[(int) MoCapKinectBone.FootRight] = MoCapMecanimBone.LeftFoot;

            #endregion

            #region Reverse Mapping

            Mecanim2KinectMapping = new MoCapKinectBone[(int) MoCapMecanimBone.LastBone];
            for (var mecanimBoneIndex = 0; mecanimBoneIndex < Mecanim2KinectMapping.Length; mecanimBoneIndex++)
            {
                Mecanim2KinectMapping[mecanimBoneIndex] = MoCapKinectBone.Unknown;
                for (var kinectBoneIndex = 0; kinectBoneIndex < Kinect2MecanimMapping.Length; kinectBoneIndex++)
                    if (Kinect2MecanimMapping[kinectBoneIndex] == (MoCapMecanimBone) mecanimBoneIndex)
                    {
                        Mecanim2KinectMapping[mecanimBoneIndex] = (MoCapKinectBone) kinectBoneIndex;
                        break;
                    }
            }

            #endregion
        }

        public static MoCapKinectBoneHierarchyNode GetBoneHierarchyNode(MoCapKinectBone bone)
        {
            if (!IsValidKinectBone(bone))
                throw new ArgumentException("Invalid Kinect bone!");

            return KinectBoneHierarchyRoot[bone];
        }

        public static Quaternion LocalRotation(ref MoCapBodyFrame frame, MoCapKinectBone bone,
            bool withCalibration = true)
        {
            if (!IsValidKinectBone(bone))
                throw new ArgumentException("Invalid Kinect bone!");

            var node = GetBoneHierarchyNode(bone);

            if (node.Parent == null)
                return frame.SkeletonTransforms[(int) bone].Rotation;

            var parentBone = node.Parent.Current;

            while (!IsValidMecanimBone(Kinect2Mecanim(parentBone)))
                parentBone = GetBoneHierarchyNode(parentBone).Parent.Current;

            return
                Quaternion.Inverse(frame.SkeletonTransforms[(int) parentBone].Rotation*
                                   (withCalibration
                                       ? Calibration.Calibration[(int) parentBone].Unit
                                       : Quaternion.Identity))*
                (frame.SkeletonTransforms[(int) bone].Rotation*
                 (withCalibration ? Calibration.Calibration[(int) bone].Unit : Quaternion.Identity));
        }

        public static bool IsValidKinectBone(MoCapKinectBone bone)
        {
            return bone != MoCapKinectBone.Unknown && bone != MoCapKinectBone.Count;
        }

        public static bool IsValidMecanimBone(MoCapMecanimBone bone)
        {
            return bone != MoCapMecanimBone.Unknown && bone != MoCapMecanimBone.LastBone;
        }

        public static MoCapMecanimBone Kinect2Mecanim(MoCapKinectBone bone)
        {
            if (!IsValidKinectBone(bone))
                return MoCapMecanimBone.Unknown;
            return Kinect2MecanimMapping[(int) bone];
        }

        public static MoCapKinectBone Mecanim2Kinect(MoCapMecanimBone bone)
        {
            if (!IsValidMecanimBone(bone))
                return MoCapKinectBone.Unknown;
            return Mecanim2KinectMapping[(int) bone];
        }
    }
}