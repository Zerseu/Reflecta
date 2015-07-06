#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
#if UNITY_STANDALONE
using System.IO.Pipes;
using System.Runtime.InteropServices;
using Reflecta;

#endif

#endregion

[RequireComponent(typeof (Camera))]
public sealed class ClientScript : MonoBehaviour
{
    private Animator AttachedAnimator;
    private SkinnedMeshRenderer AttachedBodySkinnedMeshRenderer;
    private SkinnedMeshRenderer AttachedEyelashesSkinnedMeshRenderer;
    public GameObject AttachedGameObject;

    private void Start()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 25;

        GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        GetComponent<Camera>().backgroundColor = new Color(0f, 0f, 0f, 0f);

        AttachedAnimator = AttachedGameObject.GetComponent<Animator>();
        AttachedBodySkinnedMeshRenderer =
            AttachedGameObject.transform.FindChild("Body").GetComponent<SkinnedMeshRenderer>();
        AttachedEyelashesSkinnedMeshRenderer =
            AttachedGameObject.transform.FindChild("Eyelashes").GetComponent<SkinnedMeshRenderer>();

        var renderers = AttachedGameObject.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            var movieTexture = renderer.material.mainTexture as MovieTexture;
            if (movieTexture != null && !movieTexture.isPlaying)
            {
                movieTexture.loop = true;
                movieTexture.Play();
            }
        }

#if UNITY_STANDALONE
        cameraCurrentPosition = transform.localPosition;
        cameraTargetPosition = transform.localPosition;

        OpenPipes();

        StartCoroutine(BeginWritePixels());

        BeginReadCommand();
#endif
    }

#if UNITY_STANDALONE
    private NamedPipeClientStream RenderingStream;
    private NamedPipeClientStream CommandStream;

    private readonly object Lock = new object();

    private volatile bool IsWritingPixels;

    private readonly byte[] Command = new byte[Constants.COMMAND_PIPELINE_BUFFER_SIZE];

    private readonly Vector3 VolatilePosition = Vector3.zero;
    private readonly Quaternion VolatileRotation = Quaternion.identity;
    private readonly Vector3 VolatileScale = Vector3.one;

    private readonly bool[] KeyPressed = new bool[256];

    private readonly Queue<Viseme> VisemeQueue = new Queue<Viseme>();

    private readonly Queue<string> VoiceCommandQueue = new Queue<string>();

    private readonly Queue<MoCapBodyFrame> KinectBodyQueue = new Queue<MoCapBodyFrame>();

    private readonly Queue<MoCapFaceFrame> KinectFaceQueue = new Queue<MoCapFaceFrame>();
#endif
#if UNITY_STANDALONE
    private void OpenPipes()
    {
        RenderingStream = new NamedPipeClientStream(".", "Rendering Pipeline", PipeDirection.Out,
            PipeOptions.Asynchronous);
        RenderingStream.Connect();

        CommandStream = new NamedPipeClientStream(".", "Command Pipeline", PipeDirection.In, PipeOptions.Asynchronous);
        CommandStream.Connect();
    }

    private void ClosePipes()
    {
        RenderingStream.Close();
        RenderingStream.Dispose();
        RenderingStream = null;

        CommandStream.Close();
        CommandStream.Dispose();
        CommandStream = null;
    }

    private void BeginReadCommand()
    {
        if (CommandStream != null && CommandStream.IsConnected)
        {
            CommandStream.BeginRead(Command, 0, Constants.COMMAND_PIPELINE_BUFFER_SIZE, ReadCommandAsyncCallback, null);
        }
    }

    private void ReadCommandAsyncCallback(IAsyncResult result)
    {
        if (CommandStream != null && CommandStream.IsConnected)
        {
            var command = CommandMessage.FromBytes(Command);

            switch (command.CommandType)
            {
                case CommandType.Viseme:
                    lock (Lock)
                    {
                        VisemeQueue.Enqueue(command.Viseme);
                    }
                    break;

                case CommandType.VoiceCommand:
                    lock (Lock)
                    {
                        VoiceCommandQueue.Enqueue(command.VoiceCommand);
                    }
                    break;

                case CommandType.KinectBody:
                    lock (Lock)
                    {
                        KinectBodyQueue.Enqueue(command.KinectBody);
                    }
                    break;

                case CommandType.KinectFace:
                    lock (Lock)
                    {
                        KinectFaceQueue.Enqueue(command.KinectFace);
                    }
                    break;

                case CommandType.KeyPress:
                    var keyPress = command.KeyPress;
                    break;

                case CommandType.KeyDown:
                    var keyDown = command.KeyDown;
                    lock (Lock)
                    {
                        if (keyDown < 256)
                            KeyPressed[keyDown] = true;
                    }
                    break;

                case CommandType.KeyUp:
                    var keyUp = command.KeyUp;
                    lock (Lock)
                    {
                        if (keyUp < 256)
                            KeyPressed[keyUp] = false;
                    }
                    break;

                case CommandType.MouseMove:
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        BeginReadCommand();
    }

    private void OnApplicationQuit()
    {
        ClosePipes();
    }

    private Vector3 cameraCurrentPosition;
    private Vector3 cameraTargetPosition;
    private static readonly Vector3 CAMERA_BODY_POSITION = new Vector3(0.0f, 0.9f, -1.7f);
    private static readonly Vector3 CAMERA_FACE_POSITION = new Vector3(0.0f, 1.6f, -0.4f);
    private const float MOVEMENT_SCALE = 0.25f;
    private const float MAX_DISTANCE_DELTA = 0.1f;
    private const int KINECT_BODY_QUEUE_SIZE = 4;
    private const int KINECT_FACE_QUEUE_SIZE = 4;

    private void Update()
    {
        lock (Lock)
        {
            if (AttachedGameObject != null)
            {
                AttachedGameObject.transform.localPosition = VolatilePosition;
                AttachedGameObject.transform.localRotation = VolatileRotation;
                AttachedGameObject.transform.localScale = VolatileScale;

                cameraCurrentPosition = transform.localPosition;
                if (VoiceCommandQueue.Count > 0)
                {
                    var voiceCommand = VoiceCommandQueue.Dequeue();

                    switch (voiceCommand)
                    {
                        case "forward":
                            cameraTargetPosition += Vector3.forward*MOVEMENT_SCALE;
                            break;
                        case "backward":
                            cameraTargetPosition += Vector3.back*MOVEMENT_SCALE;
                            break;
                        case "up":
                            cameraTargetPosition += Vector3.up*MOVEMENT_SCALE;
                            break;
                        case "down":
                            cameraTargetPosition += Vector3.down*MOVEMENT_SCALE;
                            break;
                        case "right":
                            cameraTargetPosition += Vector3.right*MOVEMENT_SCALE;
                            break;
                        case "left":
                            cameraTargetPosition += Vector3.left*MOVEMENT_SCALE;
                            break;

                        case "body":
                            cameraTargetPosition = CAMERA_BODY_POSITION;
                            break;
                        case "face":
                            cameraTargetPosition = CAMERA_FACE_POSITION;
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
                transform.localPosition = Vector3.MoveTowards(cameraCurrentPosition, cameraTargetPosition,
                    MAX_DISTANCE_DELTA);

                if (VisemeQueue.Count > 0)
                {
                    var viseme = VisemeQueue.Dequeue();

                    AttachedAnimator.SetInteger("VisemeID", (int) viseme);
                }

                while (KinectBodyQueue.Count > KINECT_BODY_QUEUE_SIZE)
                    KinectBodyQueue.Dequeue();
                if (KinectBodyQueue.Count > 0)
                {
                    var kinectBody = MoCapBodyFrame.Average(KinectBodyQueue.ToArray());

                    if (kinectBody.SkeletonTransforms != null && kinectBody.SkeletonTransforms.Length > 0)
                    {
                        foreach (MoCapKinectBone kinectBone in Enum.GetValues(typeof (MoCapKinectBone)))
                            if (MoCapBoneMapper.IsValidKinectBone(kinectBone))
                            {
                                var mecanimBone = MoCapBoneMapper.Kinect2Mecanim(kinectBone);

                                if (MoCapBoneMapper.IsValidMecanimBone(mecanimBone))
                                {
                                    var humanBodyBone = (HumanBodyBones) mecanimBone;
                                    var mecanimBoneName = mecanimBone.ToString().ToLower();
                                    var boneTransform = AttachedAnimator.GetBoneTransform(humanBodyBone);

                                    if (boneTransform != null)
                                    {
                                        var localRotation = MoCapBoneMapper.LocalRotation(ref kinectBody,
                                            kinectBone);

                                        boneTransform.localRotation = new Quaternion(localRotation.X, localRotation.Y,
                                            localRotation.Z, localRotation.W);
                                    }
                                }
                            }
                    }
                }

                while (KinectFaceQueue.Count > KINECT_FACE_QUEUE_SIZE)
                    KinectFaceQueue.Dequeue();
                if (KinectFaceQueue.Count > 0)
                {
                    MoCapFaceFrame kinectFace;

                    kinectFace = MoCapFaceFrame.Average(KinectFaceQueue.ToArray());

                    var headTransform = AttachedAnimator.GetBoneTransform(HumanBodyBones.Head);

                    var headRotation = new Quaternion(-kinectFace.FaceTransform.Rotation.X,
                        kinectFace.FaceTransform.Rotation.Y, kinectFace.FaceTransform.Rotation.Z,
                        kinectFace.FaceTransform.Rotation.W);

                    headTransform.localRotation = headRotation;

                    foreach (
                        MoCapKinectFacialExpression kinectFacialExpression in
                            Enum.GetValues(typeof (MoCapKinectFacialExpression)))
                        if (MoCapFacialExpressionMapper.IsValidKinectFacialExpression(kinectFacialExpression))
                        {
                            var kinectWeight = kinectFace.ExpressionWeights[(int) kinectFacialExpression];

                            MoCapMixamoFacialExpression mixamoFacialExpression;
                            float mixamoWeight;
                            MoCapFacialExpressionMapper.Kinect2Mixamo(kinectFacialExpression, kinectWeight,
                                out mixamoFacialExpression, out mixamoWeight);

                            if (MoCapFacialExpressionMapper.IsValidMixamoFacialExpression(mixamoFacialExpression))
                            {
                                AttachedBodySkinnedMeshRenderer.SetBlendShapeWeight((int) mixamoFacialExpression,
                                    mixamoWeight);

                                if (mixamoFacialExpression == MoCapMixamoFacialExpression.Blink_Left ||
                                    mixamoFacialExpression == MoCapMixamoFacialExpression.Blink_Right)
                                    AttachedEyelashesSkinnedMeshRenderer.SetBlendShapeWeight(
                                        (int) mixamoFacialExpression, mixamoWeight);
                            }
                        }
                }
            }
        }
    }

    private static byte[] Color32ArrayToByteArray(Color32[] colors)
    {
        unchecked
        {
            if (colors == null || colors.Length == 0)
                return null;

            var bytes = new byte[colors.Length*4];

            var handle = default(GCHandle);
            try
            {
                handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
                var ptr = handle.AddrOfPinnedObject();
                Marshal.Copy(ptr, bytes, 0, bytes.Length);
            }
            finally
            {
                if (handle != default(GCHandle))
                    handle.Free();
            }

            return bytes;
        }
    }

    private static Color32[] ByteArrayToColor32Array(byte[] bytes)
    {
        unchecked
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            var colors = new Color32[bytes.Length/4];

            var handle = default(GCHandle);
            try
            {
                handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
                var ptr = handle.AddrOfPinnedObject();
                Marshal.Copy(bytes, 0, ptr, bytes.Length);
            }
            finally
            {
                if (handle != default(GCHandle))
                    handle.Free();
            }

            return colors;
        }
    }

    private IEnumerator BeginWritePixels()
    {
        while (RenderingStream != null && RenderingStream.IsConnected)
        {
            yield return new WaitForEndOfFrame();

            if (!IsWritingPixels)
            {
                var screen = new Texture2D(Constants.SCREEN_WIDTH, Constants.SCREEN_HEIGHT, TextureFormat.ARGB32,
                    false, true);

                screen.ReadPixels(new Rect(0, 0, Constants.SCREEN_WIDTH, Constants.SCREEN_HEIGHT), 0, 0, false);

                var pixels = Color32ArrayToByteArray(screen.GetPixels32(0));

                Destroy(screen);

                IsWritingPixels = true;

                RenderingStream.WaitForPipeDrain();

                RenderingStream.BeginWrite(pixels, 0, Constants.SCREEN_BYTE_COUNT, WritePixelsAsyncCallback, null);
            }
        }
    }

    private void WritePixelsAsyncCallback(IAsyncResult result)
    {
        if (RenderingStream != null && RenderingStream.IsConnected)
        {
            RenderingStream.EndWrite(result);

            RenderingStream.Flush();

            IsWritingPixels = false;
        }
    }
#endif
}