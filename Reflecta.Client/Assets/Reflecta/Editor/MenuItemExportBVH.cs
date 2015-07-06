#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

#endregion

public class MenuItemExportBVH : MonoBehaviour
{
    [MenuItem("Reflecta/BVH/Export BVH")]
    private static void ExportBVH()
    {
        var selection = Selection.activeTransform;

        var filename = EditorUtility.SaveFilePanel("Export selection to BVH", "", selection.root.name, "bvh");

        if (!string.IsNullOrEmpty(filename))
            CharacterToFile(selection, filename);
    }

    [MenuItem("Reflecta/BVH/Export BVH", true)]
    private static bool ExportBVHValidate()
    {
        var selection = Selection.activeTransform;

        return selection != null && selection.GetComponents<Component>().Length == 1;
    }

    private const float GlobalScale = 100.0f;
    private static readonly HumanTemplate Template = new HumanTemplate();
    private static readonly Dictionary<string, Quaternion> BaseRotations = new Dictionary<string, Quaternion>();

    private static void ZeroBones(Transform bone)
    {
        bone.transform.rotation = Quaternion.identity;

        foreach (Transform child in bone)
            if (child.GetComponents<Component>().Length == 1)
                ZeroBones(child);
    }

    private static string ValidBone(string bone)
    {
        //var templateBone = Template.Find(bone);
        //if (!string.IsNullOrEmpty(templateBone))
        //    return templateBone;

        return bone;
    }

    private static string BoneToString(Transform bone, int indent)
    {
        BaseRotations.Add(bone.name, bone.localRotation);

        var sb = new StringBuilder();

        var tabs = string.Empty;
        for (var i = 0; i < indent; i++)
        {
            tabs = tabs + "\t";
        }

        sb.Append(tabs);
        sb.Append("JOINT ");
        sb.AppendLine(ValidBone(bone.name));
        sb.Append(tabs);
        sb.AppendLine("{");

        var offset = (bone.position - bone.parent.position)*GlobalScale;

        sb.Append(tabs);
        sb.AppendFormat("\tOFFSET {0:f6} {1:f6} {2:f6}", -offset.x, offset.y, offset.z);
        sb.AppendLine();
        sb.Append(tabs);
        sb.AppendLine("\tCHANNELS 3 Zrotation Xrotation Yrotation");

        foreach (Transform child in bone)
            sb.Append(BoneToString(child, indent + 1));

        if (bone.childCount == 0)
        {
            var end = bone.rotation*Vector3.up*bone.localPosition.magnitude*0.1f*GlobalScale;

            sb.Append(tabs);
            sb.AppendLine("\tEnd Site");
            sb.Append(tabs);
            sb.AppendLine("\t{");
            sb.Append(tabs);
            sb.AppendFormat("\t\tOFFSET {0:f6} {1:f6} {2:f6}", -end.x, end.y, end.z);
            sb.AppendLine();
            sb.Append(tabs);
            sb.AppendLine("\t}");
        }

        sb.Append(tabs);
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string SkeletonToString(Transform skeleton)
    {
        ZeroBones(skeleton);

        Template.ClearTemplate();

        var animator = skeleton.root.GetComponent<Animator>();

        foreach (HumanBodyBones humanBodyBoneEnum in Enum.GetValues(typeof (HumanBodyBones)))
        {
            var humanBodyBoneTransform = animator.GetBoneTransform(humanBodyBoneEnum);

            if (humanBodyBoneTransform != null)
                Template.Insert(humanBodyBoneTransform.name, humanBodyBoneEnum.ToString());
        }

        BaseRotations.Clear();

        BaseRotations.Add(skeleton.name, skeleton.localRotation);

        var sb = new StringBuilder();

        sb.Append("ROOT ");
        sb.AppendLine(ValidBone(skeleton.name));
        sb.AppendLine("{");

        var offset = (skeleton.position - skeleton.parent.position)*GlobalScale;

        sb.AppendFormat("\tOFFSET {0:f6} {1:f6} {2:f6}", -offset.x, offset.y, offset.z);
        sb.AppendLine();
        sb.AppendLine("\tCHANNELS 6 Xposition Yposition Zposition Zrotation Xrotation Yrotation");

        foreach (Transform child in skeleton)
            sb.Append(BoneToString(child, 1));

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string BoneRotationToString(Transform bone)
    {
        var sb = new StringBuilder();

        var rotation = bone.localRotation*Quaternion.Inverse(BaseRotations[bone.name]);
        rotation.w = -rotation.w;

        sb.AppendFormat("{0:f6} {1:f6} {2:f6} ", rotation.eulerAngles.z, -rotation.eulerAngles.x,
            rotation.eulerAngles.y);

        foreach (Transform child in bone)
            sb.Append(BoneRotationToString(child));

        return sb.ToString();
    }

    private static string PoseToString(Transform skeleton)
    {
        var sb = new StringBuilder();

        var position = (skeleton.position - skeleton.parent.position)*GlobalScale;

        sb.AppendFormat("{0:f6} {1:f6} {2:f6} ", -position.x, position.y, position.z);
        sb.Append(BoneRotationToString(skeleton));
        sb.AppendLine();

        return sb.ToString();
    }

    private static string CharacterToString(Transform skeleton)
    {
        var sb = new StringBuilder();

        sb.AppendLine("HIERARCHY");

        sb.Append(SkeletonToString(skeleton));

        sb.AppendLine("MOTION");

        var animation = skeleton.root.GetComponent<Animation>();
        var clip = animation.clip;
        var timeStep = 1f/clip.frameRate;
        var frameCount = Mathf.CeilToInt(clip.length/timeStep);
        animation[clip.name].enabled = true;
        animation[clip.name].weight = 1f;
        animation[clip.name].time = 0f;
        animation.Sample();

        sb.Append("Frames: ");
        sb.Append(frameCount);
        sb.AppendLine();
        sb.AppendFormat("Frame Time: {0:f6}", timeStep);
        sb.AppendLine();

        for (var frame = 0; frame < frameCount; frame++)
        {
            sb.Append(PoseToString(skeleton));
            animation[clip.name].time += timeStep;
            animation.Sample();
        }

        return sb.ToString();
    }

    private static void CharacterToFile(Transform skeleton, string filename)
    {
        var encoding = new UTF8Encoding(false);

        using (var sw = new StreamWriter(filename, false, encoding))
        {
            sw.Write(CharacterToString(skeleton));
        }
    }
}