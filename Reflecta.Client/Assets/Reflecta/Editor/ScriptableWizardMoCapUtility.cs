#region Using

using System;
using System.Collections.Generic;
using System.IO;
using Reflecta;
using UnityEditor;
using UnityEngine;
using Quaternion = Reflecta.Quaternion;
using Vector3 = Reflecta.Vector3;

#endregion

public sealed class ScriptableWizardMoCapUtility : ScriptableWizard
{
    private static GameObject RootObject;
    private static string DataFile = "MoCap.mocap";
    private MoCapData Data;
    public bool EnableSmoothing = true;
    public bool ProcessBodyTransforms = true;
    public bool ProcessFaceBlendShapes = true;
    public bool ProcessFaceTransforms = true;
    public int Smoothness = 4;

    private static void SetAnimationCurvesForBody(Animator animator, GameObject rootObject, TransformTime[] transforms,
        AnimationClip animationClip, MoCapMecanimBone bone, bool setPosition, bool setRotation, bool setScale)
    {
        if (animator == null)
            throw new Exception("Animator can not be null!");
        if (rootObject == null)
            throw new Exception("Root object can not be null!");
        if (transforms == null || transforms.Length == 0)
            throw new Exception("Transforms can not be empty!");
        if (animationClip == null)
            throw new Exception("Animation clip can not be null!");
        if (!MoCapBoneMapper.IsValidMecanimBone(bone))
            throw new Exception("Invalid Mecanim bone!");

        var relativeTransform = animator.GetBoneTransform((HumanBodyBones) bone);
        var relativePath = AnimationUtility.CalculateTransformPath(relativeTransform, rootObject.transform);

        var keyframesTransformPositionX = new Keyframe[transforms.Length];
        var keyframesTransformPositionY = new Keyframe[transforms.Length];
        var keyframesTransformPositionZ = new Keyframe[transforms.Length];

        var keyframesTransformRotationX = new Keyframe[transforms.Length];
        var keyframesTransformRotationY = new Keyframe[transforms.Length];
        var keyframesTransformRotationZ = new Keyframe[transforms.Length];
        var keyframesTransformRotationW = new Keyframe[transforms.Length];

        var keyframesTransformScaleX = new Keyframe[transforms.Length];
        var keyframesTransformScaleY = new Keyframe[transforms.Length];
        var keyframesTransformScaleZ = new Keyframe[transforms.Length];

        for (var i = 0; i < transforms.Length; i++)
        {
            var transform = transforms[i];

            keyframesTransformPositionX[i] = new Keyframe(transform.Time, transform.Position.X);
            keyframesTransformPositionY[i] = new Keyframe(transform.Time, transform.Position.Y);
            keyframesTransformPositionZ[i] = new Keyframe(transform.Time, transform.Position.Z);

            keyframesTransformRotationX[i] = new Keyframe(transform.Time, transform.Rotation.X);
            keyframesTransformRotationY[i] = new Keyframe(transform.Time, transform.Rotation.Y);
            keyframesTransformRotationZ[i] = new Keyframe(transform.Time, transform.Rotation.Z);
            keyframesTransformRotationW[i] = new Keyframe(transform.Time, transform.Rotation.W);

            keyframesTransformScaleX[i] = new Keyframe(transform.Time, transform.Scale.X);
            keyframesTransformScaleY[i] = new Keyframe(transform.Time, transform.Scale.Y);
            keyframesTransformScaleZ[i] = new Keyframe(transform.Time, transform.Scale.Z);
        }

        var animationCurvePositionX = new AnimationCurve(keyframesTransformPositionX);
        var animationCurvePositionY = new AnimationCurve(keyframesTransformPositionY);
        var animationCurvePositionZ = new AnimationCurve(keyframesTransformPositionZ);
        if (setPosition)
        {
            animationClip.SetCurve(relativePath, typeof (Transform), "localPosition.x", animationCurvePositionX);
            animationClip.SetCurve(relativePath, typeof (Transform), "localPosition.y", animationCurvePositionY);
            animationClip.SetCurve(relativePath, typeof (Transform), "localPosition.z", animationCurvePositionZ);
        }

        var animationCurveRotationX = new AnimationCurve(keyframesTransformRotationX);
        var animationCurveRotationY = new AnimationCurve(keyframesTransformRotationY);
        var animationCurveRotationZ = new AnimationCurve(keyframesTransformRotationZ);
        var animationCurveRotationW = new AnimationCurve(keyframesTransformRotationW);
        if (setRotation)
        {
            animationClip.SetCurve(relativePath, typeof (Transform), "localRotation.x", animationCurveRotationX);
            animationClip.SetCurve(relativePath, typeof (Transform), "localRotation.y", animationCurveRotationY);
            animationClip.SetCurve(relativePath, typeof (Transform), "localRotation.z", animationCurveRotationZ);
            animationClip.SetCurve(relativePath, typeof (Transform), "localRotation.w", animationCurveRotationW);
        }

        var animationCurveScaleX = new AnimationCurve(keyframesTransformScaleX);
        var animationCurveScaleY = new AnimationCurve(keyframesTransformScaleY);
        var animationCurveScaleZ = new AnimationCurve(keyframesTransformScaleZ);
        if (setScale)
        {
            animationClip.SetCurve(relativePath, typeof (Transform), "localScale.x", animationCurveScaleX);
            animationClip.SetCurve(relativePath, typeof (Transform), "localScale.y", animationCurveScaleY);
            animationClip.SetCurve(relativePath, typeof (Transform), "localScale.z", animationCurveScaleZ);
        }
    }

    private static void SetAnimationCurvesForFace(Animator animator, GameObject rootObject,
        KeyValuePair<float, float>[] weights, AnimationClip animationClip, MoCapMixamoFacialExpression expression)
    {
        if (animator == null)
            throw new Exception("Animator can not be null!");
        if (rootObject == null)
            throw new Exception("Root object can not be null!");
        if (weights == null || weights.Length == 0)
            throw new Exception("Weights can not be empty!");
        if (animationClip == null)
            throw new Exception("Animation clip can not be null!");
        if (!MoCapFacialExpressionMapper.IsValidMixamoFacialExpression(expression))
            throw new Exception("Invalid Mixamo facial expression!");

        var keyframes = new Keyframe[weights.Length];
        for (var i = 0; i < weights.Length; i++)
            keyframes[i] = new Keyframe(weights[i].Key, weights[i].Value);

        var animationCurve = new AnimationCurve(keyframes);

        animationClip.SetCurve("Body", typeof (SkinnedMeshRenderer),
            string.Format("blendShape.Facial_Blends.{0}", expression), animationCurve);

        if (expression == MoCapMixamoFacialExpression.Blink_Left ||
            expression == MoCapMixamoFacialExpression.Blink_Right)
            animationClip.SetCurve("Eyelashes", typeof (SkinnedMeshRenderer),
                string.Format("blendShape.Facial_Blends.{0}", expression), animationCurve);
    }

    private static AnimationClip GetAnimationClip(Animator animator, GameObject rootObject, MoCapData data,
        bool processBodyTransforms, bool processFaceTransforms, bool processFaceBlendShapes)
    {
        var animationClip = new AnimationClip();

        animationClip.name = Path.GetFileNameWithoutExtension(DataFile);
        animationClip.legacy = true;
        animationClip.wrapMode = WrapMode.Once;
        animationClip.frameRate = 25;

        animationClip.ClearCurves();

        if (data.BodyFrames != null && data.BodyFrames.Length > 0)
        {
            if (processBodyTransforms)
            {
                var transforms = new TransformTime[(int) MoCapKinectBone.Count][];

                for (var i = 0; i < (int) MoCapKinectBone.Count; i++)
                {
                    transforms[i] = new TransformTime[data.BodyFrames.Length];

                    for (var j = 0; j < data.BodyFrames.Length; j++)
                    {
                        transforms[i][j].Time = data.BodyFrames[j].SkeletonTransforms[i].Time;
                        transforms[i][j].Position = Vector3.Zero;
                        transforms[i][j].Rotation = MoCapBoneMapper.LocalRotation(ref data.BodyFrames[j],
                            (MoCapKinectBone) i);
                        transforms[i][j].Scale = Vector3.One;
                    }
                }

                foreach (MoCapKinectBone kinectBone in Enum.GetValues(typeof (MoCapKinectBone)))
                    if (MoCapBoneMapper.IsValidKinectBone(kinectBone))
                    {
                        var mecanimBone = MoCapBoneMapper.Kinect2Mecanim(kinectBone);
                        if (MoCapBoneMapper.IsValidMecanimBone(mecanimBone))
                            SetAnimationCurvesForBody(animator, rootObject, transforms[(int) kinectBone], animationClip,
                                mecanimBone, false, true, false);
                    }
            }
        }

        if (data.FaceFrames != null && data.FaceFrames.Length > 0)
        {
            if (processFaceTransforms)
            {
                var transforms = new TransformTime[data.FaceFrames.Length];

                for (var i = 0; i < data.FaceFrames.Length; i++)
                {
                    transforms[i].Time = data.FaceFrames[i].FaceTransform.Time;
                    transforms[i].Position = Vector3.Zero;
                    transforms[i].Rotation = new Quaternion(-data.FaceFrames[i].FaceTransform.Rotation.X,
                        data.FaceFrames[i].FaceTransform.Rotation.Y, data.FaceFrames[i].FaceTransform.Rotation.Z,
                        data.FaceFrames[i].FaceTransform.Rotation.W);
                    transforms[i].Scale = Vector3.One;
                }

                SetAnimationCurvesForBody(animator, rootObject, transforms, animationClip, MoCapMecanimBone.Head, false,
                    true, false);
            }

            if (processFaceBlendShapes)
            {
                var weights =
                    new List<KeyValuePair<float, float>>[(int) MoCapMixamoFacialExpression.LastBlendShape];

                foreach (
                    MoCapKinectFacialExpression kinectFacialExpression in
                        Enum.GetValues(typeof (MoCapKinectFacialExpression)))
                    if (MoCapFacialExpressionMapper.IsValidKinectFacialExpression(kinectFacialExpression))
                    {
                        for (var j = 0; j < data.FaceFrames.Length; j++)
                        {
                            MoCapMixamoFacialExpression mixamoFacialExpression;
                            float mixamoWeight;
                            MoCapFacialExpressionMapper.Kinect2Mixamo(kinectFacialExpression,
                                data.FaceFrames[j].ExpressionWeights[(int) kinectFacialExpression],
                                out mixamoFacialExpression, out mixamoWeight);

                            if (MoCapFacialExpressionMapper.IsValidMixamoFacialExpression(mixamoFacialExpression))
                            {
                                if (weights[(int) mixamoFacialExpression] == null)
                                    weights[(int) mixamoFacialExpression] =
                                        new List<KeyValuePair<float, float>>(data.FaceFrames.Length);

                                weights[(int) mixamoFacialExpression].Add(
                                    new KeyValuePair<float, float>(data.FaceFrames[j].FaceTransform.Time, mixamoWeight));
                            }
                        }
                    }

                foreach (
                    MoCapMixamoFacialExpression mixamoFacialExpression in
                        Enum.GetValues(typeof (MoCapMixamoFacialExpression)))
                    if (MoCapFacialExpressionMapper.IsValidMixamoFacialExpression(mixamoFacialExpression))
                        if (weights[(int) mixamoFacialExpression] != null &&
                            weights[(int) mixamoFacialExpression].Count > 0)
                            SetAnimationCurvesForFace(animator, rootObject,
                                weights[(int) mixamoFacialExpression].ToArray(), animationClip, mixamoFacialExpression);
            }
        }

        animationClip.EnsureQuaternionContinuity();

        return animationClip;
    }

    [MenuItem("Reflecta/Wizard/MoCap Utility")]
    private static void CreateWizard()
    {
        DisplayWizard<ScriptableWizardMoCapUtility>("MoCap", "Create", "Open");

        RootObject = GameObject.Find("AvatarFemale");
    }

    private void OnWizardCreate()
    {
        if (RootObject == null)
        {
            EditorUtility.DisplayDialog("Error!", "Please select a valid root object!", "OK");
            return;
        }

        var animator = RootObject.GetComponent<Animator>();

        if (animator == null)
        {
            EditorUtility.DisplayDialog("Error!", "Please attach a valid animator to the root object!", "OK");
            return;
        }

        if (!File.Exists(DataFile))
        {
            EditorUtility.DisplayDialog("Error!", "Please select a valid motion capture data file!", "OK");
            return;
        }

        if (Smoothness <= 0)
        {
            EditorUtility.DisplayDialog("Error!", "Please specify a valid smoothness factor!", "OK");
            return;
        }

        MoCapData.Deserialize(DataFile, out Data);
        if (EnableSmoothing)
            MoCapData.Average(ref Data, Smoothness);

        AssetDatabase.StartAssetEditing();

        var animationClip = GetAnimationClip(animator, RootObject, Data, ProcessBodyTransforms,
            ProcessFaceTransforms, ProcessFaceBlendShapes);

        AssetDatabase.CreateAsset(animationClip, "Assets/" + animationClip.name + ".anim");
        AssetDatabase.StopAssetEditing();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void OnWizardOtherButton()
    {
        DataFile = EditorUtility.OpenFilePanel("Open MoCap Data File", EditorApplication.currentScene, "mocap");
    }
}