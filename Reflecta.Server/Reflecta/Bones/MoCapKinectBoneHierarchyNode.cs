#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Reflecta
{
    public sealed class MoCapKinectBoneHierarchyNode
    {
        public IEnumerable<MoCapKinectBoneHierarchyNode> Children;
        public MoCapKinectBone Current;
        public MoCapKinectBoneHierarchyNode Parent;

        public MoCapKinectBoneHierarchyNode this[MoCapKinectBone bone]
        {
            get
            {
                if (!MoCapBoneMapper.IsValidKinectBone(bone))
                    throw new ArgumentException("Invalid Kinect bone!");

                if (Current == bone)
                    return this;

                foreach (var child in Children)
                {
                    var value = child[bone];
                    if (value != null)
                        return value;
                }

                return null;
            }
        }
    }
}